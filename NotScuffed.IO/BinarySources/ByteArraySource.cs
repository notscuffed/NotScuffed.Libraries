using System.IO;
using NotScuffed.Common;
using NotScuffed.IO.Abstractions;

namespace NotScuffed.IO.BinarySources
{
    public class ByteArraySource : IBinarySource
    {
        private readonly byte[] _array;
        private readonly int _offset;
        private readonly int _size;

        public ByteArraySource(byte[] array, int offset = 0, int size = -1)
        {
            _array = array;
            _offset = offset;
            _size = size;
        }

        public BinaryReader CreateReader()
        {
            return new BinaryReader(CreateStream());
        }

        public Stream CreateStream()
        {
            return new MemoryStream(_array, _offset, _size == -1 ? _array.Length : _size);
        }

        public IBinarySource OffsetedSource(long offset, long size)
        {
            if (_offset + offset > int.MaxValue)
                ThrowHelper.ThrowOverflowException("Offset doesn't fit inside 32-bit integer");
            
            if (size > int.MaxValue)
                ThrowHelper.ThrowOverflowException("Size doesn't fit inside 32-bit integer");
            
            return new ByteArraySource(_array, (int) (_offset + offset), (int) size);
        }
    }
}
