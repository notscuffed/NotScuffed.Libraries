using System.IO;

namespace NotScuffed.IO.Abstractions
{
    public interface IBinarySource
    {
        BinaryReader CreateReader();
        Stream CreateStream();
        IBinarySource OffsetedSource(long offset, long size);
    }
}
