using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;  
using System.Windows.Media.Imaging;

namespace Metadata.Data
{
    public class JPGInfo : BaseFileInfo
    {
        private ASCIIEncoding ascii = new ASCIIEncoding();

        public string FileTitle { get; set; }
        public string FileDate { get; set; }
        public string FileISO { get; set; }
        public string FileWhiteBalance { get; set; }
        public string FileFlash { get; set; }
        public string FileRating { get; set; }
        public string FileFNumber { get; set; }
        public string FileSaturation { get; set; }
        public string FileColorSpace { get; set; }
        public string FileFocalLength { get; set; }
        public string FileCameraModel { get; set; }
        public string FileCameraManufacturer { get; set; }
        public string FileComment { get; set; }

        public JPGInfo() { }

        public override Dictionary<string, string> GetInformation()
        {
            return new Dictionary<string, string>
            {
                { "File name:",  FileName },
                { "Title", FileTitle },
                { "Date created", FileDate },
                { "ISO", FileISO },
                { "White balance", FileWhiteBalance },
                { "Flash", FileFlash },
                { "Rating", FileRating },
                { "F number", FileFNumber },
                { "Saturation", FileSaturation },
                { "Color space", FileColorSpace },
                { "Focal length", FileFocalLength },
                { "Camera model", FileCameraModel },
                { "Camera manufacturer", FileCameraManufacturer },
                { "Comment", FileComment },
                { "Image size", FileImgSize },
                { "Size", FileSize },
                { "Path", FilePath },
                { "Type", FileType }
            };
        }

        public JPGInfo GetAllJPGInfo(BitmapMetadata TmpImgEXIF, long length, string filename, string fullpath, string path)
        {
            return new JPGInfo
            {
                FileTitle = this.GetTitle(TmpImgEXIF),
                FileDate = this.GetDateTimeOriginal(TmpImgEXIF),
                FileISO = this.GetISO(TmpImgEXIF),
                FileWhiteBalance = this.GetWhiteBalance(TmpImgEXIF),
                FileFlash = this.GetFash(TmpImgEXIF),
                FileRating = this.GetRating(TmpImgEXIF),
                FileFNumber = this.GetFNumber(TmpImgEXIF),
                FileSaturation = this.GetSaturation(TmpImgEXIF),
                FileColorSpace = this.GetColorSpace(TmpImgEXIF),
                FileCameraModel = this.GetCameraModel(TmpImgEXIF),
                FileCameraManufacturer = this.GetCameraManufacturer(TmpImgEXIF),
                FileComment = this.GetComment(TmpImgEXIF),
                FileSize = this.BytesToString(length), 
                FileName = filename,
                FilePath = fullpath,
                FileFocalLength = this.GetFocalLength(path),
                FileImgSize = GetImageSize(path),
                FileType = "JPG"
            };
        }

        /// <summary>
        /// Метод возвращающий из пункта "Описание" - Название
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetTitle(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/app1/ifd/{uint=270}").ToString();
                text = Decode(text);
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Дату сьёмки
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetDateTimeOriginal(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                return TmpImgEXIF.GetQuery("/app1/ifd/exif/{uint=36867}").ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// Метод возвращающий - ISO
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetISO(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                return TmpImgEXIF.GetQuery("/app1/ifd/exif/{uint=34855}").ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Баланс белого
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetWhiteBalance(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                int b = Convert.ToInt32(TmpImgEXIF.GetQuery("/app1/ifd/exif/{ushort=41987}"));
                // баланс белого -> 0 - это "авто", 1 - ручной баланс белого
                if (b == 0) return "Авто";
                else return "Ручной баланс белого";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Была ли задействова вспышка
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetFash(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                int flash = Convert.ToInt32(TmpImgEXIF.GetQuery("/app1/ifd/exif/{ushort=37385}"));
                if (flash == 0) return "без вспышки";
                else return "со вспышкой";
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Рейтинг фотографии
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetRating(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                int flash = Convert.ToInt32(TmpImgEXIF.GetQuery("/app1/ifd/{ushort=18249}"));
                // rating фотографии 1 звезда = 1 , 2звезды = 25, 3 - 50, 4 - 75, 5 - 100
                string val = "";
                switch (flash)
                {
                    case 1: val = "1/5"; break;
                    case 25: val = "2/5"; break;
                    case 50: val = "3/5"; break;
                    case 75: val = "4/5"; break;
                    case 100: val = "5/5"; break;
                }
                return val;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Значение диафрагмы
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns>Значение диафрагмы || пустота</returns>
        public string GetFNumber(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var t = TmpImgEXIF.GetQuery("/app1/ifd/exif/{ushort=33437}");
                var mss = ObjectToByteArray(t);
                string pv0 = Convert.ToInt32(mss[50]).ToString();
                if (pv0.Length == 1) return "1." + pv0;
                if (pv0.Length == 3) pv0 = pv0.Substring(0, 2);
                if (pv0.Length == 2 && !pv0.StartsWith("1"))
                    pv0 = pv0.Substring(0, pv0.Length - 1) + "." + pv0.Substring(pv0.Length - 1, 1);
                pv0 = pv0.Replace(".0", "");
                return "f/" + pv0;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Насыщенность
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetSaturation(BitmapMetadata TmpImgEXIF)
        {
            //насыщенность
            //0 = Normal
            //1 = Low saturation
            //2 = High saturation
            //Other = reserved 
            try
            {
                int t = Convert.ToInt32(TmpImgEXIF.GetQuery("/app1/ifd/exif/{ushort=41993}")); // насыщенность 
                string rezult = "";
                switch (t)
                {
                    case 0:
                        rezult = "Обычная насыщенность";
                        break;
                    case 1:
                        rezult = "Низкая насыщенность";
                        break;
                    case 2:
                        rezult = "Высокая насыщенность";
                        break;
                }
                return rezult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Цветовое пространство
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetColorSpace(BitmapMetadata TmpImgEXIF)
        {
            //ПРЕДСТАВЛЕНИЯ ЦВЕТА
            //1 = sRGB
            //FFFF.H = Uncalibrated
            //Other = reserved
            try
            {
                int t = Convert.ToInt32(TmpImgEXIF.GetQuery("/app1/ifd/exif/{ushort=40961}"));
                string rezult = "";
                switch (t)
                {
                    case 1:
                        rezult = "sRGB";
                        break;
                    case 65535:
                        rezult = "Некалиброванный";
                        break;
                }
                return rezult;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Фокусное расстояние
        /// </summary>
        /// <param name="TmpImgEXIF">Путь к файлу</param>
        /// <returns></returns>
        public string GetFocalLength(string path)
        {
            Image image = Image.FromFile(path);
            try
            {
                PropertyItem propItem = image.GetPropertyItem(37386);
                string pv0 = BitConverter.ToInt16(propItem.Value, 0).ToString();
                string a = Math.Floor(Convert.ToDouble(pv0) / 10).ToString();
                if (a.Length == 2)
                    return a.Insert(1, ".");
                return a;
            }
            catch
            {
                return "";
            }
            finally
            {
                image.Dispose();
            }
        }
        

        /// <summary>
        /// Метод возвращающий - Модель камеры
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetCameraModel(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                return TmpImgEXIF.CameraModel;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Камера изготовитель
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetCameraManufacturer(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                return TmpImgEXIF.CameraManufacturer;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Метод возвращающий - Комментарий к фото
        /// </summary>
        /// <param name="TmpImgEXIF">Метаданные типа BitmapMetadata</param>
        /// <returns></returns>
        public string GetComment(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                return TmpImgEXIF.Comment;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Метод конвертации объекта в массив байт
        /// </summary>
        /// <param name="obj">Объект</param>
        /// <returns></returns>
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null) return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}
