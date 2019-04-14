using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Media.Imaging;

namespace Metadata.Data
{
    public abstract class BaseFileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileSize { get; set; }
        public string FileImgSize { get; set; }
        public string FileType { get; set; }
        [JsonIgnore] public BitmapImage FileIcon { get; set; }

        public BaseFileInfo() { }
        public BaseFileInfo(string name, string path, string size, string imgSize, string type)
        {
            FileName = name;
            FilePath = path;
            FileSize = size;
            FileType = type;
            FileImgSize = imgSize;
        }

        /// <summary>
        /// Метод возвращающий - Разрешение изображения в пикселях
        /// </summary>
        /// <param name="TmpImgEXIF">Путь к файлу</param>
        /// <returns></returns>
        public string GetImageSize(string path)
        {
            var img = Image.FromFile(path);
            var res = img.Width + " x " + img.Height;
            img.Dispose();
            return res;
        }

        public string BytesToString(long byteCount)
        {
            string[] suf = { "байт", "Кб", "Мб", "Гб" };
            if (byteCount == 0)
                return "0" + suf[0];
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
        }

        public string Decode(string text)
        {
            Encoding utf8 = Encoding.GetEncoding("UTF-8");
            Encoding win1251 = Encoding.GetEncoding("Windows-1251");
            byte[] utf8Bytes = win1251.GetBytes(text);
            byte[] win1251Bytes = Encoding.Convert(utf8, win1251, utf8Bytes);
            return (win1251.GetString(win1251Bytes));
        }

        public abstract Dictionary<string, string> GetInformation();
    }
}
