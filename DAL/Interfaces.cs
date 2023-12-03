namespace DAL
{
    public interface IProvider<T> where T : class
    {
        public List<T> Load();
        public void Save(List<T> listToSave);
    }
    interface IStudy
    {
        public string Study();
    }
    interface ISing
    {
        public string Sing();
    }
    interface IRepair
    {
        public string Repair();
    }
}