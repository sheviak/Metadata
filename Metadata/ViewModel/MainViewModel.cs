using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using Metadata.Data;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Metadata.Storage;
using System.Diagnostics;

namespace Metadata.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private const string link = "https://mssg.me/sheviak.k";
        /// <summary>
        /// Свойство отображающие путь к выбранной папке
        /// </summary>
        private string _path = "";
        public string PATH
        {
            get { return this._path; }
            set
            {
                this._path = value;
                OnPropertyChanged("PATH");
            }
        }

        /// <summary>
        /// Свойство отвечающие за открытие и закрытие 
        /// окна popup (добавления и редактирвоания)
        /// </summary>
        private bool _OpenDialog;
        public bool OpenDialog
        {
            get => _OpenDialog;
            set
            {
                _OpenDialog = value;
                OnPropertyChanged("OpenDialog");
            }
        }

        /// <summary>
        /// Свойство - индикатор происходящего (загрузка | сохранение)
        /// </summary>
        private string _indicator;
        public string Indicator
        {
            get => _indicator;
            set
            {
                _indicator = value;
                OnPropertyChanged("Indicator");
            }
        }

        /// <summary>
        /// Свойство - выбранный элемент из DataGrid
        /// </summary>
        private dynamic _selectedItem;
        public dynamic SelItem
        {
            get
            {
                if (_selectedItem is JPGInfo)
                {
                    var obj = (JPGInfo)_selectedItem;
                    GetInfo = obj.GetInformation();
                    return obj;
                }

                if (_selectedItem is PDFInfo)
                {
                    var obj = (PDFInfo)_selectedItem;
                    GetInfo = obj.GetInformation();
                    return obj;
                }

                if (_selectedItem is PNGInfo)
                {
                    var obj = (PNGInfo)_selectedItem;
                    GetInfo = obj.GetInformation();
                    return obj;
                }

                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelItem");
            }
        }

        /// <summary>
        /// Коллекция - содержащая подробную информацию о файле
        /// </summary>
        private Dictionary<string, string> _getInfo;
        public Dictionary<string, string> GetInfo
        {
            get => _getInfo;
            set
            {
                _getInfo = value;
                OnPropertyChanged("GetInfo");
            }
        }

        /// <summary>
        /// коллекция - содержащая все отсканированные объекты
        /// </summary>
        private ObservableCollection<BaseFileInfo> _files = new ObservableCollection<BaseFileInfo>();
        public ObservableCollection<BaseFileInfo> Files
        {
            get => _files;
            set
            {
                _files = value;
                OnPropertyChanged("Files");
            }
        }

        public MainViewModel() { }

        public ICommand CloseProgram => new RelayCommand(() => Environment.Exit(0));
        public ICommand MinimizedProgram => new RelayCommand(() => {
            var app = System.Windows.Application.Current.Windows[0];
            app.WindowState = WindowState.Minimized;
        });

        public ICommand OpenDir => new RelayCommand(() => GetAllFilesInFolder());
        public ICommand LoadLib => new RelayCommand(() => LoadInLibrary());
        public ICommand SaveAll => new RelayCommand(() => SaveInLibrary());
        public ICommand SaveOne => new RelayCommand(() => SaveInfoAboutFile());
        public ICommand OpenWebsite => new RelayCommand(() => Process.Start(link));
        public ICommand CopyFileInfo => new RelayCommand(() => CopyInfo());
        public ICommand OpenFile => new RelayCommand(() => OpenFileInProgramm());

        /// <summary>
        /// Метод загружающий все данные о файлах из библиотеки
        /// </summary>
        private async void LoadInLibrary()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    Indicator = "Load library...";
                    OpenDialog = true;
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        try
                        {
                            var t = new Parser();
                            Files = t.GetLibrary(openFileDialog.FileName);
                        }
                        catch (Exception)
                        {
                            System.Windows.MessageBox.Show("Error parsing file", "Error!");
                        }
                    });
                    OpenDialog = false;
                });
            }
        }

        /// <summary>
        /// Метод - сохраняющий все объекты из программы в файл типа json
        /// </summary>
        private async void SaveInLibrary()
        {
            if (Files.Count == 0)
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files(*.json)|*.json";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    Indicator = "Save in library...";
                    OpenDialog = true;
                    // получаем выбранный файл
                    string filename = saveFileDialog.FileName;
                    // сохраняем текст в файл
                    var save = new Save();
                    save.Serialize(Files, filename);
                    OpenDialog = false;
                });
            }
        }

        private void CopyInfo()
        {
            if (Files.Count == 0 || SelItem == null)
                return;

            string c = "";
            foreach (var item in GetInfo)
            {
                c += item.Key + "\t\t" + item.Value + "\n";
            }
            System.Windows.Clipboard.SetText(c);
        }

        private async void SaveInfoAboutFile()
        {
            if (Files.Count == 0)
                return;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "TXT files(*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    Indicator = "Save information...";
                    OpenDialog = true;
                    // получаем выбранный файл
                    string filename = saveFileDialog.FileName;
                    // сохраняем текст в файл
                    var save = new Save();
                    save.SaveInformation(GetInfo, filename);
                    OpenDialog = false;
                });
            }
        }

        /// <summary>
        /// Открытие файла ассоциированной с ним программой
        /// </summary>
        private void OpenFileInProgramm()
        {
            try
            {
                Process.Start(((BaseFileInfo)SelItem).FilePath);
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("No file in this way!");
            }
        }

        /// <summary>
        /// Метод заполняющий коллекцию файлами
        /// </summary>
        private async void GetAllFilesInFolder()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    App.Current.Dispatcher.Invoke((Action)delegate { Files.Clear(); });
                    Indicator = "Load data...";
                    OpenDialog = true;
                    PATH = folderBrowser.SelectedPath;
                    var allowedExtentions = new List<string>(new[] { ".png", ".jpg", ".pdf" });
                    var files = Directory
                        .GetFiles(folderBrowser.SelectedPath)
                        .Where(f => allowedExtentions.Contains(Path.GetExtension(f)));

                    foreach (var s in files)
                    {
                        switch (Path.GetExtension(s).ToLower())
                        {
                            case ".jpg":
                                try
                                {
                                    var jpg = GetValues(s);
                                    var objectJpg = new JPGInfo();
                                    objectJpg = objectJpg.GetObject(
                                        TmpImgEXIF: jpg.metadata,
                                        length: jpg.foto.Length,
                                        filename: Path.GetFileName(s),
                                        fullpath: Path.GetFullPath(s),
                                        path: s
                                    );
                                    jpg.foto.Close();
                                    App.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        objectJpg.FileIcon = new BitmapImage(new Uri("Icons/image.png", UriKind.Relative));
                                        this.Files.Add(objectJpg);
                                    });
                                }
                                catch { }
                                break;
                            case ".png":
                                try
                                {
                                    var png = GetValues(s);
                                    var objectPng = new PNGInfo();
                                    objectPng = objectPng.GetObject(
                                        TmpImgEXIF: png.metadata,
                                        length: png.foto.Length,
                                        filename: Path.GetFileName(s),
                                        fullpath: Path.GetFullPath(s),
                                        path: s
                                    );
                                    png.foto.Close();
                                    App.Current.Dispatcher.Invoke((Action)delegate
                                    {
                                        objectPng.FileIcon = new BitmapImage(new Uri("Icons/image.png", UriKind.Relative));
                                        this.Files.Add(objectPng);
                                    });
                                }
                                catch { }
                                break;
                            case ".pdf":
                                try
                                {
                                    PdfDocument document = PdfReader.Open(s);
                                    var obj = new PDFInfo();
                                    obj = obj.GetObject(
                                        document: document,
                                        name: Path.GetFileName(s),
                                        fullpath: Path.GetFullPath(s)
                                    );
                                    App.Current.Dispatcher.Invoke((Action)delegate {
                                        obj.FileIcon = new BitmapImage(new Uri("Icons/pdf.png", UriKind.Relative));
                                        this.Files.Add(obj);
                                    });
                                }
                                catch { }
                                break;
                        }
                    }
                    OpenDialog = false;
                });
            }
        }

        /// <summary>
        /// Метод возвращаюший кортеж из потока FileStream и фрейма из BitmapMetadata
        /// </summary>
        /// <param name="s">Путь</param>
        /// <returns>Возврат объектов FileStream и BitmapMetadata</returns>
        private (FileStream foto, BitmapMetadata metadata) GetValues(string s)
        {
            FileStream Foto = File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read);
            BitmapDecoder decoder = JpegBitmapDecoder.Create(Foto, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default);
            BitmapMetadata TmpImgEXIF = (BitmapMetadata)decoder.Frames[0].Metadata.Clone();

            return (foto: Foto, metadata: TmpImgEXIF);
        }
    }
}


