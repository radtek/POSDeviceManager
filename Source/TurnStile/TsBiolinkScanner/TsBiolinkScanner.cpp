// This is the main DLL file.

#include "stdafx.h"
#include "TsBiolinkScanner.h"
#define MAX_QUALITY_THREASHOLD 60
#define MIN_QUALITY_THREASHOLD 30
#define SCAN_TIMEOUT 10000

INetPass *pINetPass = NULL;
INetPassScanner *pIScannerDevice = NULL;
INetPassAccess *pIAccessDevice = NULL;

TsBiolinkScanner::TsBiolinkScannerDevice::TsBiolinkScannerDevice()
{
}

TsBiolinkScanner::TsBiolinkScannerDevice::~TsBiolinkScannerDevice()
{
	if(_license != nullptr)
		_license->Close();
	if (pIAccessDevice)
		pIAccessDevice->Release();
	if (pIScannerDevice)
		pIScannerDevice->Release();
	if (pINetPass)
		pINetPass->Release();
	_license = nullptr;
	pIAccessDevice = NULL;
	pIScannerDevice = NULL;
	pINetPass = NULL;
	if(hApi)
	{
		((HRESULT ((WINAPI *)(DWORD))) ::GetProcAddress (hApi, "CanBeUnload"))(INFINITE);
		::FreeLibrary (hApi);
	}
	hApi = NULL;
}

void TsBiolinkScanner::TsBiolinkScannerDevice::Active::set(bool value)
{
	_active = false;
	if(value)
	{
		// получение лицензии
		_license = gcnew Biolink::Biometrics2::License(Biolink::Biometrics2::LicenseType::ALL);
		
		// загрузка библиотеки
		hApi = ::LoadLibrary (TEXT("BlNetpassApi.dll"));
		if(!hApi)
			throw gcnew System::Exception (L"Не найдена библиотека BlNetpassApi.dll");

		HRESULT (WINAPI * pNetPassCreate)(LPCSTR, const GUID*, void **);
		pNetPassCreate = (HRESULT (WINAPI *)(LPCSTR, const GUID*, void **))::GetProcAddress (hApi, "BlNetPassCreateA");
		if (NULL == pNetPassCreate)
			throw gcnew System::Exception (L"Не найдена функция BlNetPassCreateA в библиотеке BlNetpassApi.dll");
		
		if (ERROR_SUCCESS != (pNetPassCreate)(NULL, NULL, (void **)&pINetPass))
			throw gcnew System::Exception (L"Не удалось получить общий интерфейс INetPass");

		_active = true;
	}
	else
	{
		if(_license != nullptr)
			_license->Close();
		if (pIScannerDevice)
			pIScannerDevice->Release();
		if (pIAccessDevice)
			pIAccessDevice->Release();
		if (pINetPass)
			pINetPass->Release();

		_license = nullptr;
		pIAccessDevice = NULL;
		pIScannerDevice = NULL;
		pINetPass = NULL;

		if(hApi)
		{
			((HRESULT ((WINAPI *)(DWORD))) ::GetProcAddress (hApi, "CanBeUnload"))(INFINITE);
			::FreeLibrary (hApi);
		}
		hApi = NULL;
	}
}

System::String ^TsBiolinkScanner::TsBiolinkScannerDevice::IdentificationData::get()
{ 	
	if(!_active)
		return String::Empty; 
		//throw gcnew System::Exception (L"Устройство не активно");

	// подключение к устройствам
	ConnectAccessDevice();

	// получение изображения
	BYTE dataBuffer[1024 * 256];
	DWORD dataBufferLen = sizeof(dataBuffer);
	HRESULT hResult;
	DWORD dImageSize;
	if (ERROR_SUCCESS != (hResult = pIScannerDevice->GetImage((LPBYTE)dataBuffer, dataBufferLen, 
		&dImageSize, SCAN_TIMEOUT)))
		return String::Empty; 

	array<Byte>^ imageArray = gcnew array<Byte>(dImageSize);
	System::Runtime::InteropServices::Marshal::Copy((IntPtr)dataBuffer, imageArray, 0, dImageSize);

	// проверка качества изображения
	Biolink::Biometrics2::Image ^image = gcnew Biolink::Biometrics2::Image(imageArray);
	if(image->ExpressQuality() >= MAX_QUALITY_THREASHOLD)
	{
		// запуск сценария "2" (произведено успешное сканирование)
//		pIAccessDevice->SetEvent(2);

		// формирование шаблона
		Biolink::Biometrics2::ImageSet ^imageSet = gcnew Biolink::Biometrics2::ImageSet();
		imageSet->AddImage(image, Biolink::Biometrics2::FingerCode::Unknown);
		Biolink::Biometrics2::ImageProcessor ^processor = gcnew Biolink::Biometrics2::ImageProcessor(Biolink::Biometrics2::MathType::BIOLINK);
		Biolink::Biometrics2::Template ^imageTemplate = processor->CreateTemplate(imageSet);
		return System::Convert::ToBase64String(imageTemplate->ToArray());
	}
	if(image->ExpressQuality() >= MIN_QUALITY_THREASHOLD)
		// запуск сценария "4" (ошибка распознования)
		pIAccessDevice->SetEvent(4);
	return String::Empty; 
}

bool TsBiolinkScanner::TsBiolinkScannerDevice::Open()
{
	if(!_active)
		throw gcnew System::Exception (L"Устройство не активно");

	ConnectAccessDevice();
	// запуск сценария "3" (открытие двери)
	return ERROR_SUCCESS == pIAccessDevice->SetEvent(3);
}

void TsBiolinkScanner::TsBiolinkScannerDevice::Close(bool accessDenied)
{
	if(!_active)
		throw gcnew System::Exception (L"Устройство не активно");
	ConnectAccessDevice();
	pIAccessDevice->SetEvent(5);
}

void TsBiolinkScanner::TsBiolinkScannerDevice::ConnectScannerDevice()
{
	// получаем интерфейс устройства
	if(pIScannerDevice)
		return;

	// получение списка устройств
	INetPassEnum *pIEnum = NULL;
	if(ERROR_SUCCESS == pINetPass->CreateNetPassEnum((void **)&pIEnum, CLSID_MatchBook,
		IID_INetPassEnum))
		// получение устройства по идентификатору
		pIEnum->CreateDevice((void**) &pIScannerDevice, IID_INetPassDevice, Address);
	else
		throw gcnew System::Exception (L"Ошибка при получении интерфейса биометрического сканера");

	if (pIEnum)
		pIEnum->Release();
	if(!pIScannerDevice)
		throw gcnew System::Exception (L"Сканер с переданным идентификатором не найден");
}

void TsBiolinkScanner::TsBiolinkScannerDevice::ConnectAccessDevice()
{
	// подключение к сканеру
	ConnectScannerDevice();

	// получаем интерфейс устройства
	if(pIAccessDevice)
		return;
	
	if(ERROR_SUCCESS != pIScannerDevice->QueryInterface(IID_INetPassAccess, (void **)&pIAccessDevice))
		throw gcnew System::Exception (L"Ошибка при получении интерфейса устройства управления доступом");

	// установка параметров устройства
	DWORD dValue = 0;
	pIAccessDevice->WriteDeviceReg(4, &dValue, sizeof(DWORD), NULL); 
	dValue = _timeout * 1000;
	pIAccessDevice->WriteDeviceReg(16, &dValue, sizeof(DWORD), NULL);
}
