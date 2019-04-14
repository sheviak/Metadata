using Metadata.Data;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace Metadata.Storage
{
    public class Parser
    {
        public ObservableCollection<BaseFileInfo> GetLibrary(string file)
        {
            var Files = new ObservableCollection<BaseFileInfo>();
            if (file != "")
            {
                string json = File.ReadAllText("3333.json");
                JArray jsonArray = JArray.Parse(json);
                JToken jsonArray_Item = jsonArray.First;
                while (jsonArray_Item != null)
                {
                    string type = jsonArray_Item.Value<string>("FileType");
                    switch (type)
                    {
                        case "PDF":
                            Files.Add(new PDFInfo
                            {
                                FileAuthor = jsonArray_Item.Value<string>("FileAuthor"),
                                FileCreationDate = jsonArray_Item.Value<string>("FileCreationDate"),
                                FileCreator = jsonArray_Item.Value<string>("FileCreator"),
                                FileImgSize = null,
                                FileKeywords = jsonArray_Item.Value<string>("FileKeywords"),
                                FileModificationDate = jsonArray_Item.Value<string>("FileModificationDate"),
                                FileName = jsonArray_Item.Value<string>("FileName"),
                                FilePath = jsonArray_Item.Value<string>("FilePath"),
                                FileProducer = jsonArray_Item.Value<string>("FileProducer"),
                                FileIcon = new BitmapImage(new Uri("Icons/pdf.png", UriKind.Relative)),
                                FileReference = jsonArray_Item.Value<string>("FileReference"),
                                FileSize = jsonArray_Item.Value<string>("FileSize"),
                                FileSubject = jsonArray_Item.Value<string>("FileSubject"),
                                FileTitle = jsonArray_Item.Value<string>("FileTitle"),
                                FileType = type
                            });
                            break;
                        case "JPG":
                            Files.Add(new JPGInfo
                            {
                                FileTitle = jsonArray_Item.Value<string>("FileTitle"),
                                FileDate = jsonArray_Item.Value<string>("FileDate"),
                                FileISO = jsonArray_Item.Value<string>("FileISO"),
                                FileWhiteBalance = jsonArray_Item.Value<string>("FileWhiteBalance"),
                                FileFlash = jsonArray_Item.Value<string>("FileFlash"),
                                FileRating = jsonArray_Item.Value<string>("FileRating"),
                                FileFNumber = jsonArray_Item.Value<string>("FileFNumber"),
                                FileSaturation = jsonArray_Item.Value<string>("FileSaturation"),
                                FileColorSpace = jsonArray_Item.Value<string>("FileColorSpace"),
                                FileCameraModel = jsonArray_Item.Value<string>("FileCameraModel"),
                                FileCameraManufacturer = jsonArray_Item.Value<string>("FileCameraManufacturer"),
                                FileComment = jsonArray_Item.Value<string>("FileComment"),
                                FileSize = jsonArray_Item.Value<string>("FileSize"),
                                FileName = jsonArray_Item.Value<string>("FileName"),
                                FilePath = jsonArray_Item.Value<string>("FilePath"),
                                FileFocalLength = jsonArray_Item.Value<string>("FileFocalLength"),
                                FileImgSize = jsonArray_Item.Value<string>("FileImgSize"),
                                FileType = type,
                                FileIcon = new BitmapImage(new Uri("Icons/image.png", UriKind.Relative))
                            });
                            break;
                        case "PNG":
                            break;
                        default:
                            break;
                    }
                    //Be careful, you take the next from the current item, not from the JArray object. 
                    jsonArray_Item = jsonArray_Item.Next;
                }
            }
            return Files;
        }
    }
}
