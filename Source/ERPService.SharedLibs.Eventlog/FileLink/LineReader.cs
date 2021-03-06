﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ERPService.SharedLibs.Eventlog.FileLink
{
    internal class LineReader
    {
        private const Int64 _bufSize = 1024;
        private Stream _stream;

        internal LineReader(Stream stream)
        {
            _stream = stream;
        }

        internal bool Eof
        {
            get
            {
                return _stream.Position == _stream.Length;
            }
        }

        internal void Seek(Int64 offset, SeekOrigin origin)
        {
            _stream.Seek(offset, origin);
        }

        internal string ReadLine()
        {
            // сюда помещаем результат чтения
            var result = new List<byte>((int)_bufSize);

            do
            {
                // определяем, сколько можем прочесть из потока
                var recentBytes = _stream.Length - _stream.Position;
                if (recentBytes > 0)
                {
                    // создаем временный буфер для чтения данных
                    var buffer = new byte[recentBytes > _bufSize ? _bufSize : recentBytes];

                    // читаем
                    var bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        // разбираем временный буфер
                        for (var i = 0; i < bytesRead; i++)
                        {
                            if (buffer[i] != 13 && buffer[i] != 10)
                                result.Add(buffer[i]);
                            else if (buffer[i] == 10)
                            {
                                // достигли конца строки
                                // определяем, насколько нужно вернуться назад
                                _stream.Seek(-(bytesRead - (i + 1)), SeekOrigin.Current);
                                // возващаем результат
                                return Encoding.Default.GetString(result.ToArray());
                            }
                        }
                    }
                }
            }
            while (_stream.Position < _stream.Length);

            // сюда попадаем, если файл состоит из одной строки, не завершенной 
            // символом LF
            return Encoding.Default.GetString(result.ToArray());
        }
    }
}
