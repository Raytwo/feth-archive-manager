using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using G1Tool.IO;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FETHArchiveManager
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DATA0 data0 { get; set; }
        private byte[] data1;
        public INFO0 info0 { get; set; }
        public INFO2 info2 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            data0 = (DATA0)this.Resources["data0"];
            info0 = (INFO0)this.Resources["info0"];
            info2 = new INFO2();
        }

        private void AddButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Add(new INFO0Entry()
            {
                EntryID = 0,
                UncompressedSize = 0x0,
                CompressedSize = 0x0,
                Compressed = false,
                Filepath = "rom:/"
            });
        }

        private void SaveButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".bin";
            dialog.FileName = "INFO0";
            dialog.Filter =
                    "FETH Patch Binary (INFO0.bin)|*.bin";
            if (dialog.ShowDialog() == true)
            {
                INFO2 info2 = new INFO2(info0.Count);
                info0.Write(dialog.FileName);
                info2.Write(Path.GetDirectoryName(dialog.FileName) + "/INFO2.bin");
            }
        }

        private void OpenButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Clear();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".bin";
            dialog.FileName = "INFO0";
            dialog.Filter =
                    "FETH Patch Binary (INFO0.bin)|*.bin";

            if (dialog.ShowDialog() == true)
            {
                info0.Read(dialog.FileName);
                if (File.Exists(Path.GetFullPath(dialog.FileName) + "INFO2"))
                    info2.Read(Path.GetFullPath(dialog.FileName) + "INFO2");

            }
        }

        private void RemoveButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Remove((INFO0Entry)dataGridINFO.SelectedItem);
        }

        private void NewButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Clear();

            var assembly = Assembly.GetExecutingAssembly();
            string info0ResourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("patch2.INFO0.bin"));
            string info1ResourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("patch2.INFO1.bin"));
            string info2ResourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("patch2.INFO2.bin"));
            info0.Read(new EndianBinaryReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(info0ResourceName), Endianness.Little));
            info2.Read(new EndianBinaryReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(info2ResourceName), Endianness.Little));
        }

        private void OpenButtonDATA_Click(object sender, RoutedEventArgs e)
        {
            data0.Clear();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".bin";
            dialog.FileName = "DATA0";
            dialog.Filter =
                    "FETH Data Binary (DATA0.bin)|*.bin";

            if (dialog.ShowDialog() == true)
            {
                string data1Path = Path.GetDirectoryName(dialog.FileName) + "/DATA1.bin";

                data0.Read(dialog.FileName);

                if (File.Exists(data1Path))
                {
                    data1 = File.ReadAllBytes(data1Path);
                }
            }
        }

        private void ExtractButtonDATA_Click(object sender, RoutedEventArgs e)
        {
            Extract_Click(sender, e);
        }

        private void AddButtonDATA_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RemoveButtonDATA_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Extract_Click(object sender, RoutedEventArgs e)
        {
            DATA0Entry entry = (DATA0Entry)dataGridDATA.SelectedItem;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".bin.gz";
            dialog.FileName = entry.EntryID.ToString();
            dialog.Filter = "GZipped FETH DATA0 Entry|*.bin.gz";

            if (dialog.ShowDialog() == true)
            {
                using (EndianBinaryReader r = new EndianBinaryReader(new MemoryStream(data1), Endianness.Little))
                {
                    using (EndianBinaryWriter w = new EndianBinaryWriter(new FileStream(dialog.FileName, FileMode.Create), Endianness.Little))
                    {
                        r.SeekBegin(entry.Offset);

                        if (entry.Compressed)
                            w.Write(r.ReadBytes((int)entry.CompressedSize));
                        else
                            w.Write(r.ReadBytes((int)entry.UncompressedSize));
                    }
                }
            }
        }

        private void ExtractDecompress_Click(object sender, RoutedEventArgs e)
        {
            DATA0Entry entry = (DATA0Entry)dataGridDATA.SelectedItem;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".bin";
            dialog.FileName = entry.EntryID.ToString();
            dialog.Filter = "FETH DATA0 Entry|*.bin";

            if (entry.Compressed)
            {
                if (dialog.ShowDialog() == true)
                {
                    using (EndianBinaryReader r = new EndianBinaryReader(new MemoryStream(data1), Endianness.Little))
                    {
                        using (EndianBinaryWriter w = new EndianBinaryWriter(new FileStream(dialog.FileName, FileMode.Create), Endianness.Little))
                        {
                            KTGZip zlib = new KTGZip();
                            r.SeekBegin(entry.Offset);
                            w.Write(zlib.Decompress(r.ReadBytes((int)entry.CompressedSize)));
                        }
                    }
                }
            }
        }

        private void ExtractAllButtonDATA_Click(object sender, RoutedEventArgs e)
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;


            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach(DATA0Entry entry in data0)
                {
                    if (entry.CompressedSize == 0 || entry.UncompressedSize == 0)
                        continue;

                    using (EndianBinaryReader r = new EndianBinaryReader(new MemoryStream(data1), Endianness.Little))
                    {
                        using (EndianBinaryWriter w = new EndianBinaryWriter(new FileStream(dialog.FileName + "/" + entry.EntryID + ".bin", FileMode.Create), Endianness.Little))
                        {
                            r.SeekBegin(entry.Offset);

                            if (entry.Compressed)
                                w.Write(r.ReadBytes((int)entry.CompressedSize));
                            else
                                w.Write(r.ReadBytes((int)entry.UncompressedSize));
                        }
                    }
                }
                
            }
        }
    }
}
