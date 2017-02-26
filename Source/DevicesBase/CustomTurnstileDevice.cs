using System;
using System.Threading;
using DevicesCommon;

namespace DevicesBase
{
    /// <summary>
    /// ������� ����� ��� ��������� ���������� ����������
    /// </summary>
    public abstract class CustomTurnstileDevice : CustomRS485Device, ITurnstileDevice
    {
        #region ���������

        private const int FlashSleep = 80;
        private const int ShortSleep = 100;
        private const int PassWatchSleep = 10;
        private const int MaxZeroRead = 25;
        private const string OperationCancelled = "�������� �������� �����������. �����: {0}";

        #endregion

        #region ����

        private TurnstileDirection _direction;
        private object _syncObject;
        private int _timeout;
        
        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// �������� ������� �����������
        /// </summary>
        /// <param name="count">���������� ��������</param>
        /// <param name="beep">������������ �������� ��������</param>
        private void FlashRed(int count, bool beep)
        {
            // ����������� ��������� ������� 
            OnGreen(false);

            for (int i = 0; i < count; i++)
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
        private void FlashGreen(int count, bool beep)
        {
            // ����������� ��������� �������
            OnRed(false);

            for (int i = 0; i < count; i++)
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
        protected abstract byte TermChar { get; }

        /// <summary>
        /// ������ ����������������� ������
        /// </summary>
        /// <returns>������, �������������� ��� ���������</returns>
        protected abstract string OnReadIdData();

        /// <summary>
        /// ���������� ������� �����������
        /// </summary>
        /// <param name="flashOn">��������</param>
        protected abstract void OnRed(bool flashOn);

        /// <summary>
        /// ���������� ������� �����������
        /// </summary>
        /// <param name="flashOn">��������</param>
        protected abstract void OnGreen(bool flashOn);

        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="beepOn">��������</param>
        protected abstract void OnBeep(bool beepOn);

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
        protected abstract bool OnPassComplete();

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
        public int Timeout 
        {
            get { return _timeout; }
            set { _timeout = value; } 
        }

        /// <summary>
        /// ������� ��������
        /// </summary>
        /// <returns>true, ���� � ������� �������� ����� �������� �������� ������</returns>
        public bool Open()
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
                    bool passComplete = false;
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
        public void Close(bool accessDenied)
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
