using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NotScuffed.IO.Abstractions;
using NotScuffed.IO.BinarySources;
using NUnit.Framework;

namespace NotScuffed.Tests
{
    [TestFixture]
    public class BinarySourcesTests
    {
        [Test]
        public void ByteArraySourceTest()
        {
            var data = InitializeTestData();
            TestSource(new ByteArraySource(data));
        }
        
        [Test]
        public void FileSourceTest()
        {
            var tempPath = $"{Path.GetTempPath()}{Path.DirectorySeparatorChar}{Guid.NewGuid()}.bin";
            var data = InitializeTestData();
            File.WriteAllBytes(tempPath, data);
                
            try
            {
                TestSource(new FileSource(tempPath));
            }
            finally
            {
                File.Delete(tempPath);
            }
        }

        private static void TestSource(IBinarySource source)
        {
            var subSource = source.OffsetedSource(12, 24);

            {
                using var reader = source.CreateReader();
                Assert.AreEqual(0, reader.BaseStream.Position);
                Assert.AreEqual(1024, reader.BaseStream.Length);

                Assert.AreEqual(0, reader.ReadInt32());
                Assert.AreEqual(1, reader.ReadInt32());
                Assert.AreEqual(2, reader.ReadInt32());

                reader.BaseStream.Seek(-12, SeekOrigin.End);
                Assert.AreEqual(253, reader.ReadInt32());
                Assert.AreEqual(254, reader.ReadInt32());
                Assert.AreEqual(255, reader.ReadInt32());
                
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                Assert.AreEqual(255, reader.ReadInt32());
            }

            {
                using var reader = subSource.CreateReader();
                Assert.AreEqual(0, reader.BaseStream.Position);
                Assert.AreEqual(24, reader.BaseStream.Length);

                Assert.AreEqual(3, reader.ReadInt32());
                Assert.AreEqual(4, reader.ReadInt32());
                Assert.AreEqual(5, reader.ReadInt32());

                reader.BaseStream.Seek(-12, SeekOrigin.End);
                Assert.AreEqual(6, reader.ReadInt32());
                Assert.AreEqual(7, reader.ReadInt32());
                Assert.AreEqual(8, reader.ReadInt32());
                
                reader.BaseStream.Seek(-4, SeekOrigin.Current);
                Assert.AreEqual(8, reader.ReadInt32());
            }

            {
                using var reader = subSource.CreateReader();
                reader.BaseStream.Seek(0, SeekOrigin.End);
                Assert.Throws<EndOfStreamException>(() => reader.ReadInt32());
            }

            {
                using var reader = subSource.CreateReader();
                
                Assert.Throws<IOException>(() => reader.BaseStream.Seek(-1, SeekOrigin.Begin));
                Assert.Throws<IOException>(() => reader.BaseStream.Seek(-1, SeekOrigin.Current));
                Assert.Throws<IOException>(() => reader.BaseStream.Seek(-25, SeekOrigin.End));
                
                reader.BaseStream.Seek(1, SeekOrigin.End); // Seeking past end is fine
                
                // Verify we don't read anything when position is past the end of stream
                var temp = new byte[8];
                for (var i = 0; i < temp.Length; i++)
                    temp[i] = 0xC3;
                Assert.AreEqual(0, reader.Read(temp));
                Assert.IsTrue(temp.All(x => x == 0xC3));
            }
        }

        private static byte[] InitializeTestData()
        {
            var data = new byte[1024];

            var ints = MemoryMarshal.Cast<byte, int>(data);
            for (var i = 0; i < ints.Length; i++)
            {
                ints[i] = i;
            }

            return data;
        }
    }
}