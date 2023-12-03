using System.Runtime.Serialization.Formatters.Binary;

namespace DAL
{
#pragma warning disable SYSLIB0011
    public class BinaryProvider<T> : IProvider<T> where T : class
    {
        string FileName = "";
        BinaryFormatter formatter = new();
        public BinaryProvider(string fileName)
        {
            FileName = AppDomain.CurrentDomain.BaseDirectory + fileName;
            using StreamWriter w = File.AppendText(FileName);
        }
        public List<T> Load()
        {
            List<T> deserialised;
            using (FileStream fileStream = new(FileName, FileMode.Open))
            {
                try
                {
                    return deserialised = (List<T>)formatter.Deserialize(fileStream);
                }
                catch (Exception e)
                {
                    return new List<T> { };
                }
            }
        }
        public void Save(List<T> obj)
        {
            using (FileStream fileStream = new(FileName, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fileStream, obj);
            }
        }
    }
}