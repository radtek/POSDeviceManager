using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.Win32.SafeHandles;

namespace ERPService.SharedLibs.Helpers.SerialCommunications
{
    /// <summary>
    /// Права на доступ к файлу
    /// </summary>
    internal enum DESIRED_ACCESS : uint
    {
        /// <summary>
        /// Чтение
        /// </summary>
        GENERIC_READ = 0x80000000,

        /// <summary>
        /// Запись
        /// </summary>
        GENERIC_WRITE = 0x40000000,

        /// <summary>
        /// Выполнение
        /// </summary>
        GENERIC_EXECUTE = 0x20000000,

        /// <summary>
        /// Полный доступ
        /// </summary>
        GENERIC_ALL = 0x10000000,
    };

    /// <summary>
    /// Режим разделения доступа к файлу
    /// </summary>
    internal enum SHARE_MODE : uint
    {
        /// <summary>
        /// Совместное чтение из фала
        /// </summary>
        FILE_SHARE_READ = 0x00000001,

        /// <summary>
        /// Совместная запись в файл
        /// </summary>
        FILE_SHARE_WRITE = 0x00000002,

        /// <summary>
        /// Удаление файла
        /// </summary>
        FILE_SHARE_DELETE = 0x00000004
    };

    /// <summary>
    /// Флаги и атрибуты для доступа к файлу
    /// </summary>
    internal enum FLAGSANDATTRIBUTES : uint
    {
        /// <summary>
        /// Только для чтения
        /// </summary>
        FILE_ATTRIBUTE_READONLY = 0x00000001,

        /// <summary>
        /// Скрытый
        /// </summary>
        FILE_ATTRIBUTE_HIDDEN = 0x00000002,

        /// <summary>
        /// Системный
        /// </summary>
        FILE_ATTRIBUTE_SYSTEM = 0x00000004,

        /// <summary>
        /// Папка
        /// </summary>
        FILE_ATTRIBUTE_DIRECTORY = 0x00000010,

        /// <summary>
        /// Архивный
        /// </summary>
        FILE_ATTRIBUTE_ARCHIVE = 0x00000020,

        /// <summary>
        /// Зашифрованный
        /// </summary>
        FILE_ATTRIBUTE_ENCRYPTED = 0x00000040,

        /// <summary>
        /// Нормальный
        /// </summary>
        FILE_ATTRIBUTE_NORMAL = 0x00000080,

        /// <summary>
        /// Временный
        /// </summary>
        FILE_ATTRIBUTE_TEMPORARY = 0x00000100,

        /// <summary>
        /// Unknown
        /// </summary>
        FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,

        /// <summary>
        /// Unknown
        /// </summary>
        FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,

        /// <summary>
        /// Сжатый 
        /// </summary>
        FILE_ATTRIBUTE_COMPRESSED = 0x00000800,

        /// <summary>
        /// Unknown
        /// </summary>
        FILE_ATTRIBUTE_OFFLINE = 0x00001000,

        /// <summary>
        /// Unknown
        /// </summary>
        FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,

        /// <summary>
        /// Для перекрытого ввода-вывода
        /// </summary>
        FILE_FLAG_OVERLAPPED = 0x40000000
    };

    /// <summary>
    /// Флаги создания/открытия файла
    /// </summary>
    internal enum CREATION_DISPOSITION : uint
    {
        /// <summary>
        /// Создать новый 
        /// </summary>
        CREATE_NEW = 1,

        /// <summary>
        /// Создавать всегда
        /// </summary>
        CREATE_ALWAYS,

        /// <summary>
        /// Открыть существующий
        /// </summary>
        OPEN_EXISTING,

        /// <summary>
        /// Открывать всегда
        /// </summary>
        OPEN_ALWAYS,

        /// <summary>
        /// Открывать существующий и удалять текущее содержимое
        /// </summary>
        TRUNCATE_EXISTING
    };

    /// <summary>
    /// Флаги прерывания операций
    /// </summary>
    internal enum PURGE_FLAGS : uint
    {
        /// <summary>
        /// Пустое значение
        /// </summary>
        PURGE_EMPTY = 0x0000,

        /// <summary>
        /// Прервать передачу
        /// </summary>
        PURGE_TXABORT = 0x0001,

        /// <summary>
        /// Прервать прием
        /// </summary>
        PURGE_RXABORT = 0x0002,

        /// <summary>
        /// Очистить исходящий буфер
        /// </summary>
        PURGE_TXCLEAR = 0x0004,

        /// <summary>
        /// Очистить входящий буфер
        /// </summary>
        PURGE_RXCLEAR = 0x0008
    }

    /// <summary>
    /// События коммункационного устройства
    /// </summary>
    internal enum CommEvents : uint
    {
        /// <summary>
        /// Any Character received
        /// </summary>
        EV_RXCHAR = 0x0001,

        /// <summary>
        /// Received certain character
        /// </summary>
        EV_RXFLAG = 0x0002,

        /// <summary>
        /// Transmitt Queue Empty
        /// </summary>
        EV_TXEMPTY = 0x0004,

        /// <summary>
        /// CTS changed state
        /// </summary>
        EV_CTS = 0x0008,

        /// <summary>
        /// DSR changed state
        /// </summary>
        EV_DSR = 0x0010,

        /// <summary>
        /// RLSD changed state
        /// </summary>
        EV_RLSD = 0x0020,

        /// <summary>
        /// BREAK received
        /// </summary>
        EV_BREAK = 0x0040,

        /// <summary>
        /// Line status error occurred
        /// </summary>
        EV_ERR = 0x0080,

        /// <summary>
        /// Ring signal detected
        /// </summary>
        EV_RING = 0x0100,

        /// <summary>
        /// Printer error occured
        /// </summary>
        EV_PERR = 0x0200,

        /// <summary>
        /// Receive buffer is 80 percent full
        /// </summary>
        EV_RX80FULL = 0x0400,

        /// <summary>
        /// Provider specific event 1
        /// </summary>
        EV_EVENT1 = 0x0800,

        /// <summary>
        /// Provider specific event 2
        /// </summary>
        EV_EVENT2 = 0x1000
    }

    /// <summary>
    /// Extended function for EscapeCommFunction
    /// </summary>
    internal enum ExtendedFunctions : uint
    {
        CLRDTR = 6,
        CLRRTS = 4,
        SETDTR = 5,
        SETRTS = 3
    }

    /// <summary>
    /// Системные коды ошибок, фильтруемые в библиотеке
    /// </summary>
    internal enum SystemErrorCodes : uint
    {
        /// <summary>
        /// The requested resource is in use
        /// </summary>
        ERROR_BUSY = 170,

        /// <summary>
        /// The I/O operation has been aborted because of either a thread exit or an application request
        /// </summary>
        ERROR_OPERATION_ABORTED = 995,

        /// <summary>
        /// The device is not connected
        /// </summary>
        ERROR_DEVICE_NOT_CONNECTED = 1167
    }

    /// <summary>
    /// DTR line control
    /// </summary>
    public enum DtrControl : uint
    {
        /// <summary>
        /// Disables the DTR line when the device is opened and leaves it disabled
        /// </summary>
        Disable = 0x00,

        /// <summary>
        /// Enables the DTR line when the device is opened and leaves it on
        /// </summary>
        Enable = 0x01,

        /// <summary>
        /// Enables DTR handshaking
        /// </summary>
        Handshake = 0x02
    }

    /// <summary>
    /// RTS line control
    /// </summary>
    public enum RtsControl : uint
    {
        /// <summary>
        /// Disables the RTS line when the device is opened and leaves it disabled
        /// </summary>
        Disable = 0x00,

        /// <summary>
        /// Enables the RTS line when the device is opened and leaves it on
        /// </summary>
        Enable = 0x01,

        /// <summary>
        /// Enables RTS handshaking
        /// </summary>
        Handshake = 0x02,

        /// <summary>
        /// Specifies that the RTS line will be high if bytes are available for transmission
        /// </summary>
        Toggle = 0x03
    }

    /// <summary>
    /// Defines the control setting for a serial communications device
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DCB
    {
        #region Поля

        /// <summary>
        /// Length of the structure, in bytes
        /// </summary>
        public uint DCBlength;

        /// <summary>
        /// Baud rate at which the communications device operates
        /// </summary>
        public uint BaudRate;

        /// <summary>
        /// Communication flags
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Reserved; must be zero
        /// </summary>
        public ushort wReserved;

        /// <summary>
        /// Minimum number of bytes allowed in the input buffer before flow control is activated to inhibit the sender
        /// </summary>
        public ushort XonLim;

        /// <summary>
        /// Maximum number of bytes allowed in the input buffer before flow control is activated to allow 
        /// transmission by the sender
        /// </summary>
        public ushort XoffLim;

        /// <summary>
        /// Number of bits in the bytes transmitted and received
        /// </summary>
        public byte ByteSize;

        /// <summary>
        /// Parity scheme to be used
        /// </summary>
        public byte Parity;

        /// <summary>
        /// Number of stop bits to be used
        /// </summary>
        public byte StopBits;

        /// <summary>
        /// Value of the XON character for both transmission and reception
        /// </summary>
        public sbyte XonChar;

        /// <summary>
        /// Value of the XOFF character for both transmission and reception
        /// </summary>
        public sbyte XoffChar;

        /// <summary>
        /// Value of the character used to replace bytes received with a parity error
        /// </summary>
        public sbyte ErrorChar;

        /// <summary>
        /// Value of the character used to signal the end of data
        /// </summary>
        public sbyte EofChar;

        /// <summary>
        /// Value of the character used to signal an event
        /// </summary>
        public sbyte EvtChar;

        /// <summary>
        /// Reserved; do not use
        /// </summary>
        public ushort wReserved1;

        #endregion

        #region Свойства

        /// <summary>
        /// If this member is TRUE, binary mode is enabled
        /// </summary>
        public uint fBinary
        {
            get { return Flags & 0x0001; }
            set { Flags = Flags & ~1U | value; }
        }

        /// <summary>
        /// If this member is TRUE, parity checking is performed and errors are reported
        /// </summary>
        public uint fParity
        {
            get { return (Flags >> 1) & 1; }
            set { Flags = Flags & ~(1U << 1) | (value << 1); }
        }

        /// <summary>
        /// If this member is TRUE, the CTS (clear-to-send) signal is monitored for output flow control. 
        /// If this member is TRUE and CTS is turned off, output is suspended until CTS is sent again
        /// </summary>
        public uint fOutxCtsFlow
        {
            get { return (Flags >> 2) & 1; }
            set { Flags = Flags & ~(1U << 2) | (value << 2); }
        }

        /// <summary>
        /// If this member is TRUE, the DSR (data-set-ready) signal is monitored for output flow control. 
        /// If this member is TRUE and DSR is turned off, output is suspended until DSR is sent again
        /// </summary>
        public uint fOutxDsrFlow
        {
            get { return (Flags >> 3) & 1; }
            set { Flags = Flags & ~(1U << 3) | (value << 3); }
        }

        /// <summary>
        /// DTR (data-terminal-ready) flow control
        /// </summary>
        public uint fDtrControl
        {
            get { return (Flags >> 4) & 3; }
            set { Flags = Flags & ~(3U << 4) | (value << 4); }
        }

        /// <summary>
        /// If this member is TRUE, the communications driver is sensitive to the state of the DSR signal. 
        /// The driver ignores any bytes received, unless the DSR modem input line is high
        /// </summary>
        public uint fDsrSensitivity
        {
            get { return (Flags >> 6) & 1; }
            set { Flags = Flags & ~(1U << 6) | (value << 6); }
        }

        /// <summary>
        /// If this member is TRUE, transmission continues after the input buffer has come within XoffLim bytes of 
        /// being full and the driver has transmitted the XoffChar character to stop receiving bytes. 
        /// If this member is FALSE, transmission does not continue until the input buffer is within XonLim bytes 
        /// of being empty and the driver has transmitted the XonChar character to resume reception
        /// </summary>
        public uint fTXContinueOnXoff
        {
            get { return (Flags >> 7) & 1; }
            set { Flags = Flags & ~(1U << 7) | (value << 7); }
        }

        /// <summary>
        /// Indicates whether XON/XOFF flow control is used during transmission
        /// </summary>
        public uint fOutX
        {
            get { return (Flags >> 8) & 1; }
            set { Flags = Flags & ~(1U << 8) | (value << 8); }
        }

        /// <summary>
        /// Indicates whether XON/XOFF flow control is used during reception
        /// </summary>
        public uint fInX
        {
            get { return (Flags >> 9) & 1; }
            set { Flags = Flags & ~(1U << 9) | (value << 9); }
        }

        /// <summary>
        /// Indicates whether bytes received with parity errors are replaced with the character specified by the 
        /// ErrorChar member
        /// </summary>
        public uint fErrorChar
        {
            get { return (Flags >> 10) & 1; }
            set { Flags = Flags & ~(1U << 10) | (value << 10); }
        }

        /// <summary>
        /// If this member is TRUE, null bytes are discarded when received
        /// </summary>
        public uint fNull
        {
            get { return (Flags >> 11) & 1; }
            set { Flags = Flags & ~(1U << 11) | (value << 11); }
        }

        /// <summary>
        /// RTS (request-to-send) flow control
        /// </summary>
        public uint fRtsControl
        {
            get { return (Flags >> 12) & 3; }
            set { Flags = Flags & ~(3U << 12) | (value << 12); }
        }

        /// <summary>
        /// If this member is TRUE, the driver terminates all read and write operations with an error status 
        /// if an error occurs
        /// </summary>
        public uint fAbortOnError
        {
            get { return (Flags >> 14) & 1; }
            set { Flags = Flags & ~(1U << 14) | (value << 14); }
        }

        #endregion
    }

    /// <summary>
    /// Таймауты последовательного порта
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct COMMTIMEOUTS
    {
        public uint ReadIntervalTimeout;
        public uint ReadTotalTimeoutMultiplier;
        public uint ReadTotalTimeoutConstant;
        public uint WriteTotalTimeoutMultiplier;
        public uint WriteTotalTimeoutConstant;
    }

    /// <summary>
    /// Для перекрытого ввода-вывода
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct OVERLAPPED
    {
        internal UIntPtr Internal;
        internal UIntPtr InternalHigh;
        internal uint Offset;
        internal uint OffsetHigh;
        internal IntPtr hEvent;
    }

    /// <summary>
    /// Обертка для Windows API
    /// </summary>
    internal class WinApi
    {
        /// <summary>
        /// Проверка результата вызова API-функции
        /// </summary>
        /// <param name="value">Результат вызова</param>
        /// <exception cref="Win32Exception">Исключение бросается, если value == false</exception>
        internal static void Win32Check(bool value)
        {
            if (!value)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Creates or opens a file, file stream, directory, physical disk, volume, console buffer, 
        /// tape drive, communications resource, mailslot, or named pipe
        /// </summary>
        /// <param name="lpFileName">A pointer to a null-terminated string that specifies the 
        /// name of an object to create or open</param>
        /// <param name="dwDesiredAccess">The access to the object, which can be read, write, or both</param>
        /// <param name="dwShareMode">The sharing mode of an object, which can be read, write, both, or none</param>
        /// <param name="lpSecurityAttributes">A pointer to a SECURITY_ATTRIBUTES structure that determines 
        /// whether or not the returned handle can be inherited by child processes</param>
        /// <param name="dwCreationDisposition">An action to take on files that exist and do not exist</param>
        /// <param name="dwFlagsAndAttributes">The file attributes and flags</param>
        /// <param name="hTemplateFile">A handle to a template file with the GENERIC_READ access right</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static SafeFileHandle CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            CREATION_DISPOSITION dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
            );

        /// <summary>
        /// Reads data from a file, starting at the position that the file pointer indicates
        /// </summary>
        /// <param name="hFile">A handle to the file to be read</param>
        /// <param name="lpBuffer">A pointer to the buffer that receives the data read from a file</param>
        /// <param name="nNumberOfBytesToRead">The number of bytes to be read from a file</param>
        /// <param name="lpNumberOfBytesRead">A pointer to the variable that receives the number of bytes read</param>
        /// <param name="lpOverlapped">Pointer to an OVERLAPPED structure. Do not use</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool ReadFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            ref uint lpNumberOfBytesRead,
            IntPtr lpOverlapped
            );

        /// <summary>
        /// Writes data to a file at the position specified by the file pointer
        /// </summary>
        /// <param name="hFile">Handle to the file</param>
        /// <param name="lpBuffer">Pointer to the buffer containing the data to be written to the file</param>
        /// <param name="nNumberOfBytesToWrite">Number of bytes to be written to the file</param>
        /// <param name="lpNumberOfBytesWritten">Pointer to the variable that receives the number of bytes written</param>
        /// <param name="lpOverlapped">Pointer to an OVERLAPPED structure. Do not use</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool WriteFile(
            SafeFileHandle hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            ref uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped
            );

        /// <summary>
        /// Retrieves the current control settings for a specified communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="lpDCB">Pointer to a DCB structure that receives the control settings information</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool GetCommState(SafeFileHandle hFile, ref DCB lpDCB);

        /// <summary>
        /// Configures a communications device according to the specifications in a device-control block
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="lpDCB">Reference to a DCB structure that contains the configuration information 
        /// for the specified communications device</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool SetCommState(
            SafeFileHandle hFile,
            ref DCB lpDCB
            );

        /// <summary>
        /// Discards all characters from the output or input buffer of a specified communications resource. 
        /// It can also terminate pending read or write operations on the resource.
        /// </summary>
        /// <param name="hFile">Handle to the communications resource</param>
        /// <param name="dwFlags">Flags</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool PurgeComm(
            SafeFileHandle hFile,
            uint dwFlags
            );

        /// <summary>
        /// Retrieves the time-out parameters for all read and write operations on a 
        /// specified communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications resource</param>
        /// <param name="lpCommTimeouts">Reference to a COMMTIMEOUTS structure in which the time-out 
        /// information is returned</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool GetCommTimeouts(
            SafeFileHandle hFile,
            ref COMMTIMEOUTS lpCommTimeouts
            );

        /// <summary>
        /// Sets the time-out parameters for all read and write operations on a specified communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications resource</param>
        /// <param name="lpCommTimeouts">Pointer to a COMMTIMEOUTS structure that contains the new 
        /// time-out values</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool SetCommTimeouts(
            SafeHandle hFile,
            ref COMMTIMEOUTS lpCommTimeouts
            );

        /// <summary>
        /// Waits for an event to occur for a specified communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="lpEvtMask">Pointer to a variable that receives a mask indicating the type of 
        /// event that occurred</param>
        /// <param name="lpOverlapped">Pointer to an OVERLAPPED structure. Do not use</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool WaitCommEvent(
            SafeFileHandle hFile,
            ref uint lpEvtMask,
            IntPtr lpOverlapped
            );

        /// <summary>
        /// Specifies a set of events to be monitored for a communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="dwEvtMask">Events to be enabled</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool SetCommMask(
            SafeFileHandle hFile,
            uint dwEvtMask
            );

        /// <summary>
        /// Directs a specified communications device to perform an extended function
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="dwFunc">Extended function to be performed</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool EscapeCommFunction(
            SafeFileHandle hFile,
            uint dwFunc
            );

        /// <summary>
        /// Initializes the communications parameters for a specified communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="dwInQueue">Recommended size of the device's internal input buffer, in bytes</param>
        /// <param name="dwOutQueue">Recommended size of the device's internal output buffer, in bytes</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool SetupComm(
            SafeFileHandle hFile,
            uint dwInQueue,
            uint dwOutQueue
            );

        /// <summary>
        /// Retrieves information about a communications error and reports the current status of a communications device
        /// </summary>
        /// <param name="hFile">Handle to the communications device</param>
        /// <param name="lpErrors">Pointer to a variable to be filled with a mask indicating the type of error</param>
        /// <param name="lpStat">Pointer to a COMSTAT structure in which the device's status information is returned</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool ClearCommError(
            SafeFileHandle hFile,
            ref uint lpErrors,
            IntPtr lpStat
            );

        /// <summary>
        /// Flushes the buffers of a specified file and causes all buffered 
        /// data to be written to a file
        /// </summary>
        /// <param name="hFile">A handle to an open file</param>
        [DllImport("Kernel32.dll", SetLastError = true)]
        internal extern static bool FlushFileBuffers(SafeFileHandle hFile);
    }
}
