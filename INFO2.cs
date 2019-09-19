using G1Tool.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FETHArchiveManager
{
    public class INFO2
    {
        public long INFO0Count { get; set; }
        public long INFO1Count => 4;

        public INFO2(long info0_count, long info1_count = 4)
        {
            INFO0Count = info0_count;
        }

        public void Write(string filename)
        {
            Write(new EndianBinaryWriter(filename, Endianness.Little));
        }

        public void Write(EndianBinaryWriter w)
        {
            w.Write(INFO0Count);
            w.Write(INFO1Count);

            w.Close();
        }
        public void Read(string filename)
        {
            Read(new EndianBinaryReader(filename, Endianness.Little));
        }

        public void Read(EndianBinaryReader r)
        {
            throw new NotImplementedException();
            //INFO0Count = r.ReadInt64();
        }
    }
}
