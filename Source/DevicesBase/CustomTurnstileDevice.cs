using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ��������� ���������� ����������
    /// </summary>
    public abstract class CustomTurnstileDevice : CustomRS485Device, ITurnstileDevice
    {
        #region ���������

        private const Int32 FlashSleep = 80;
        private const Int32 ShortSleep = 100;
        private const Int32 PassWatchSleep = 10;
        private const Int32 MaxZeroRead = 25;
        private const string OperationCancelled = "�������� �������� �����������. �����: {0}";

        #endregion

        #region ����

        private TurnstileDirection _direction;
        private object _syncObject;
        private Int32 _timeout;
        
        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// �������� ������� �����������
        /// </summary>
        /// <param name="count">���������� ��������</param>
        /// <param name="beep">������������ �������� ��������</param>
        private void FlashRed(Int32 count, Boolean beep)
        {
            // ����������� ��������� ������� 
            OnGreen(false);

            for (Int32 i = 0; i < count; i++)
            {
                OnRed(true);
                if (beep)
                    OnBeep(true);
                Thread.Sleep(FlashSleep);
                if (beep)
                    OnBeep(false);
                OnRed(false);
            }
        }

        /// <summary>
        /// �������� ������� �����������
        /// </summary>
        /// <param name="count">���������� ��������</param>
        /// <param name="beep">������������ �������� ��������</param>
        private void FlashGreen(Int32 count, Boolean beep)
        {
            // ����������� ��������� �������
            OnRed(false);

            for (Int32 i = 0; i < count; i++)
            {
                OnGreen(true);
                if (beep)
                    OnBeep(true);
                Thread.Sleep(FlashSleep);
                if (beep)
                    OnBeep(false);
                OnGreen(false);
            }
        }

        #endregion

        #region �������� � ������, ����������� � ��������

        /// <summary>
        /// ����-������
        /// </summary>
        protected abstract Byte TermChar { get; }

        /// <summary>
        /// ������ ����������������� ������
        /// </summary>
        /// <returns>������, �������������� ��� ���������</returns>
        protected abstract string OnReadIdData();

        /// <summary>
        /// ���������� ������� �����������
        /// </summary>
        /// <param name="flashOn">��������</param>
        protected abstract void OnRed(Boolean flashOn);

        /// <summary>
        /// ���������� ������� �����������
        /// </summary>
        /// <param name="flashOn">��������</param>
        protected abstract void OnGreen(Boolean flashOn);

        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="beepOn">��������</param>
        protected abstract void OnBeep(Boolean beepOn);

        /// <summary>
        /// �������� ������� �������� ��� ���������
        /// </summary>
        protected abstract void OnOpen();

        /// <summary>
        /// �������� ������� �������� ��� ���������
        /// </summary>
        protected abstract void OnClose();

        /// <summary>
        /// �������� ����� ���������� ������� ����� ��������
        /// </summary>
        /// <returns>true, ���� ��� �������� ������</returns>
        protected abstract Boolean OnPassComplete();

        #endregion

        #region ���������� ������� �������� ������

        /// <summary>
        /// �������� ����� ��������� ����������
        /// </summary>
        protected override void OnAfterActivate()
        {
            base.OnAfterActivate();
            FlashRed(3, true);
            Close(false);
        }

        /// <summary>
        /// �������� ����� ������������ ����������
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
            FlashRed(3, true);
            Close(false);
            base.OnBeforeDeactivate();
        }

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomTurnstileDevice()
            : base()
        {
            _direction = TurnstileDirection.Entry;
            _syncObject = new object();
            _timeout = 15;
        }

        #endregion

        #region ���������� ITurnstileDevice Members

        /// <summary>
        /// �����������, � ������� �������� ��������
        /// </summary>
        public TurnstileDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        /// <summary>
        /// ������� �������� ���������
        /// </summary>
        public Int32 Timeout 
        {
            get { return _timeout; }
            set { _timeout = value; } 
        }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <returns>true, ���� � ������� �������� ����� �������� �������� ������</returns>
        public Boolean Open()
        {
            TimeSpan passTimeOut = new TimeSpan(0, 0, _timeout);
            lock (_syncObject)
            {
                // �������� ������������
                FlashGreen(2, true);
                OnGreen(true);
                // ��������� ��������
                OnOpen();

                try
                {
                    // �������� �����
                    DateTime fixedDt = DateTime.Now;

                    // ������� ���� ������� ����� ����
                    Boolean passComplete = false;
                    do
                    {
                        passComplete = OnPassComplete();
                        if (!passComplete)
                            Thread.Sleep(PassWatchSleep);
                    }
                    while (!passComplete && (DateTime.Now - fixedDt) < passTimeOut);
                    return passComplete;
                }
                finally
                {
                    // ��������� ��������, ���������� �� ����, �������� ������ ��� ���
                    FlashGreen(2, true);
                    Close(false);
                }
            }
        }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <param name="accessDenied">������ ��������</param>
        public void Close(Boolean accessDenied)
        {
            lock (_syncObject)
            {
                // ��������� ��������
                if (accessDenied)
                {
                    // �������� ��������, ������ ��� ������ ��������
                    FlashRed(3, true);
                }
                OnRed(true);
                OnClose();
            }
        }

        /// <summary>
        /// ��������� ���� ����������������� ������ �� ����������
        /// </summary>
        public string IdentificationData 
        { 
            get
            {
                string data = string.Empty;
                lock (_syncObject)
                {
                    data = OnReadIdData();
                    if (!string.IsNullOrEmpty(data))
                    {
                        FlashRed(1, true);
                        OnRed(true);
                    }
                }
                return data;
            }
        }

        #endregion
    }
}
