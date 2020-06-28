using System;
using System.IO;

namespace NotScuffed.IO
{
    public class SubStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly long _offset;
        private readonly long _length;
        private long _position;

        public SubStream(Stream baseStream, long offset, long length)
        {
            if (baseStream == null) throw new ArgumentNullException(nameof(baseStream));
            if (!baseStream.CanSeek) throw new NotSupportedException();
            if (!baseStream.CanRead) throw new ArgumentException("Cannot read base stream");
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            _baseStream = baseStream;
            _position = 0;
            _offset = offset;
            _length = length;
            
            baseStream.Seek(offset, SeekOrigin.Current);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckIfDisposed();
            var remaining = _length - _position;
            if (remaining <= 0) return 0;
            if (remaining < count) count = (int) remaining;
            var read = _baseStream.Read(buffer, offset, count);
            _position += read;
            return read;
        }

        public override bool CanRead
        {
            get
            {
                CheckIfDisposed();
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                CheckIfDisposed();
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                CheckIfDisposed();
                return false;
            }
        }

        public override long Length
        {
            get
            {
                CheckIfDisposed();
                return _length;
            }
        }

        public override long Position
        {
            get
            {
                CheckIfDisposed();
                return _position;
            }
            set => throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return origin switch
            {
                SeekOrigin.Begin => _position = _baseStream.Seek(_offset + offset, origin) - _offset,
                SeekOrigin.Current => _position = _baseStream.Seek(offset, origin) - _offset,
                SeekOrigin.End => _position =
                    _baseStream.Seek(_baseStream.Length - (_offset + _length) + offset, origin) - _offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            if (_baseStream == null)
                return;

            try
            {
                _baseStream.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        public override void Flush()
        {
            CheckIfDisposed();
            _baseStream.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
        
        private void CheckIfDisposed()
        {
            if (_baseStream == null) throw new ObjectDisposedException(nameof(SubStream));
        }
    }
}