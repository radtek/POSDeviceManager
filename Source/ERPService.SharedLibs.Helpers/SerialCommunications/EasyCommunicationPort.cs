using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// ��������� ������� ������� ���������� �����
    /// </summary>
    public class CommStateEventArgs : EventArgs
    {
        private DCB _dcb;
        private bool _handled;

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        /// <param name="dcb">����������� ��������� �����</param>
        public CommStateEventArgs(DCB dcb)
            : base()
        {
            _dcb = dcb;
            _handled = false;
        }

        /// <summary>
        /// ����������� ��������� �����
        /// </summary>
        public DCB DCB
        {
            get
            {
                return _dcb;
            }
            set
            {
                _dcb = value;
            }
        }

        /// <summary>
        /// ������� ����������
        /// </summary>
        public bool Handled
        {
            get
            {
                return _handled;
            }
            set
            {
                _handled = value;
            }
        }
    }

    /// <summary>
    /// ���������������� ����
    /// </summary>
    public class EasyCommunicationPort : IDisposable
    {
        #region �������� ���� ������

        // ������ ����� �����
        private const string _portNameFormat = @"\\.\{0}";
        // ������� ������ �����
        private const uint _byteTimeout = 100;

        // ���������� ����� ��� ������/������ � ����
        private byte[] _operationBuffer = new byte[1024];
        private byte[] _smallBuffer = new byte[1];

        // ��������� �����
        private SafeFileHandle _handle;
        // ��� �����
        private string _portName;
        // ������������ �����
        private DCB _dcb;
        // �������� �����
        private COMMTIMEOUTS _timeouts;
        // ��������
        private uint _baudRate;
        // �������� 
        private Parity _parity;
        // �������� ���
        private StopBits _stopBits;
        // ��� ������
        private byte _dataBits;
        // �������� DTR/DSR
        private bool _dsrFlow;
        // ������� ������
        private uint _readTimeout;
        // ������� ������
        private uint _writeTimeout;
        // ������� ����������������� �����
        private bool _isSerial;
        // ������������ ��������� ��� ���������
        private bool _throwTimeoutExceptions;
        // ����� �� ������ �� ������������ ������
        private bool _canReadFromParallel;
        // ������ �������� ������ �����
        private uint _readBufferSize;
        // ������ ��������� ������ �����
        private uint _writeBufferSize;

        #endregion

        #region �����������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public EasyCommunicationPort()
        {
            _handle = null;
            _portName = "COM1";
            _isSerial = true;
            _baudRate = 9600;
            _parity = Parity.None;
            _stopBits = StopBits.One;
            _dataBits = 8;
            _dsrFlow = false;
            _readTimeout = 50;
            _writeTimeout = 2000;
            _throwTimeoutExceptions = true;
            _canReadFromParallel = false;
            _readBufferSize = 2048;
            _writeBufferSize = 2048;
        }

        #endregion

        #region �������� �������� � ������

        /// <summary>
        /// ���������, ������ ���� ��� ���
        /// </summary>
        private void CheckOpening()
        {
            if (!IsOpen)
                throw new Exception(string.Format("���� {0} �� ������", _portName));
        }

        /// <summary>
        /// ������������� ������� ������ �����
        /// </summary>
        /// <param name="timeoutValue"></param>
        private void SetReadTimeout(uint timeoutValue)
        {
            if (!IsSerial)
                return;
            _timeouts.ReadTotalTimeoutConstant = timeoutValue;
            WinApi.Win32Check(WinApi.SetCommTimeouts(_handle, ref _timeouts));
        }

        /// <summary>
        /// ����������� ���������� ��� �������� � ����������� �� ��������
        /// </summary>
        private void ThrowTimeoutException()
        {
            if (_throwTimeoutExceptions)
                throw new System.TimeoutException();
        }

        /// <summary>
        /// ������ ������� �������� � ��������� ������� �����
        /// </summary>
        private void SetupComm()
        {
            CheckOpening();
            WinApi.Win32Check(WinApi.SetupComm(_handle, _readBufferSize, _writeBufferSize));
        }

        /// <summary>
        /// �������� ������ �����
        /// </summary>
        /// <param name="input">�������� �������� �����</param>
        /// <param name="output">�������� ��������� �����</param>
        private void DiscardBuffers(bool input, bool output)
        {
            // ��������� ��������� �����
            CheckOpening();
            if (!IsSerial)
                // ������ ��� ���������������� ������
                return;

            // �������������� �������� ������
            PURGE_FLAGS flags = PURGE_FLAGS.PURGE_EMPTY;

            // ���� ������ �������� �������� �����
            if (input)
                // ������ ����� ��� ������� ��������� ������
                flags = flags | PURGE_FLAGS.PURGE_RXABORT | PURGE_FLAGS.PURGE_RXCLEAR;

            // ���� ������ �������� ��������� �����
            if (output)
                // ������ ����� ��� ������� ���������� ������
                flags = flags | PURGE_FLAGS.PURGE_TXABORT | PURGE_FLAGS.PURGE_TXCLEAR;

            bool purgeResult = WinApi.PurgeComm(_handle, (uint)flags);
            if (!purgeResult)
            {
                uint errors = 0;

                if (Marshal.GetLastWin32Error() == (uint)SystemErrorCodes.ERROR_OPERATION_ABORTED)
                    WinApi.ClearCommError(_handle, ref errors, IntPtr.Zero);
                else
                    WinApi.Win32Check(purgeResult);
            }
        }

        #endregion

        #region ��������

        /// <summary>
        /// ����� �� ��������� �������� ������ �� ������������ ������.
        /// ���� ���, �� ��� ������� ������ ����� ��������������
        /// </summary>
        public bool CanReadFromParallel
        {
            get
            {
                return _canReadFromParallel;
            }
            set
            {
                _canReadFromParallel = value;
            }
        }

        /// <summary>
        /// ������������ ���������� �� �������� ������ � �����������
        /// </summary>
        public bool ThrowTimeoutExceptions
        {
            get
            {
                return _throwTimeoutExceptions;
            }
            set
            {
                _throwTimeoutExceptions = value;
            }
        }

        /// <summary>
        /// ���������� ������� ������ �� ����������������� �����
        /// </summary>
        public bool IsSerial
        {
            get
            {
                return _isSerial;
            }
        }

        /// <summary>
        /// ���������� ������� �������� �����
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _handle != null && !_handle.IsInvalid;
            }
        }

        /// <summary>
        /// ��� �����
        /// </summary>
        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                Close();
                _portName = value;
                if (_portName.Length > 3)
                    // ��� ����� ������
                    _isSerial = string.Compare(_portName.Substring(0, 3), "COM", true) == 0;
                else
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// �������� ������ ������� ����� ����
        /// </summary>
        public int BaudRate
        {
            get
            {
                return (int)_baudRate;
            }
            set
            {
                _baudRate = (uint)value;
                if (_isSerial && IsOpen)
                {
                    _dcb.BaudRate = (uint)_baudRate;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// �������� ��������
        /// </summary>
        public Parity Parity
        {
            get
            {
                return _parity;
            }
            set
            {
                _parity = value;
                if (_isSerial && IsOpen)
                {
                    _dcb.Parity = (byte)_parity;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// �������� ���
        /// </summary>
        public StopBits StopBits
        {
            get
            {
                return _stopBits;
            }
            set
            {
                _stopBits = value;
                if (_isSerial && IsOpen)
                {
                    _dcb.StopBits = (byte)_stopBits;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// ��� ������
        /// </summary>
        public int DataBits
        {
            get
            {
                return _dataBits;
            }
            set
            {
                _dataBits = (byte)value;
                if (_isSerial && IsOpen)
                {
                    _dcb.ByteSize = _dataBits;
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// �������� �� ���������� ����� DSR
        /// </summary>
        public bool DsrFlow
        {
            get
            {
                return _dsrFlow;
            }
            set
            {
                _dsrFlow = value;
                if (_isSerial && IsOpen)
                {
                    _dcb.fOutxDsrFlow = (uint)(_dsrFlow ? 1 : 0);
                    WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));
                }
            }
        }

        /// <summary>
        /// ������� ������ ������
        /// </summary>
        public int ReadTimeout
        {
            get
            {
                return (int)_readTimeout;
            }
            set
            {
                CheckOpening();
                unchecked
                {
                    _readTimeout = (uint)value;
                }
                if (_isSerial)
                {
                    // �������� ������ ��������������� ������ ��� ����������������� �����
                    if (_readTimeout == uint.MaxValue)
                    {
                        // �������� ������ ����� ����������� ����������,
                        // ��������� ������, ��� ����������� � ������ �����
                        _timeouts.ReadIntervalTimeout = uint.MaxValue;
                        _timeouts.ReadTotalTimeoutMultiplier = 0;
                        _timeouts.ReadTotalTimeoutConstant = 0;
                    }
                    else
                    {
                        // �������� ������ ����� ���������, ���� ���� ��������� �������� 
                        // ���������� ����, ���� ������� ����� ��������
                        _timeouts.ReadIntervalTimeout = _byteTimeout;
                        _timeouts.ReadTotalTimeoutMultiplier = _byteTimeout;
                        _timeouts.ReadTotalTimeoutConstant = _readTimeout;
                    }
                    WinApi.Win32Check(WinApi.SetCommTimeouts(_handle, ref _timeouts));
                }
            }
        }

        /// <summary>
        /// ������� ������ ������
        /// </summary>
        public int WriteTimeout
        {
            get
            {
                return (int)_writeTimeout;
            }
            set
            {
                CheckOpening();
                _writeTimeout = (uint)value;
                // ������
                _timeouts.WriteTotalTimeoutMultiplier = 100;
                _timeouts.WriteTotalTimeoutConstant = _writeTimeout;
                WinApi.Win32Check(WinApi.SetCommTimeouts(_handle, ref _timeouts));
            }
        }

        /// <summary>
        /// �������� ��� ��������� ������ DTR
        /// </summary>
        public bool DtrEnable
        {
            set
            {
                CheckOpening();
                WinApi.Win32Check(WinApi.EscapeCommFunction(_handle,
                    (uint)(value ? ExtendedFunctions.SETDTR : ExtendedFunctions.CLRDTR)));
            }
        }

        /// <summary>
        /// �������� ��� ��������� ������ RTS
        /// </summary>
        public bool RtsEnable
        {
            set
            {
                CheckOpening();
                WinApi.Win32Check(WinApi.EscapeCommFunction(_handle,
                    (uint)(value ? ExtendedFunctions.SETRTS : ExtendedFunctions.CLRRTS)));
            }
        }

        /// <summary>
        /// ������������� ������ �������� ������ �����
        /// </summary>
        public int ReadBufferSize
        {
            set
            {
                _readBufferSize = (uint)value;
                SetupComm();
            }
        }

        /// <summary>
        /// ������������� ������ ��������� ������ �����
        /// </summary>
        public int WriteBufferSize
        {
            set
            {
                _writeBufferSize = (uint)value;
                SetupComm();
            }
        }

        #endregion

        #region ������

        /// <summary>
        /// �������� ��������� ������� �� ������� ������ �����
        /// </summary>
        /// <param name="customChar">������ ������������ �������������</param>
        public bool WaitChar(bool customChar)
        {
            uint mask = 0;
            WinApi.Win32Check(WinApi.SetCommMask(_handle,
                customChar ? (uint)CommEvents.EV_RXFLAG : (uint)CommEvents.EV_RXCHAR));
            try
            {
                WinApi.Win32Check(WinApi.WaitCommEvent(_handle, ref mask, IntPtr.Zero));
                if (customChar)
                    return (mask & (uint)CommEvents.EV_RXFLAG) == (uint)CommEvents.EV_RXFLAG;
                else
                    return (mask & (uint)CommEvents.EV_RXCHAR) == (uint)CommEvents.EV_RXCHAR;
            }
            finally
            {
                WinApi.Win32Check(WinApi.SetCommMask(_handle, (uint)CommEvents.EV_TXEMPTY));
            }
        }

        /// <summary>
        /// ��������� ��������������� ����
        /// </summary>
        public void Open()
        {
            // ���� ���� ������
            if (IsOpen)
                // ��������� ���
                Close();

            // ��������� ����
            _handle = WinApi.CreateFile(
                string.Format(_portNameFormat, _portName),
                (uint)(DESIRED_ACCESS.GENERIC_READ | DESIRED_ACCESS.GENERIC_WRITE),
                0, 
                IntPtr.Zero, 
                CREATION_DISPOSITION.OPEN_EXISTING, 
                0, 
                IntPtr.Zero);

            // ��������� ��������� ��������
            WinApi.Win32Check(!_handle.IsInvalid);

            if (IsSerial)
            {
                // ������ ��������� �����
                WinApi.Win32Check(WinApi.GetCommState(_handle, ref _dcb));

                // �������������� ��������� ���������� �� ���������
                _dcb.BaudRate = _baudRate;
                _dcb.ByteSize = _dataBits;
                _dcb.fOutxDsrFlow = _dsrFlow ? 1U : 0U;
                _dcb.Parity = (byte)_parity;
                _dcb.StopBits = (byte)_stopBits;

                // ���� ���� ���������� �������
                if (SetCommStateEvent != null)
                {
                    CommStateEventArgs e = new CommStateEventArgs(_dcb);
                    SetCommStateEvent(this, e);
                    if (e.Handled)
                        _dcb = e.DCB;
                }

                // ������ ���������
                WinApi.Win32Check(WinApi.SetCommState(_handle, ref _dcb));

                // ������������� ����� �������
                WinApi.SetCommMask(_handle, (uint)CommEvents.EV_TXEMPTY);
            }
        }

        /// <summary>
        /// ��������� ���������������� ����
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// ������ ������ �� �����
        /// </summary>
        /// <param name="buffer">����� ��� ���������� ���������� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="count">������� ���������</param>
        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "�� ����� ����� ������ ��� ������ �� �����");
            if (count < 1)
                throw new ArgumentOutOfRangeException("count", count, "����� ������ ��� ������ ������� ����");

            CheckOpening();
            if (!IsSerial && !_canReadFromParallel)
            {
                // ���� ������������ ���� � ����� ������� ������� 
                // �� ������ ��� ������������ ������
                Array.Clear(buffer, offset, count);
                return count;
            }

            uint totalRead = 0, bytesRead;

            // ������� ������ ��� �������� ������
            Array.Clear(buffer, offset, count);
            // �������� �����
            DateTime startTime = DateTime.Now;
            do
            {
                // ������� ��������� �����
                Array.Clear(_operationBuffer, 0, _operationBuffer.Length);
                uint recentDataSize = (uint)count - totalRead;
                bytesRead = 0;

                // ������ ������
                WinApi.Win32Check(WinApi.ReadFile(
                    _handle,
                    _operationBuffer,
                    recentDataSize >
                        (uint)_operationBuffer.Length ? (uint)_operationBuffer.Length : recentDataSize,
                    ref bytesRead,
                    IntPtr.Zero));

                // ���� ���-�� ���������
                if (bytesRead > 0)
                {
                    // �������� � �������� �����
                    Array.Copy(_operationBuffer, 0, buffer, offset + totalRead, bytesRead);
                    // ����������� ����� ����������� ����
                    totalRead += bytesRead;
                }

                if (_readTimeout != uint.MaxValue)
                {
                    TimeSpan ellapsed = DateTime.Now.Subtract(startTime);
                    if (Math.Abs(_readTimeout - ellapsed.TotalMilliseconds) < _byteTimeout)
                        // ���� ������� ����� ��������� �������� � ��������� �������� ��������
                        // ������ �������� ������ �����
                        ThrowTimeoutException();
                }
            }
            while (bytesRead > 0 && totalRead < count);
            if (totalRead > 0)
                return (int)totalRead;
            else
            {
                if (_readTimeout != uint.MaxValue)
                    ThrowTimeoutException();
                return -1;
            }
        }

        /// <summary>
        /// ������ ���� ���� �� ��������� ������ �����
        /// </summary>
        public int ReadByte()
        {
            uint bytesRead = 0;
            WinApi.Win32Check(WinApi.ReadFile(_handle, _smallBuffer, 1, ref bytesRead, 
                IntPtr.Zero));

            if (bytesRead > 0)
                return (int)_smallBuffer[0];
            else
            {
                ThrowTimeoutException();
                return -1;
            }
        }

        /// <summary>
        /// ������ ������ � ����. ������� ���� ����� ������� � ������� ���������
        /// </summary>
        /// <param name="buffer">����� ������ ��� ������</param>
        public int Write(byte[] buffer)
        {
            return Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// ������ ������ � ����
        /// </summary>
        /// <param name="buffer">����� ������ ��� ������</param>
        /// <param name="offset">�������� �� ������ ������</param>
        /// <param name="count">������� ��������</param>
        public int Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "�� ����� ����� ������ ��� ������ � ����");
            if (count < 1)
                throw new ArgumentOutOfRangeException("count", count, "����� ������������ ������ ������� ����");

            uint totalWritten = 0, bytesWritten;
            CheckOpening();
            do
            {
                uint
                    recentDataSize = (uint)count - totalWritten,
                    bytesToWrite = recentDataSize > (uint)_operationBuffer.Length ? (uint)_operationBuffer.Length : recentDataSize;
                bytesWritten = 0;

                // �������� ����� ������ �� ��������� ������ �� ���������
                Array.Clear(_operationBuffer, 0, _operationBuffer.Length);
                Array.Copy(buffer, offset + totalWritten, _operationBuffer, 0, bytesToWrite);

                // ���������� ������
                WinApi.Win32Check(WinApi.WriteFile(_handle, _operationBuffer, bytesToWrite,
                    ref bytesWritten, IntPtr.Zero));

                // ���� ���-�� ��������
                if (bytesWritten > 0)
                    // ����������� ����� ����������� ����
                    totalWritten += bytesWritten;
            }
            while (bytesWritten > 0 && totalWritten < count);
            if (totalWritten > 0)
                return (int)totalWritten;
            else
            {
                ThrowTimeoutException();
                return -1;
            }
        }

        /// <summary>
        /// ������ ������ ����� � ����
        /// </summary>
        /// <param name="byteToWrite">���� ��� ������</param>
        public void WriteByte(int byteToWrite)
        {
            _smallBuffer[0] = (byte)byteToWrite;
            int bytesWritten = Write(_smallBuffer, 0, 1);
            if (bytesWritten == 0)
                ThrowTimeoutException();
        }

        /// <summary>
        /// �������� �������� ����� � ���������� ����� ������
        /// </summary>
        public void DiscardInBuffer()
        {
            DiscardBuffers(true, false);
        }

        /// <summary>
        /// �������� ��������� ����� � ���������� ������ ������
        /// </summary>
        public void DiscardOutBuffer()
        {
            DiscardBuffers(false, true);
        }

        /// <summary>
        /// �������� ��� ������ �����, �������� ��� �������� �����/������
        /// </summary>
        public void DiscardBuffers()
        {
            DiscardBuffers(true, true);
        }

        /// <summary>
        /// ����� ������ ����� � ������ ������ � ����������
        /// </summary>
        public void Flush()
        {
            WinApi.Win32Check(WinApi.FlushFileBuffers(_handle));
        }

        /// <summary>
        /// ����� ����� ������ ����������
        /// </summary>
        public void ClearError()
        {
            uint errors = 0;
            WinApi.Win32Check(WinApi.ClearCommError(_handle, ref errors, IntPtr.Zero));
        }

        #endregion

        #region �������

        /// <summary>
        /// ������� ������� ���������� ����� (�����������)
        /// </summary>
        public event EventHandler<CommStateEventArgs> SetCommStateEvent;

        #endregion

        #region ���������� IDisposable

        /// <summary>
        /// ����������� �������, ������������ �����������
        /// </summary>
        public void Dispose()
        {
            if (_handle != null)
            {
                // ��������� ����
                _handle.Dispose();
                _handle = null;
            }
        }

        #endregion
    }
}
