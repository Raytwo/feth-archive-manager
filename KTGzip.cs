using G1Tool.IO;
using System;
using System.IO;
using System.IO.Compression;

namespace FETHArchiveManager
{
    public class KTGZip : ICompression
    {
        public byte[] Compress(byte[] file)
        {

            throw new NotImplementedException();
        }

        public byte[] Decompress(byte[] file)
        {
            using (EndianBinaryReader r = new EndianBinaryReader(new MemoryStream(file), Endianness.Little))
            {
                uint splitSize = r.ReadUInt32();
                uint entryCount = r.ReadUInt32();
                uint uncompSize = r.ReadUInt32();

                uint[] splits = r.ReadUInt32s((int)entryCount);
                byte[] output = new byte[uncompSize];

                r.SeekBegin((r.Position + 0x7F) & ~0x7F); // Align

                using (EndianBinaryWriter w = new EndianBinaryWriter(new MemoryStream(output), Endianness.Little))
                {
                    for (int i = 0; i < entryCount; i++)
                    {
                        uint cur_comp = r.ReadUInt32();
                        if (i == entryCount - 1)
                        {
                            if (cur_comp != splits[i] - 4)
                                w.Write(splits[i]);
                            else
                            {
                                using (GZipStream deflate = new GZipStream(new MemoryStream(r.ReadBytes((int)cur_comp)), CompressionMode.Decompress))
                                    deflate.CopyTo(w.BaseStream);
                            }
                        }
                        else
                        {
                            if (cur_comp == splits[i] - 4)
                            {
                                using (GZipStream deflate = new GZipStream(new MemoryStream(r.ReadBytes((int)cur_comp)), CompressionMode.Decompress))
                                    deflate.CopyTo(w.BaseStream);
                            }
                        }

                        r.SeekBegin(r.Position + 0x7F & ~0x7F); // Align
                    }
                }
                return output;
            }
        }
    }

}