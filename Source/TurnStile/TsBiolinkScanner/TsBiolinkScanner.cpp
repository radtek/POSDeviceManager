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
		// ��������� ��������
		_license = gcnew Biolink::Biometrics2::License(Biolink::Biometrics2::LicenseType::ALL);
		
		// �������� ����������
		hApi = ::LoadLibrary (TEXT("BlNetpassApi.dll"));
		if(!hApi)
			throw gcnew System::Exception (L"�� ������� ���������� BlNetpassApi.dll");

		HRESULT (WINAPI * pNetPassCreate)(LPCSTR, const GUID*, void **);
		pNetPassCreate = (HRESULT (WINAPI *)(LPCSTR, const GUID*, void **))::GetProcAddress (hApi, "BlNetPassCreateA");
		if (NULL == pNetPassCreate)
			throw gcnew System::Exception (L"�� ������� ������� BlNetPassCreateA � ���������� BlNetpassApi.dll");
		
		if (ERROR_SUCCESS != (pNetPassCreate)(NULL, NULL, (void **)&pINetPass))
			throw gcnew System::Exception (L"�� ������� �������� ����� ��������� INetPass");

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
		//throw gcnew System::Exception (L"���������� �� �������");

	// ����������� � �����������
	ConnectAccessDevice();

	// ��������� �����������
	BYTE dataBuffer[1024 * 256];
	DWORD dataBufferLen = sizeof(dataBuffer);
	HRESULT hResult;
	DWORD dImageSize;
	if (ERROR_SUCCESS != (hResult = pIScannerDevice->GetImage((LPBYTE)dataBuffer, dataBufferLen, 
		&dImageSize, SCAN_TIMEOUT)))
		return String::Empty; 

	array<Byte>^ imageArray = gcnew array<Byte>(dImageSize);
	System::Runtime::InteropServices::Marshal::Copy((IntPtr)dataBuffer, imageArray, 0, dImageSize);

	// �������� �������� �����������
	Biolink::Biometrics2::Image ^image = gcnew Biolink::Biometrics2::Image(imageArray);
	if(image->ExpressQuality() >= MAX_QUALITY_THREASHOLD)
	{
		// ������ �������� "2" (����������� �������� ������������)
//		pIAccessDevice->SetEvent(2);

		// ������������ �������
		Biolink::Biometrics2::ImageSet ^imageSet = gcnew Biolink::Biometrics2::ImageSet();
		imageSet->AddImage(image, Biolink::Biometrics2::FingerCode::Unknown);
		Biolink::Biometrics2::ImageProcessor ^processor = gcnew Biolink::Biometrics2::ImageProcessor(Biolink::Biometrics2::MathType::BIOLINK);
		Biolink::Biometrics2::Template ^imageTemplate = processor->CreateTemplate(imageSet);
		return System::Convert::ToBase64String(imageTemplate->ToArray());
	}
	if(image->ExpressQuality() >= MIN_QUALITY_THREASHOLD)
		// ������ �������� "4" (������ �������������)
		pIAccessDevice->SetEvent(4);
	return String::Empty; 
}

bool TsBiolinkScanner::TsBiolinkScannerDevice::Open()
{
	if(!_active)
		throw gcnew System::Exception (L"���������� �� �������");

	ConnectAccessDevice();
	// ������ �������� "3" (�������� �����)
	return ERROR_SUCCESS == pIAccessDevice->SetEvent(3);
}

void TsBiolinkScanner::TsBiolinkScannerDevice::Close(bool accessDenied)
{
	if(!_active)
		throw gcnew System::Exception (L"���������� �� �������");
	ConnectAccessDevice();
	pIAccessDevice->SetEvent(5);
}

void TsBiolinkScanner::TsBiolinkScannerDevice::ConnectScannerDevice()
{
	// �������� ��������� ����������
	if(pIScannerDevice)
		return;

	// ��������� ������ ���������
	INetPassEnum *pIEnum = NULL;
	if(ERROR_SUCCESS == pINetPass->CreateNetPassEnum((void **)&pIEnum, CLSID_MatchBook,
		IID_INetPassEnum))
		// ��������� ���������� �� ��������������
		pIEnum->CreateDevice((void**) &pIScannerDevice, IID_INetPassDevice, Address);
	else
		throw gcnew System::Exception (L"������ ��� ��������� ���������� ��������������� �������");

	if (pIEnum)
		pIEnum->Release();
	if(!pIScannerDevice)
		throw gcnew System::Exception (L"������ � ���������� ��������������� �� ������");
}

void TsBiolinkScanner::TsBiolinkScannerDevice::ConnectAccessDevice()
{
	// ����������� � �������
	ConnectScannerDevice();

	// �������� ��������� ����������
	if(pIAccessDevice)
		return;
	
	if(ERROR_SUCCESS != pIScannerDevice->QueryInterface(IID_INetPassAccess, (void **)&pIAccessDevice))
		throw gcnew System::Exception (L"������ ��� ��������� ���������� ���������� ���������� ��������");

	// ��������� ���������� ����������
	DWORD dValue = 0;
	pIAccessDevice->WriteDeviceReg(4, &dValue, sizeof(DWORD), NULL); 
	dValue = _timeout * 1000;
	pIAccessDevice->WriteDeviceReg(16, &dValue, sizeof(DWORD), NULL);
}
