using Metadata.Data;
using Newtonsoft.Json;
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
    }
}
