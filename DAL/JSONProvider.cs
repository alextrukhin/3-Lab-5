using System.Text.Json;

namespace DAL
{
    public class JSONProvider<T>:IProvider<T> where T : class
    {
        string FileName = "";
        JsonSerializerOptions options = new() { WriteIndented = true };
        public JSONProvider(string fileName)
        {
            FileName = AppDomain.CurrentDomain.BaseDirectory + fileName;
            using StreamWriter w = File.AppendText(FileName);
        }
        public List<T> Load()
        {
            using (FileStream fileStream = new(FileName, FileMode.Open))
            {
                try
                {
                    return (List<T>)JsonSerializer.Deserialize(fileStream, typeof(List<T>), options);
                }catch (System.Text.Json.JsonException ex) {
                    return new List<T>();
                }
            }
        }
        public void Save(List<T> listToSave)
        {
            using (FileStream fileStream = new(FileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fileStream.SetLength(0); // Fixes issue with deleting nodes
                JsonSerializer.Serialize<List<T>>(fileStream, listToSave, options);
            }
        }
    }
}
