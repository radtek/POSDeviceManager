using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ERPService.SharedLibs.Helpers
{
    /// <summary>
    /// ��������� ������� ��� ����������� �������
    /// </summary>
    public enum SourceStartPosition
    {
        /// <summary>
        /// � ������ ������-���������
        /// </summary>
        Beginning,

        /// <summary>
        /// � ������� ������� ������-���������
        /// </summary>
        Current,

        /// <summary>
        /// � ������������ ������� ������-���������
        /// </summary>
        Offset
    }

    /// <summary>
    /// ��������������� ����� ��� ����������� ������ ������ ������ � ������
    /// </summary>
    public class StreamCopier
    {
        #region ����

        private byte[] _buffer;
        private bool _disposeSource;
        private bool _disposeDest;
        private Int64 _sourceOffset;
        private Int64 _length;
        private SourceStartPosition _startPosition;

        #endregion

        #region ��������

        /// <summary>
        /// ������ ����������� ������ ������
        /// </summary>
        public int BufferSize
        {
            get { return _buffer.Length; }
            set 
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("BufferSize");
                _buffer = new byte[value];
            }
        }

        /// <summary>
        /// ����������� ������� ������-���������
        /// </summary>
        /// <remarks>�������� �� ��������� - false</remarks>
        public bool DisposeSource
        {
            get { return _disposeSource; }
            set { _disposeSource = value; }
        }

        /// <summary>
        /// ����������� ������� ������-���������
        /// </summary>
        /// <remarks>�������� �� ��������� - false</remarks>
        public bool DisposeDest
        {
            get { return _disposeDest; }
            set { _disposeDest = value; }
        }

        /// <summary>
        /// �������� �� ������ ������-���������
        /// </summary>
        /// <remarks>������������, ���� �������� StartPosition 
        /// ����������� � SourceStartPosition.Offset</remarks>
        public Int64 SourceOffset
        {
            get { return _sourceOffset; }
            set 
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("SourceOffset");
                _sourceOffset = value; 
            }
        }

        /// <summary>
        /// ����� ���������� ������
        /// </summary>
        /// <remarks>
        /// ���� � ������ ������ ������ Copy ����� ���������� ������ �����: 
        /// 1) ����, �� ��� ��������������� ������ ����� ������-���������;
        /// 2) -1, �� ����������� ����������� �� ����� ������
        /// </remarks>
        public Int64 Length
        {
            get { return _length; }
            set 
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException("Length");
                _length = value; 
            }
        }

        /// <summary>
        /// ��������� ������� � ������-���������
        /// </summary>
        /// <remarks>��� �������, �� �������������� �����, �������� �������� ������ ���� 
        /// ����������� � �������� SourceStartPosition.Current</remarks>
        public SourceStartPosition StartPosition
        {
            get { return _startPosition; }
            set { _startPosition = value; }
        }

        #endregion

        #region ������������

        /// <summary>
        /// ������� ��������� ������
        /// </summary>
        public StreamCopier(int bufferSize)
        {
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException("bufferSize");

            // ������������� ������� ���������� ����������
            BufferSize = bufferSize;
            _disposeSource = false;
            _disposeDest = false;
            _sourceOffset = 0;
            _length = 0;
            _startPosition = SourceStartPosition.Beginning;
        }

        /// <summary>
        /// ������� ��������� ������, ������ ������ ������ ������ 1 ��
        /// </summary>
        public StreamCopier()
            : this(1048576)
        {
        }

        #endregion

        #region �������� ������

        /// <summary>
        /// ����������� ������ �� ������ ������ � ������
        /// </summary>
        /// <param name="source">�����-��������</param>
        /// <param name="dest">�����-����������</param>
        /// <returns>���������� ������������� ����</returns>
        public Int64 Copy(Stream source, Stream dest)
        {
            try
            {
                // ��������� ������� � ������-���������
                switch (_startPosition)
                {
                    case SourceStartPosition.Beginning:
                        if (source.CanSeek)
                            source.Seek(0, SeekOrigin.Begin);
                        break;
                    case SourceStartPosition.Offset:
                        if (source.CanSeek)
                            source.Seek(_sourceOffset, SeekOrigin.Begin);
                        break;
                }

                Int64 totalCopied = 0;

                // ����� ���������� ������
                switch (_length)
                {
                    case 0:
                        // ����� ����� ������ ���������
                        _length = source.Length;
                        break;
                    case -1:
                        // ����� ����������� ���������� ���������� ����
                        // ������ ����� ����������� �� ����� ������-���������
                        _length = Int64.MaxValue;
                        break;
                }

                while (totalCopied < _length)
                {
                    Int64 count = _length - totalCopied;
                    if (count > _buffer.Length)
                        count = _buffer.Length;

                    int bytesRead = source.Read(_buffer, 0, (int)count);
                    if (bytesRead == 0)
                        break;

                    dest.Write(_buffer, 0, bytesRead);
                    totalCopied += bytesRead;
                }

                return totalCopied;
            }
            finally
            {
                if (_disposeSource)
                    source.Dispose();
                if (_disposeDest)
                    dest.Dispose();
            }
        }

        #endregion
    }
}
