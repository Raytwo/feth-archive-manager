using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace FETHArchiveManager
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public INFO0 info0 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            info0 = (INFO0)this.Resources["info0"];
        }

        private void AddButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Add(new INFO0Entry() { EntryID = 0, UncompressedSize = 0x0, CompressedSize = 0x0, Compressed = false, Filepath = "rom:/" });
        }

        private void SaveButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".bin";
            dialog.FileName = "INFO0";
            dialog.Filter =
                    "FETH Patch Binary (INFO0.bin)|*.bin";
            if(dialog.ShowDialog() == true)
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
            }
        }

        private void RemoveButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Remove((INFO0Entry)dataGridINFO.SelectedItem);
        }

        private void NewButtonINFO_Click(object sender, RoutedEventArgs e)
        {
            info0.Clear();
        }
    }
}
