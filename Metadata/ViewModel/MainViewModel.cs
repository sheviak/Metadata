using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Metadata.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public static string PATH = ".\\default";
        public ObservableCollection<string> Files { get; set; } = new ObservableCollection<string>();

        public MainViewModel()
        {
            string dirName = ".\\default";
            var files = Directory.GetFiles(dirName);
            foreach (string s in files)
            {
                Console.WriteLine(s);
            }
        }

        public ICommand OpenDir => new RelayCommand(() => GetAllFilesInFolder());
        public ICommand CloseProgram => new RelayCommand(() => Environment.Exit(0));
        public ICommand MinimizedProgram => new RelayCommand(() => {
            var app = System.Windows.Application.Current.Windows[0];
            app.WindowState = WindowState.Minimized;
        });

        public ICommand ExportFileInfo => new RelayCommand(() => System.Windows.MessageBox.Show("File exported"));
        public ICommand CopyFileInfo => new RelayCommand(() => System.Windows.MessageBox.Show("Info copied"));


        /// <summary>
        /// Метод заполняющий коллекцию файлами
        /// </summary>
        public void GetAllFilesInFolder()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PATH = folderBrowser.SelectedPath;
                var allowedExtentions = new List<string>(new[] { ".png", ".jpg", ".pdf" });
                var files = Directory
                    .GetFiles(PATH)
                    .Where(f => allowedExtentions.Contains(Path.GetExtension(f)));

                foreach (string s in files)
                {
                    Files.Add(s);
                }
            }
        }


    }
}
