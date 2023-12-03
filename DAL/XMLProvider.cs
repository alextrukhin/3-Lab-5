using System.Xml;
using System.Xml.Serialization;

namespace DAL
{
    public class XMLProvider<T> : IProvider<T> where T : class
    {
        string FileName = "";
        readonly XmlSerializer serializer = new(typeof(List<T>));
        public XMLProvider(string fileName)
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
                    return (List<T>)serializer.Deserialize(fileStream);
                }catch (Exception e)
                {
                    return new List<T> { };
                }
            }
        }
        public void Save(List<T> listToSave)
        {
            using (FileStream fileStream = new(FileName, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fileStream, listToSave);
            }
        }
    }
}
