using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using System.Collections;
using DevicesCommon.Helpers;
using DevicesBase.Helpers;

namespace DevicesBase
{
	/// <summary>
	/// ������� ����� ��� ���� ���������
	/// </summary>
	public abstract class CustomDevice : MarshalByRefObject, IDevice
	{
        #region �������� ���� ������

        // ������������� ����������
        private String deviceId;
        // ������� �������� ����� ������, ������������� ��� ��������� ������
        private Dictionary<Int16, String> specificErrors;
        // ��������� ��� ���������������� ������ ����������
        private ILogger logger;
        // ��������� ��� ������� � ���� ���������������� ������
        private ISerialPortsPool _serialPortsPool;
        // ��� ������
        private ErrorCode _errorCode;

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ��� ���������������� ������
        /// </summary>
        public ISerialPortsPool SerialPortsPool
        {
            get { return _serialPortsPool; }
            set { _serialPortsPool = value; }
        }

        #endregion

        #region �����������

        /// <summary>
		/// ������� ���������� ������ ����������
		/// </summary>
		protected CustomDevice()
		{
            deviceId = String.Empty;
            specificErrors = new Dictionary<Int16, String>();
            ErrorCode = new ServerErrorCode(this, GeneralError.Success);
		}

		#endregion

		#region �������� � ������, ��������� �� ��������

        /// <summary>
        /// ��������� �������� �������� ������ ��������� ������ � ����������� � �������
        /// �������� ����� ������
        /// </summary>
        /// <param name="specificCode">��� ������ ��������� ������ � �����������</param>
        /// <param name="specificDescription">�������� ���� ������ ��������� ������ � �����������</param>
        protected void AddSpecificError(Int16 specificCode, String specificDescription)
        {
            specificErrors.Add(specificCode, specificDescription);
        }

        /// <summary>
        /// ���������� �������� ���� ������ ��������� ������ � �����������
        /// </summary>
        /// <param name="specificCode">��� ������</param>
        /// <returns>�������� ���� ������ ��������� ������ � �����������</returns>
        protected String GetSpecificDescription(Int16 specificCode)
        {
            return specificErrors.ContainsKey(specificCode) ?
                specificErrors[specificCode] : String.Format("�� ������� �������� ��� ������ � ����� {0}", specificCode);
        }

		#endregion

		#region ���������� IDevice

		/// <summary>
		/// ����������� � ���������� ����������
		/// </summary>
		public abstract bool Active { get; set; }

		/// <summary>
		/// ������������� ����������
		/// </summary>
		public String DeviceId
		{
			get { return deviceId; }
			set { deviceId = value; }
		}

        /// <summary>
        /// ��� ������
        /// </summary>
        public ErrorCode ErrorCode
        {
            get { return _errorCode; }
            protected set 
            {
                _errorCode = new ErrorCode(value.Sender, value.Value, value.Description,
                    value.SpecificValue, value.SpecificDescription);
                //_errorCode = value; 
            }
        }

        /// <summary>
        /// ��������� ��� ���������������� ������ ����������
        /// </summary>
        public ILogger Logger 
        {
            get { return logger; }
            set { logger = value; }
        }

		#endregion
	}
}
