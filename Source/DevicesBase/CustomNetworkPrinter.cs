using System;
using System.Collections.Generic;
using System.Text;
using DevicesBase.Communicators;
using DevicesCommon.Helpers;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ���������� ���������, ������������ �� TCP
    /// </summary>
    public abstract class CustomNetworkPrinter : CustomPrintableDevice
    {
        #region ����

        private TcpCommunicator _communicator;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomNetworkPrinter()
            : base()
        {
        }

        #endregion

        #region �������� ������

        private TcpCommunicator CreateCommunicator()
        {
            return new TcpCommunicator(PortName, TcpPort)
            {
                ReadTimeout = this.ReadTimeout,
                WriteTimeout = this.WriteTimeout
            };
        }

        #endregion

        #region ���������� ������� ������� ������� ��� ���������� ������ � ����������������� �������

        /// <summary>
        /// ������ �� ����������������� �����
        /// </summary>
        protected override Boolean IsSerial
        {
            get { return false; }
        }

        /// <summary>
        /// ���������������� ����
        /// </summary>
        protected override EasyCommunicationPort Port
        {
            get
            {
                throw new InvalidOperationException("��� ���������� ������������� ��� ������ �� TCP/IP");
            }
        }

        /// <summary>
        /// ������������ ��������
        /// </summary>
        public override void Dispose()
        {
            if (_communicator != null)
            {
                _communicator.Dispose();
                _communicator = null;
            }
        }

        /// <summary>
        /// ���������� ����� ���������� ����������
        /// </summary>
        protected override void OnBeforeActivate()
        {
        }

        /// <summary>
        /// ���������� ����� ��������� ����������
        /// </summary>
        protected override void OnAfterActivate()
        {
        }

        /// <summary>
        /// ���������� ����� ������������ ����������
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
        }

        /// <summary>
        /// ���������� ����� ����������� ����������
        /// </summary>
        protected override void OnAfterDeactivate()
        {
        }

        /// <summary>
        /// ��������� ����������
        /// </summary>
        public override Boolean Active
        {
            get 
            {
                // ���������� ������ �������
                return true; 
            } 
            set
            {
                if (value)
                {
                    OnBeforeActivate();
                    OnAfterActivate();
                }
                else
                {
                    OnBeforeDeactivate();
                    Dispose();
                    OnAfterDeactivate();
                }
            }
        }

        /// <summary>
        /// ������ ���������
        /// </summary>
        /// <param name="xmlData">������ ���������</param>
        public override void Print(string xmlData)
        {
            try
            {
                using (_communicator = CreateCommunicator())
                {
                    _communicator.Open();
                    base.Print(xmlData);
                }
            }
            finally
            {
                _communicator = null;
            }
        }

        /// <summary>
        /// ��������� ����������� ����������
        /// </summary>
        public override PrinterStatusFlags PrinterStatus
        {
            get
            {
                if (_communicator != null)
                {
                    return OnQueryPrinterStatus(_communicator);
                }
                else
                {
                    using (var communicator = CreateCommunicator())
                    {
                        communicator.Open();
                        return OnQueryPrinterStatus(communicator);
                    }
                }
            }
        }

        #endregion

        #region ��� ������ � TCP-���������

        /// <summary>
        /// TCP-���� ��������
        /// </summary>
        protected abstract Int32 TcpPort
        {
            get;
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected abstract Int32 ReadTimeout
        {
            get;
        }

        /// <summary>
        /// ������� ������
        /// </summary>
        protected abstract Int32 WriteTimeout
        {
            get;
        }

        /// <summary>
        /// ������ ������� ��������
        /// </summary>
        /// <param name="communicator">������������</param>
        protected abstract PrinterStatusFlags OnQueryPrinterStatus(TcpCommunicator communicator);

        /// <summary>
        /// ������������ ��� ������/������ ������ � ���� ��������
        /// </summary>
        protected TcpCommunicator Communicator
        {
            get { return _communicator; }
        }

        #endregion
    }
}
