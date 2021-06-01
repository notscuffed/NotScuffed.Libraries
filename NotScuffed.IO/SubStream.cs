using System.IO;
using NotScuffed.Common;

namespace NotScuffed.IO
{
    public class SubStream : Stream
    {
        private Stream _baseStream;
        private readonly long _offset;
        private readonly long _length;
        private long _position;

        public SubStream(Stream baseStream, long offset, long length)
        {
            ArgGuard.ThrowIfNull(baseStream, nameof(baseStream));
            ArgGuard.ThrowIfFalse(baseStream.CanRead, nameof(baseStream));
            ArgGuard.ThrowIfFalse(baseStream.CanSeek, nameof(baseStream));
            ArgGuard.ThrowIfFalse(offset >= 0, nameof(offset));

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

        private void CheckIfDisposed()
        {
            if (_baseStream == null)
                ThrowHelper.ThrowObjectDisposedException(GetType().Name);
        }

        public override long Length
        {
            get
            {
                CheckIfDisposed();
                return _length;
            }
        }

        public override bool CanRead
        {
            get
            {
                CheckIfDisposed();
                return true;
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

        public override bool CanSeek
        {
            get
            {
                CheckIfDisposed();
                return true;
            }
        }

        public override long Position
        {
            get
            {
                CheckIfDisposed();
                return _position;
            }
            set => Seek(value, SeekOrigin.Begin);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            CheckIfDisposed();
            switch (origin)
            {
                case SeekOrigin.Begin:
                {
                    if (_offset + offset < _offset)
                        ThrowHelper.ThrowIOException("An attempt was made to move the substream pointer before the beginning of the substream.");
                    
                    return _position = _baseStream.Seek(_offset + offset, origin) - _offset;
                }
                case SeekOrigin.Current:
                {
                    if (_position + offset < _offset)
                        ThrowHelper.ThrowIOException("An attempt was made to move the substream pointer before the beginning of the substream.");
                    
                    return _position = _baseStream.Seek(offset, origin) - _offset;
                }
                case SeekOrigin.End:
                {
                    var newPosition = _offset + _length + offset;
                    
                    if (newPosition < _offset)
                        ThrowHelper.ThrowIOException("An attempt was made to move the substream pointer before the beginning of the substream.");
                    
                    return _position = _baseStream.Seek(newPosition, SeekOrigin.Begin) - _offset;
                }
                default:
                    ThrowHelper.ThrowArgumentOutOfRangeException(nameof(origin), origin);
                    return 0;
            }
        }

        public override void SetLength(long value)
        {
            ThrowHelper.ThrowNotSupportedException("Setting substream length is not supported");
        }

        public override void Flush()
        {
            CheckIfDisposed();
            _baseStream.Flush();
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

            _baseStream = null;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            ThrowHelper.ThrowNotImplementedException();
        }
    }
}
