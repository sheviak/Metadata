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
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Interop;
using Metadata.Storage;

namespace Metadata.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
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

                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelItem");
            }
        }

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

        private ObservableCollection<BaseFileInfo> _files;
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
             
        public void dd()
        {

            //Dictionary<string, List<object>> test = new Dictionary<string, List<object>>();

            /*
            test.Add("JPG", new ObservableCollection<Data.FileInfo>
            {
                new Data.JPG(name: "te", path: "sdf", tt: "sdf"),
                new Data.JPG(name: "te", path: "sdf", tt: "sdf"),
            });

            test.Add("PNG", new ObservableCollection<Data.FileInfo>
            {
                new Data.PNG(name: "er", path: "qwe", tt: "ssdsdfdf", re: "sdf")
            });
            */
            /*
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter("test_2.json"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, test);
                }
            }
            */


            /*
            // для диссериализации
            string json = "";
            json = File.ReadAllText("text.json");

            var o = JObject.Parse(json);
            var t1 = o["PNG"]
                .Select(x => new Data.PNG
                {
                    FileName = x["FileName"].ToString(),
                    FilePath = x["FilePath"].ToString(),
                    sd       = x["sd"].ToString(),
                    test     = x["test"].ToString()
                }).ToList();

            var t2 = o["JPG"]
                .Select(x => new Data.JPG
                {
                    FileName = x["FileName"].ToString(),
                    FilePath = x["FilePath"].ToString(),
                    test = x["test"].ToString()
                }).ToList();

            // для сериализаии
            Dictionary<string, List<Data.BaseFileInfo>> test = new Dictionary<string, List<Data.BaseFileInfo>>();
            test.Add("PNG", new List<Data.BaseFileInfo>(t1));
            test.Add("JPG", new List<Data.BaseFileInfo>(t2));

            JsonSerializer ser = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter("trrr.json"))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    ser.Serialize(writer, test);
                }
            }
            */

        }

        public ICommand CloseProgram => new RelayCommand(() => Environment.Exit(0));
        public ICommand MinimizedProgram => new RelayCommand(() => {
            var app = System.Windows.Application.Current.Windows[0];
            app.WindowState = WindowState.Minimized;
        });
        public ICommand OpenDir => new RelayCommand(() => GetAllFilesInFolder());
        public ICommand LoadLib => new RelayCommand(() => LoadInLibrary());
        public ICommand SaveAll => new RelayCommand(() => SaveInLibrary());
        public ICommand ExportFileInfo => new RelayCommand(() => System.Windows.MessageBox.Show("File exported"));
        public ICommand CopyFileInfo => new RelayCommand(() => System.Windows.MessageBox.Show("Info copied"));

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
                        var t = new Parser();
                        Files = t.GetLibrary(openFileDialog.FileName);
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
            if (Files == null || Files.Count == 0)
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
                    var allowedExtentions = new List<string>(new[] { ".png", ".jpg", ".pdf" });
                    var files = Directory
                        .GetFiles(folderBrowser.SelectedPath)
                        .Where(f => allowedExtentions.Contains(Path.GetExtension(f)));
                    //.Select(file => new { Name = Path.GetFileName(file), Url = PATH, FileExt = Path.GetExtension(file)});

                    foreach (var s in files)
                    {
                        System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.ExtractAssociatedIcon(Path.GetFullPath(s));
                        switch (Path.GetExtension(s).ToLower())
                        {
                            case ".jpg":
                                FileStream Foto = File.Open(s, FileMode.Open, FileAccess.Read, FileShare.Read); // открыли файл по адресу s для чтения
                                BitmapDecoder decoder = JpegBitmapDecoder.Create(Foto, BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.Default); //"распаковали" снимок и создали объект decoder
                                BitmapMetadata TmpImgEXIF = (BitmapMetadata)decoder.Frames[0].Metadata.Clone(); //считали и сохранили метаданные
                                var o = new JPGInfo();
                                o = o.GetAllJPGInfo(
                                    TmpImgEXIF: TmpImgEXIF,
                                    length: Foto.Length,
                                    filename: Path.GetFileName(s),
                                    fullpath: Path.GetFullPath(s),
                                    path: s
                                );
                                Foto.Close();
                                App.Current.Dispatcher.Invoke((Action)delegate
                                {
                                    o.FileIcon = new BitmapImage(new Uri("Icons/image.png", UriKind.Relative));
                                    this.Files.Add(o);
                                });
                                break;
                            case ".png":
                                //this.Files.Add(new PNGInfo());
                                break;
                            case ".pdf":
                                PdfDocument document = PdfReader.Open(s);
                                var obj = new PDFInfo();
                                obj = obj.GetAllPDFInfo(
                                    document: document,
                                    name: Path.GetFileName(s),
                                    fullpath: Path.GetFullPath(s)
                                );
                                App.Current.Dispatcher.Invoke((Action)delegate {
                                    obj.FileIcon = new BitmapImage(new Uri("Icons/pdf.png", UriKind.Relative));
                                    this.Files.Add(obj);
                                });
                                break;
                        }
                    }
                    OpenDialog = false;
                });
            }
        }
    }

    internal static class IconUtilities
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }
    }
}


