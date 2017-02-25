using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevicesBase;
using DevicesCommon;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace BiolinkLock
{
    [TurnstileDevice("tsBiolinkLockDevice")]
    public class BiolinkLockDevice : CustomSerialDevice, ITurnstileDevice
    {
        #region Поля

        private Biolink.Biometrics2.License _license;

        private Biolink.Biometrics2.Scanner _scanner;

        #endregion

        #region Свойства

        public string ScannerId { get; set; }

        public int MinQualityThreashold { get; set; }

        public int Timeout
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public TurnstileDirection Direction
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string IdentificationData
        {
            get 
            {
                if (_scanner != null)
                {
                    using (var image = _scanner.AcquireImage(Biolink.Biometrics2.DeviceMode.Default))
                    {
                        int quality = image.ExpressQuality();
                        if (quality < MinQualityThreashold)
                            return String.Empty;

                        using (var imageSet = new Biolink.Biometrics2.ImageSet())
                        {
                            imageSet.AddImage(image, Biolink.Biometrics2.FingerCode.Unknown);
                            using (var processor = new Biolink.Biometrics2.ImageProcessor(Biolink.Biometrics2.MathType.BIOLINK))
                            {
                                using (var template = processor.CreateTemplate(imageSet))
                                {
                                    return Convert.ToBase64String(template.ToArray());
                                }
                            }
                        }

                    }
                }
                return String.Empty;
            }
        }

        #endregion

        #region Конструктор

        public BiolinkLockDevice()
            : base()
        {

        }

        #endregion

        #region Переопределенные методы

        protected override void OnAfterActivate()
        {
            _license = new Biolink.Biometrics2.License(Biolink.Biometrics2.LicenseType.ALL);

            //получаем список устройств
            using (var devices = new Biolink.Biometrics2.DeviceList(Biolink.Biometrics2.DeviceType.UmatchScanner))
            {
                if (devices.Size == 0)
                    return;

                // пока что выбираем первый попавшийся сканер
                for (int i = 0; i < devices.Size; i++)
                    using (var descriptor = devices.DeviceDescriptor(i))
                    {
                        if (descriptor.InstanceId == ScannerId)
                            _scanner = new Biolink.Biometrics2.Scanner(descriptor);
                    }
            }

            if (_scanner == null)
            {
            }

            base.OnAfterActivate();
        }

        protected override void OnAfterDeactivate()
        {
            if (_scanner != null)
            {
                _scanner.Dispose();
                _scanner = null;
            }
            base.OnAfterDeactivate();
        }

        public override void Dispose()
        {
            if (_scanner != null)
            {
                _scanner.Dispose();
                _scanner = null;
            }
            if (_license != null)
            {
                _license.Close();
                _license = null;
            }
            base.Dispose();
        }

        #endregion

        #region Методы интерфейса

        public bool Open()
        {
            throw new NotImplementedException();
        }

        public void Close(bool accessDenied)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override string PortName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool Active
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Address
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
