using System;
using System.ComponentModel;
using DevicesBase.Helpers;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ���� ���������, ������������ ����� ���������������� � 
    /// ������������ �����
    /// </summary>
    public abstract class CustomSerialDevice : CustomConnectableDevice, 
        ISerialDevice, IDisposable
	{
        private string _portName;
        private int _baudRate;
        private bool _portCaptured;
        private bool _portOpened;
        private bool _portActivated;
        private bool _blockPortGetterReentrancy;
        private bool _disposed;

        #region �������� ������

        private EasyCommunicationPort CaptureAndGetPort()
        {
            EasyCommunicationPort port;

            if (_portCaptured)
            {
                port = SerialPortsPool.GetPort(DeviceId, _portName);
            }
            else
            {
                port = SerialPortsPool.CapturePort(DeviceId, _portName, false, TimeSpan.MinValue);
                _portCaptured = true;
            }

            return port;
        }

        private void OpenPort(EasyCommunicationPort port)
        {
            if (_portOpened)
                return;

            port.SetCommStateEvent += SetCommStateEventHandler;
            try
            {
                // �������������� �������� �� �������� �����
                OnBeforeActivate();

                // ��������� ���� �� �������� ��������
                port.BaudRate = _baudRate;
                port.Open();

                _portOpened = true;
            }
            catch (Win32Exception e)
            {
                // ������������� ��� ������
                ErrorCode = new ServerErrorCode(this, e);
                throw;
            }
            finally
            {
                port.SetCommStateEvent -= SetCommStateEventHandler;
            }
        }

        private void ActivatePort(EasyCommunicationPort port)
        {
            if (_portActivated)
                return;

            try
            {
                // �������������� �������� ����� �������� �����
                OnAfterActivate();
                _portActivated = true;
            }
            catch (Win32Exception e)
            {
                ErrorCode = new ServerErrorCode(this, e);
                throw;
            }
        }

        #endregion

        #region ���������� �������� � ������

        /// <summary>
		/// ������� ����������, ������������ � ����������������� �����
		/// </summary>
		protected CustomSerialDevice() : base()
		{
            _portName = "COM1";
            _baudRate = 9600;
		}

        /// <summary>
        /// ���������� ������� ������ �� ����������������� �����
        /// </summary>
        protected virtual bool IsSerial
        {
            get { return Port.IsSerial; }
        }

        /// <summary>
        /// ���������������� ����
        /// </summary>
        protected virtual EasyCommunicationPort Port
        {
            get 
            {
                var port = CaptureAndGetPort();

                if (!_blockPortGetterReentrancy)
                {
                    // ����������� ����������������� ����� �����,
                    // ������� �������� ��-�� ��������� � �������� Port �� �������
                    // OnBeforeActivate � OnAfterActivate

                    _blockPortGetterReentrancy = true;
                    try
                    {
                        OpenPort(port);
                        ActivatePort(port);
                    }
                    finally
                    {
                        _blockPortGetterReentrancy = false;
                    }
                }

                return port;
            }
        }

		#endregion

		#region ������, ����������� � �������-��������

		/// <summary>
		/// ���������� ����� ���������� ����������
		/// </summary>
		protected virtual void OnBeforeActivate()
		{
		}

		/// <summary>
		/// ���������� ����� ��������� ����������
		/// </summary>
		protected virtual void OnAfterActivate()
		{
		}

        /// <summary>
        /// ���������� ����� ������������ ����������
        /// </summary>
        protected virtual void OnBeforeDeactivate()
        {
        }

        /// <summary>
        /// ���������� ����� ����������� ����������
        /// </summary>
        protected virtual void OnAfterDeactivate()
        {
        }

        /// <summary>
        /// ���������� ������� ����������� ��������� ���������� �����
        /// </summary>
        /// <param name="sender">���������������� ����</param>
        /// <param name="e">��������� �������</param>
        protected virtual void SetCommStateEventHandler(object sender, CommStateEventArgs e)
        {
        }

		#endregion

		#region ���������� ISerialDevice

		/// <summary>
		/// �������� �������� ������ ����� ����
		/// </summary>
		public int Baud
		{
			get { return _baudRate; }
			set { _baudRate = value; }
		}

		/// <summary>
		/// ��� ����� (����., COM1, LPT1...)
		/// </summary>
		public override string PortName
		{
			get { return _portName; }
            set { _portName = value; } 
		}

		/// <summary>
		/// ����������� � ���������� ����������
		/// </summary>
        /// <remarks>
        /// ����������� ��������� ������� �������. ��. <see cref="CustomSerialDevice.Port"/>.
        /// </remarks>
		public override bool Active
		{
            get 
            {
                // ������ ������ ������� ���������� ��������,
                // �.�. �������� ����� ������� �������
                return true;
            }
            set
			{
                // ������ ����������;
                // ���� ����� ����������� �����
			}
		}

        #endregion

        #region ���������� IDisposable

        /// <summary>
		/// ������������ ��������
		/// </summary>
		public virtual void Dispose()
		{
            if (_disposed)
                return;

            if (_portCaptured)
            {
                // �������������� �������� �� ������������ �����
                if (_portActivated)
                {
                    OnBeforeDeactivate();
                    _portActivated = false;
                }

                // ������������ ����
                SerialPortsPool.ReleasePort(DeviceId, _portName);
                _portCaptured = false;
                
                // �������������� �������� ����� ������������ �����
                OnAfterDeactivate();
            }

            _disposed = true;
        }

		#endregion
    }
}
