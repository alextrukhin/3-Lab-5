using DAL;
using System.Text.RegularExpressions;

namespace BLL
{
    public class EntityService
    {
        List<Entity> data = new();
        public IEntityContext<Entity> db = new EntityContext<Entity>();
        public EntityService()
        {
            data = db.Provider.Load();
        }
        public EntityService(IEntityContext<Entity> customDB)
        {
            db = customDB;
            data = db.Provider.Load();
        }
        public void Insert(Entity input)
        {
            data.Add(input);
            db.Provider.Save(data);
        }
        public void Update(Entity input, int index)
        {
            data[index] = input;
            db.Provider.Save(data);
        }
        public void Delete(int index)
        {
            data.RemoveAt(index);
            db.Provider.Save(data);
        }
        public void Load() => db.Provider.Load();
        public void Save() => db.Provider.Save(data);
        public int Length() => data.Count;
        public string DBName => db.DBName;
        public string DBType => db.DBType;
        public string DBFile => db.DBFile;
        public void SetProvider(string dbType, string dbName) { db.SetProvider(dbType, dbName); data = db.Provider.Load(); }
        public void SetProvider(string FileName) { db.SetProvider(FileName); data = db.Provider.Load(); }
        public string[] AvailableDBTypes => db.AvailableDBTypes;
        public static void ValidateName(string? name)
        {
            if (name == null || !Regex.Match(name, @"^\p{L}{1,32}$", RegexOptions.IgnoreCase).Success) throw new ArgumentException();
        }
        public static void ValidateID(string? id)
        {
            if (id == null || !Regex.Match(id, @"^КВ\d{8}$", RegexOptions.IgnoreCase).Success) throw new ArgumentException();
        }
        public static void ValidateCourse(int? course)
        {
            if (course == null || course < 1 || course > 6) throw new ArgumentException();
        }
        public static void ValidateMark(int? mark)
        {
            if (mark == null || mark < 0 || mark > 5) throw new ArgumentException();
        }
        public static void ValidateCountry(string? country)
        {
            if (country == null || !Regex.Match(country, @"^(\p{L}| |-){1,32}$", RegexOptions.IgnoreCase).Success) throw new ArgumentException();
        }
        public static void ValidateForeignPassportNumber(string? number)
        {
            if (number == null || !Regex.Match(number, @"^(\p{L}|-|\d){1,32}$", RegexOptions.IgnoreCase).Success) throw new ArgumentException();
        }
        public List<Tuple<int, Entity>> Search(Func<Entity, bool> filterFunction)
        {
            List<Tuple<int, Entity>> Entities = new();
            if (data.Count == 0) { return Entities; }
            for (int i = 0; i < data.Count; i++)
            {
                Entity cur = data[i];
                if (filterFunction(cur)) Entities.Add(Tuple.Create(i, cur));
            }
            return Entities;
        }

        public List<Tuple<int, Entity>> Search()
        {
            return Search((Entity input) =>
            {
                if (input is Student)
                {
                    Student student = input as Student;
                    if (student is not null)
                    {
                        if (student.ForeignPassportNumber != null && student.Course == 1 && student.GPA == 5) return true;
                    }
                }
                return false;
            });
        }

        public Entity this[int position]
        {
            get => data[position];
            set => data[position] = value;
        }
    }
}
