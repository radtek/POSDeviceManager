using System;
using System.Collections.Generic;
using System.Text;
using DevicesCommon;
using DevicesCommon.Helpers;
using DevicesBase;
using DevicesBase.Communicators;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using ERPService.SharedLibs.Helpers.SerialCommunications;

namespace DigiSimple
{
    /// <summary>
    /// ���������� ��������� ����� DIGI (����������, ����������� ��������)
    /// </summary>
    [Scale(DeviceNames.digiSimpleScales)]
    public sealed class DigiScales : CustomSerialDevice, IScaleDevice
    {
        private string _connStr;
        private Thread _worker;
        private int _weight;
        private Object _syncObject;
        private bool _terminated;
        private byte[] _rawData;
        private int _rawPos;
        private byte[] _strData;
        private int _timeoutsCount;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public DigiScales()
        {
            // �������������� �������� ����
            _weight = -1;
            // ������� ������� �����
            _worker = new Thread(WorkerRoutine);
            // ������� ������ ��� ������������� �������
            _syncObject = new Object();
            // ���� ��� ���������� �������� ������
            _terminated = true;
        }

        /// <summary>
        /// ���� ���������� ������
        /// </summary>
        private bool Terminated
        {
            get
            {
                bool t;
                lock (_syncObject)
                {
                    t = _terminated;
                }
                return t;
            }
            set
            {
                lock (_syncObject)
                {
                    _terminated = value;
                }
            }
        }

        /// <summary>
        /// ������ "�����" ������ �� �������
        /// </summary>
        private void ParseRawData()
        {
            // ���� ��������� 23 ��� 32 �����
            // �� ��������� �������� ������������������
            //
            // 23 ����� - ���� �� ���������� ��� ����
            // 32 ����� - ���� ��� ���� ����������
            if (_rawPos == 23 || _rawPos == 32)
            {
                // ������� �� "�����" ������ ������
                // � ������� ���� ��� ��������� �����
                int j = 0;
                for (int i = 6; i < 13; i++)
                {
                    if (_rawData[i] != 0x2E)
                    {
                        _strData[j] = _rawData[i];
                        j++;
                    }
                }

                // ������ ����������� � �����
                Weight = Convert.ToInt32(Encoding.Default.GetString(_strData));
            }

            //��������� � ������ �������
            _rawPos = 0;
        }

        /// <summary>
        /// �����, ����������� ������� �����
        /// </summary>
        private void WorkerRoutine()
        {
            byte[] opData = new byte[1024];
            _strData = new byte[6];
            _rawData = new byte[50];
            _rawPos = 0;
            _timeoutsCount = 0;

            while (!Terminated)
            {
                try
                {
                    // ������ ���, ��� ���� � ������ �����
                    int bytesRead = Port.Read(opData, 0, opData.Length);
                    if (bytesRead == -1)
                    {
                        // ������ �� ���������
                        // ������������������ ��� ��������� ����������
                        Thread.Sleep(50);
                        
                        if (_timeoutsCount < int.MaxValue)
                            // ����������� ������� �������� ������,
                            // ������������� ���������
                            _timeoutsCount++;

                        if (_timeoutsCount > 5)
                            // ���� ����� ����� �������� ��������� 2,
                            // �������, ��� ��� ����� � ������
                            Weight = -1;
                    }
                    else
                    {
                        // ���������� ������� ���������
                        _timeoutsCount = 0;

                        // ��������� ���������� ������ �� ������������������,
                        // ��������������� �������� LF
                        for (int i = 0; i < bytesRead; i++)
                        {
                            // ������� ������ ����� LF
                            if (opData[i] == 0x0A)
                                // ��������� "�����" ������, ���������� � �������
                                ParseRawData();
                            else
                            {
                                // �������� ��������� ���� � ������� "����� ������"
                                _rawData[_rawPos] = opData[i];
                                _rawPos++;

                                // ���� ������� ���������, ������������ � �� ������
                                if (_rawPos == _rawData.Length)
                                    _rawPos = 0;
                            }
                        }
                    }
                }
                catch (Win32Exception e)
                {
                    // ���������� ���� ������
                    Port.ClearError();
                    // ������� ������ �����
                    Port.DiscardBuffers();
                    // ������������ � ������ �������
                    _rawPos = 0;

                    // ������������� ��������� �� ������
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("������ �� ����� ������ ������� � ������.");
                    sb.AppendLine(string.Format("���: {0}", e.GetType().Name));
                    sb.AppendLine(string.Format("�����: {0}", e.Message));
                    sb.AppendLine("����������� �����:");
                    sb.Append(e.StackTrace);

                    Logger.WriteEntry(sb.ToString(), EventLogEntryType.Error);
                }
            }
        }

        #region ���������� ������� �������� ������

        /// <summary>
        /// ����� ��������� ����������
        /// </summary>
        protected override void OnAfterActivate()
        {
            // ��������� �����
            Port.Parity = Parity.None;
            Port.DataBits = 8;
            Port.StopBits = StopBits.One;

            // ������� ������
            //Port.ReadTimeout = 5000;
            Port.ReadTimeout = -1;

            // ���������� ���� ����������
            Terminated = false;
            // ��������� ������� �����
            _worker.Start();
        }

        /// <summary>
        /// ����� ������������ ����������
        /// </summary>
        protected override void OnBeforeDeactivate()
        {
            lock (_syncObject)
            {
                // ������������� ���� ����������
                Terminated = true;
            }

            // ������� ���������� ������ ������
            _worker.Join();
        }

        #endregion

        #region ���������� IScaleDevice

        /// <summary>
        /// ������ ����������� � �����
        /// </summary>
        public string ConnectionString
        {
            get { return _connStr; }
            set
            {
                _connStr = value;
                
                // ��������� ������ �����������
                ConnStrHelper connStrHelper = new ConnStrHelper(_connStr);

                // �������������� ����� ������ �� RS-232
                if (string.Compare(connStrHelper[1], "rs", true) != 0)
                    throw new InvalidOperationException("���� ������������ ����� ������ �� ���������� RS-232");

                // �������������� ��������� �����
                PortName = connStrHelper[2];
                Baud = Convert.ToInt32(connStrHelper[3]);
            }
        }

        /// <summary>
        /// �������� ������ � ����
        /// </summary>
        /// <param name="xmlData">������ ��� ��������</param>
        public void Upload(string xmlData)
        {
            // �������� �� �������������� ������
        }

        /// <summary>
        /// ������� ��������� ����
        /// </summary>
        public int Weight
        {
            get 
            {
                int tmp;
                lock (_syncObject)
                {
                    tmp = _weight;
                }
                return tmp;
            }
            private set
            {
                lock (_syncObject)
                {
                    _weight = value;
                }
            }
        }

        #endregion
    }
}
