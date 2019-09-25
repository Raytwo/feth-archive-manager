using G1Tool.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FETHArchiveManager
{
    public class DATA0 : ObservableCollection<DATA0Entry>
    {
        public DATA0()
        {

        }

        public void Write(string filename)
        {
            Write(new EndianBinaryWriter(filename, Endianness.Little));
        }

        public void Write(EndianBinaryWriter w)
        {
            var output = from item in this orderby item.EntryID select item;

            foreach (DATA0Entry entry in output)
            {
                w.Write(entry.EntryID);
                w.Write(entry.UncompressedSize);
                w.Write(entry.CompressedSize);
                w.Write(entry.Compressed);
            }

            w.Close();
        }
        public void Read(string filename)
        {
            Read(new EndianBinaryReader(filename, Endianness.Little));
        }

        public void Read(EndianBinaryReader r)
        {
            long count = r.Length / 0x20;

            for (int i = 0; i < count; i++)
            {
                this.Add(new DATA0Entry()
                {
                    EntryID = r.ReadInt64(),
                    UncompressedSize = r.ReadInt64(),
                    CompressedSize = r.ReadInt64(),
                    Compressed = Convert.ToBoolean(r.ReadInt64())
                });
            }

            r.Close();
        }
    }

    public class DATA0Entry
    {
        public long EntryID { get; set; }
        public long UncompressedSize { get; set; }
        public long CompressedSize { get; set; }
        public bool Compressed { get; set; }
    }
}
