// TsBiolinkScanner.h
#include "BlNetpassUid.h"
#include "BlNetpassApi.h"

#pragma once

using namespace System;
using namespace DevicesCommon;
using namespace DevicesCommon::Helpers;
using namespace DevicesBase;

namespace TsBiolinkScanner {

	[TurnstileDevice("Biolink U-Match BI Eth")]
	public ref class TsBiolinkScannerDevice: CustomConnectableDevice, ITurnstileDevice
	{
	private:
		int _timeout;
		int _address;
		bool _active;
		TurnstileDirection _direction;
		HINSTANCE hApi;
		Biolink::Biometrics2::License ^_license;
		void ConnectScannerDevice();
		void ConnectAccessDevice();
	protected:
	public:
        //     Направление, в котором работает турникет
        virtual property TurnstileDirection Direction 
		{ 
			TurnstileDirection get() { return _direction; }
			void set(TurnstileDirection value) { _direction = value; }
		}
        //
        // Summary:
        //     Очередной блок идентификационных данных от устройства
		virtual property System::String ^IdentificationData
		{
			System::String^ get();
		}

        //
        // Summary:
        //     Таймаут открытия турникета
        virtual property int Timeout 
		{ 
			int get() { return _timeout; }
			void set(int value) { _timeout = value; }
		}

        virtual property int Baud
		{ 
			int get() { return 0; }
			void set(int value) { }
		}

        //
        // Summary:
        //     Адрес устройства
        virtual property int Address
		{ 
			int get() { return _address; }
			void set(int value) { _address = value; }
		}

		virtual property System::String^ PortName
		{ 
			System::String^ get() override { return System::String::Empty; }
			void set(System::String^ value) override { }
		}

		virtual property bool Active
		{ 
			bool get() override { return _active; }
			void set(bool value) override;
		}


        // Summary:
        //     Закрыть турникет
        // Parameters:
        //   accessDenied:
        //     Доступ запрещен
        virtual void Close(bool accessDenied);

        // Summary:
        //     Открыть турникет
        // Returns:
        //     true, если в течение таймаута через турникет совершен проход
        virtual bool Open();

		TsBiolinkScannerDevice();
		~TsBiolinkScannerDevice();
	};
}
