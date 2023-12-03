using System.Text.RegularExpressions;

namespace DAL
{
    public interface IEntityContext<T> where T : class, new()
    {
        public IProvider<T> Provider { get; set; }
        public string DBName { get; set; }
        public string DBType { get; set; }
        public string DBFile { get; }
        public string[] AvailableDBTypes { get; }
        public void SetProvider(string dbType, string dbName)
        {
        }
        public void SetProvider(string FileName)
        {
        }
    }
    public class EntityContext<T> : IEntityContext<T> where T : class, new()
    {
        public IProvider<T> Provider { get; set; }
        string _DBName = "custom-db";
        public string DBName { get => _DBName; set { _DBName = value ?? throw new ArgumentException(); } }
        public string DBType { get; set; }
        public string DBFile => $"{DBName}.{DBType}";
        public string[] AvailableDBTypes => new string[] { "json", "xml", "bin", "txt" };
        public EntityContext()
        {
            DBType = "json";
            Provider = new JSONProvider<T>(DBFile);
        }
        public void SetProvider(string dbType, string dbName)
        {
            DBType = dbType;
            DBName = dbName;
            switch (dbType)
            {
                case "json":
                    Provider = new JSONProvider<T>(DBFile);
                    break;
                case "xml":
                    Provider = new XMLProvider<T>(DBFile);
                    break;
                case "bin":
                    Provider = new BinaryProvider<T>(DBFile);
                    break;
                case "txt":
                    Provider = new CustomProvider<T>(DBFile);
                    break;
            }
        }
        public void SetProvider(string FileName)
        {
            if (!Regex.IsMatch(FileName, @"^[a-zA-Z0-9_\-\.]+\.[a-zA-Z0-9]+$")) throw new ArgumentException();
            string[] Parts = FileName.Split('.');
            SetProvider(Parts[1], Parts[0]);
        }
    }
}
