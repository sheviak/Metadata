using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;

namespace Metadata.Data
{
    public class PNGInfo : BaseFileInfo
    {
        public string FileDateCreated { get; set; }
        public string FileDateModification { get; set; }
        public string RedX { get; set; }
        public string RedY { get; set; }
        public string GreenX { get; set; }
        public string GreenY { get; set; }
        public string BlueX { get; set; }
        public string BlueY { get; set; }
        public string WhitePointX { get; set; }
        public string WhitePointY { get; set; }

        public PNGInfo() { }

        public override Dictionary<string, string> GetInformation()
        {
            return new Dictionary<string, string>
            {
                { "File name", FileName },
                { "Image size", FileImgSize },
                { "Date created", FileDateCreated },
                { "Date modification", FileDateModification },
                { "Size", FileSize },
                { "Type", FileType },
                { "Path", FilePath },
                { "Red X", RedX },
                { "Red Y", RedY },
                { "Green X", GreenX },
                { "Green Y", GreenY },
                { "Blue X", BlueX },
                { "Blue Y", BlueY },
                { "White point X", WhitePointX },
                { "White point Y", WhitePointY }
            };
        }

        public PNGInfo GetObject(BitmapMetadata TmpImgEXIF, long length, string filename, string fullpath, string path)
        {
            return new PNGInfo
            {
                FileDateCreated = GetFileCreated(path),
                FileDateModification = GetFileModification(path),
                FileSize = this.BytesToString(length),
                FileName = filename,
                FilePath = fullpath,
                FileImgSize = GetImageSize(path),
                FileType = "PNG",
                RedX = GetRedX(TmpImgEXIF),
                RedY = GetRedY(TmpImgEXIF),
                GreenX = GetGreenX(TmpImgEXIF),
                GreenY = GetGreenY(TmpImgEXIF),
                BlueX = GetBlueX(TmpImgEXIF),
                BlueY = GetBlueY(TmpImgEXIF),
                WhitePointX = GetWhitePointX(TmpImgEXIF),
                WhitePointY = GetWhitePointY(TmpImgEXIF)
            };
        }

        public string GetFileCreated(string path) => File.GetCreationTime(path).ToString();
        public string GetFileModification(string path) => File.GetLastWriteTime(path).ToString();

        public string GetRedX(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/RedX").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetRedY(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/RedY").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetGreenX(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/GreenX").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetGreenY(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/GreenY").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetBlueX(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/BlueX").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetBlueY(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/BlueY").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetWhitePointX(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/WhitePointX").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string GetWhitePointY(BitmapMetadata TmpImgEXIF)
        {
            try
            {
                var text = TmpImgEXIF.GetQuery("/cHRM/WhitePointY").ToString();
                return text;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
