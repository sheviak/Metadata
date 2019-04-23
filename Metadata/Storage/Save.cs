using Metadata.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Metadata.Storage
{
    public class Save
    {
        public void Serialize(ObservableCollection<BaseFileInfo> files, string path)
        {
            JsonSerializer ser = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            {
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    ser.Serialize(writer, files);
                }
            }
        }

        public void SaveInformation(Dictionary<string, string> item, string path)
        {
            StreamWriter sw = new StreamWriter(path);
            try
            {
                foreach (var t in item)
                {
                    sw.WriteLine(t.Key + "\t\t" + t.Value);
                }
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message, "Exception");
            }
            finally
            {
                sw.Close();
            }
        }
    }
}
