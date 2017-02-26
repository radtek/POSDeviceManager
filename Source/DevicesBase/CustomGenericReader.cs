using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using DevicesCommon;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DevicesBase
{
	/// <summary>
    /// ������� ����� ��� ���������, ��������������� ��� ������ ������ ���� �������
	/// </summary>
	public abstract class CustomGenericReader : CustomSerialDevice, IGenericReader
    {
        #region ���� 

        private Parity _parity;
        private Byte _stopChar;
        private Queue<string> _data;
        private object _syncObject;
        private Thread _readerThread;
        private Boolean _terminated;
        private Byte[] _buffer;
        private StringBuilder _tempData;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        protected CustomGenericReader()
            : base()
        {
            _parity = Parity.None;
            _terminated = false;
            _syncObject = new object();
            _stopChar = 0x0A;
            _data = new Queue<string>();
            _readerThread = new Thread(ReadData);
            _buffer = new Byte[1024];
            _tempData = new StringBuilder();
        }

        #endregion

        #region �������� �������� � ������

        private Boolean Terminated
        {
            get
            {
                Boolean value;
                lock (_syncObject)
                {
                    value = _terminated;
                }
                return value;
            }
            set
            {
                lock (_syncObject)
                {
                    _terminated = value;
                }
            }
        }

        private void LogException(Exception e)
        {
            _tempData.Length = 0;
            Logger.WriteEntry(e.Message, EventLogEntryType.Error);
        }

        /// <summary>
        /// ���������� ���������� �� ������� ������
        /// </summary>
        /// <param name="rawData">"�����" ������</param>
        protected abstract string Prepare(string rawData);

        /// <summary>
        /// ����� ��������� ����������
        /// </summary>
        protected override void OnAfterActivate()
        {
            Port.Parity = _parity;
            Port.DsrFlow = false;
            Port.ReadTimeout = -1;
            Port.DiscardBuffers();

            // �������� ������ ������
            _readerThread.Start();
        }

        /// <summary>
        /// ����� ������������ ����������
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
            // ��������� ������, ��������� ������
            Terminated = true;
            _readerThread.Join(30000);
        }

        private void ReadData()
        {
            while (!Terminated)
            {
                try
                {
                    // ������ ���, ��� ���� �� �������� ������ �����
                    Array.Clear(_buffer, 0, _buffer.Length);
                    Int32 bytesRead = Port.Read(_buffer, 0, _buffer.Length);

                    if (bytesRead > 0)
                    {
                        // �������� ��������� ������ �� ��������� ������
                        for (Int32 i = 0; i < bytesRead; i++)
                        {
                            if (_buffer[i] == _stopChar)
                            {
                                // ��������� ��������� ������ ������
                                // ������ ������
                                string preparedData = Prepare(_tempData.ToString());
                                if (!string.IsNullOrEmpty(preparedData))
                                {
                                    lock (_syncObject)
                                    {
                                        // �������� � �������
                                        _data.Enqueue(preparedData);
                                    }
                                }

                                // ������� ��������� ��������� ������
                                _tempData.Length = 0;
                            }
                            else
                            {
                                // �� ��������� ��������� �������� ������ �������� �������
                                if (_buffer[i] > 0x20)
                                    _tempData.Append((Char)_buffer[i]);
                            }
                        }
                    }
                    else
                        Thread.Sleep(100);
                }
                catch (Win32Exception e)
                {
                    LogException(e);
                }
                catch (TimeoutException e)
                {
                    LogException(e);
                }
            }
        }

        #endregion

        #region ���������� ICardReader

        /// <summary>
        /// ��������
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// ����-������
        /// </summary>
        public Byte StopChar
        {
            get { return _stopChar; }
            set { _stopChar = value; }
        }

        /// <summary>
        /// ��������� ���� ������
        /// </summary>
        public string Data
        {
            get 
            {
                string nextData;
                lock (_syncObject)
                {
                    // ��������� ��������� ������ ������ �� �������
                    nextData = _data.Dequeue();
                }
                return nextData;
            }
        }

        /// <summary>
        /// ��������� �������
        /// </summary>
        public Boolean Empty
        {
            get
            {
                Boolean state;
                lock (_syncObject)
                {
                    state = _data.Count == 0;
                }
                return state;
            }
            set
            {
                // ������� ������� ������
                if (value)
                {
                    lock (_syncObject)
                    {
                        _data.Clear();
                    }
                }
            }
        }

        #endregion
    }
}
