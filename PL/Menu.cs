using DAL;
using BLL;
using System.Reflection;
using System.Text;

namespace PL
{
    public class Menu
    {
        string EntityMethodPrefix = "do ";
        EntityService People = new();
        string nextState = "main";
        string? nextStateArg1;
        int? nextStateArg2;
        public void ShowMain()
        {
            Console.Clear();
            Console.WriteLine("Enter:\n" +
            "1 - view all;\n" +
            "2 - view the database of students;\n" +
            "3 - view the database of tailors;\n" +
            "4 - view the database of singers;\n" +
            "5 - add students to the database;\n" +
            "6 - add tailors to the database;\n" +
            "7 - add singers to the database;\n" +
            "8 - search\n" +
            "9 - db settings (name and format)\n" +
            "0 - EXIT");

            bool done = false;
            do
            {
                var input = Console.ReadLine();
                switch (input)
                {
                    case "0": nextState = "finish"; return;
                    case "1": nextState = "list"; nextStateArg1 = null; return;
                    case "2": nextState = "list"; nextStateArg1 = "Student"; return;
                    case "3": nextState = "list"; nextStateArg1 = "Tailor"; return;
                    case "4": nextState = "list"; nextStateArg1 = "Singer"; nextStateArg2 = null; return;
                    case "5": nextState = "add"; nextStateArg1 = "Student"; nextStateArg2 = null; return;
                    case "6": nextState = "add"; nextStateArg1 = "Tailor"; nextStateArg2 = null; return;
                    case "7": nextState = "add"; nextStateArg1 = "Singer"; return;
                    case "8": nextState = "search"; return;
                    case "9": nextState = "db-settings"; return;
                }
            }
            while (!done);

            return;
        }
        public void ShowEntities()
        {
            // nextStateArg1 is class name (Student / Tailor / Singer / null = all)
            Console.Clear();
            Console.WriteLine("Entities: " + People.Length() + " items");
            int currentIndex = 0;
            while (currentIndex < People.Length())
            {
                if (nextStateArg1 == null || People[currentIndex].GetType().Name == nextStateArg1)
                {
                    Console.WriteLine(1 + currentIndex + ". " + People[currentIndex]);
                }
                currentIndex++;
            }
            Console.WriteLine("0 - menu; entity number and enter - view entity");

            bool done = false;
            do
            {
                var input = Console.ReadLine();
                switch (input)
                {
                    case "0": nextState = "main"; return;
                    default:
                        try
                        {
                            int parsed = Convert.ToInt32(input);
                            if (parsed < 1 || parsed > People.Length()) throw new ArgumentException();
                            done = true;
                            nextState = "entity";
                            nextStateArg2 = parsed - 1;
                            return;
                        }
                        catch (Exception) { Console.WriteLine("Wrong option!"); }
                        break;
                }
            }
            while (!done);
        }
        public void ShowEntity()
        {
            // nextStateArg2 is index
            Console.Clear();
            Console.WriteLine("Entity: " + (nextStateArg2 + 1) + " of " + People.Length() + " items");
            int entityIndex = nextStateArg2.GetValueOrDefault();
            Entity entity = People[entityIndex];
            Console.WriteLine(1 + entityIndex + ". " + entity);
            StringBuilder functionOptsString = new("Entity methods ('do <methods>' to use):");
            for (int i = 0; i < entity.Methods.Length; i++)
            {
                functionOptsString.Append(" " + entity.Methods[i]);
            }

            Console.WriteLine(functionOptsString);
            Console.WriteLine("0 - menu; 1 - edit; 2 - delete");

            bool done = false;
            do
            {
                var input = Console.ReadLine();
                if (input != null && input.StartsWith("do "))
                {
                    string functionName = input[EntityMethodPrefix.Length..];
                    Type thisType = entity.GetType();
                    MethodInfo? theMethod = thisType.GetMethod(functionName);
                    if (theMethod == null)
                    {
                        Console.WriteLine("Error: no such function");
                    }
                    else
                    {
                        Console.WriteLine(theMethod.Invoke(entity, new string[] { }));
                        People.Save();
                    }
                }
                else
                {
                    switch (input)
                    {
                        case "0": nextState = "main"; nextStateArg2 = null; return;
                        case "1": nextState = "add"; nextStateArg1 = entity.GetType().Name; return;
                        case "2":
                            try
                            {
                                People.Delete(entityIndex);
                                nextState = "main"; nextStateArg2 = null; return;
                            }
                            catch (Exception) { Console.WriteLine("Failed to delete!"); }
                            break;
                    }
                }
            }
            while (!done);
        }
        public void AddEntity()
        {
            Console.Clear();
            bool isEditing = nextStateArg2 != null;
            int entityIndex = nextStateArg2.GetValueOrDefault();
            Console.WriteLine((isEditing ? "Editing " : "Adding ") + nextStateArg1 + (isEditing ? "#" + nextStateArg2 : ""));
            if (isEditing)
            {
                Console.WriteLine(People[entityIndex]);
            }
            Entity? newEntity = null;
            switch (nextStateArg1)
            {
                case "Student":
                    Student? oldStudent = isEditing ? (Student)People[entityIndex] : null;
                    newEntity = new Student(
                        AskName(isEditing ? oldStudent.LastName : null),
                        AskID(isEditing ? oldStudent.StudentID : null),
                        AskCourse(isEditing ? oldStudent.Course : null),
                        AskGPA(isEditing ? oldStudent.GPA : null),
                        AskCountry(isEditing ? oldStudent.Country : null),
                        AskForeignPassportNumber(isEditing ? oldStudent.ForeignPassportNumber : null)
                    );
                    break;
                case "Tailor":
                    Tailor? oldTailor = isEditing ? (Tailor)People[entityIndex] : null;
                    newEntity = new Tailor(
                        AskName(isEditing ? oldTailor.LastName : null)
                    );
                    break;
                case "Singer":
                    Singer? oldSinger = isEditing ? (Singer)People[entityIndex] : null;
                    newEntity = new Singer(
                        AskName(isEditing ? oldSinger.LastName : null)
                    );
                    break;
                default:
                    Console.WriteLine("I don`t know how to create this entity(\nNew key to go back");
                    Console.ReadKey();
                    break;
            }
            if (newEntity != null)
            {
                if (isEditing)
                {
                    People.Update(newEntity, entityIndex);
                }
                else
                {
                    People.Insert(newEntity);
                }
            }

            nextState = "main"; return;
        }
        public void ShowSearch()
        {
            List<Tuple<int, Entity>> searchList = People.Search();
            Console.Clear();
            Console.WriteLine("Search: " + searchList.Count + " items ");
            int currentIndex = 0;
            while (currentIndex < searchList.Count)
            {
                Console.WriteLine(1 + searchList[currentIndex].Item1 + ". " + searchList[currentIndex].Item2);
                currentIndex++;
            }
            Console.WriteLine("0 - menu");

            bool done = false;
            do
            {
                var input = Console.ReadLine();
                switch (input)
                {
                    case "0": nextState = "main"; return;
                    default:
                        return;
                }
            }
            while (!done);
        }
        public void ShowDBSettings()
        {
            string CurrentDBName = $"{People.DBName}.{People.DBType}";
            Console.Clear();
            Console.WriteLine("Current DB: " + CurrentDBName);
            Console.WriteLine("Available db types: " + string.Join(",", People.AvailableDBTypes));
            Console.WriteLine("Enter new db name: ");
            Console.WriteLine("0 - menu");
            bool done = false;
            do
            {
                var input = Console.ReadLine();
                if (input == null || input == "") { continue; }
                switch (input)
                {
                    case "0": nextState = "main"; return;
                    default:
                        try
                        {
                            People.SetProvider(input);
                            return;
                        }catch (Exception) { Console.WriteLine("Wrong input"); continue; }
                }
            }
            while (!done);
        }
        string AskName(string? suggested)
        {
            bool done = false;
            string name = "";
            do
            {
                Console.WriteLine("Enter name: " + (suggested != null ? "(" + suggested + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "" && suggested != null) { return suggested; }
                try
                {
                    EntityService.ValidateName(stringFromConsole);
                    name = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong name!"); continue; }
                done = true;
            }
            while (!done);
            return name;
        }
        string AskID(string? suggested)
        {
            bool done = false;
            string id = "";
            do
            {
                Console.WriteLine("Enter id (like КВ00000000): " + (suggested != null ? "(" + suggested + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "" && suggested != null) { return suggested; }
                try
                {
                    EntityService.ValidateID(stringFromConsole);
                    id = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong id!"); continue; }
                done = true;
            }
            while (!done);
            return id;
        }
        int? AskCourse(int? suggested)
        {
            bool done = false;
            int course = 0;
            do
            {
                Console.WriteLine("Enter course: " + (suggested != null ? "(" + suggested + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (suggested != null) { return suggested; }
                    return null;
                }
                try
                {
                    int parsed = Convert.ToInt32(stringFromConsole);
                    EntityService.ValidateCourse(parsed);
                    course = parsed;
                }
                catch (Exception) { Console.WriteLine("Wrong course!"); continue; }
                done = true;
            }
            while (!done);
            return course;
        }
        int? AskGPA(int? suggested)
        {
            bool done = false;
            int gpa = 0;
            do
            {
                Console.WriteLine("Enter GPA: " + (suggested != null ? "(" + suggested + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (suggested != null) { return suggested; }
                    return null;
                }
                try
                {
                    int parsed = Convert.ToInt32(stringFromConsole);
                    EntityService.ValidateMark(parsed);
                    gpa = parsed;
                }
                catch (Exception) { Console.WriteLine("Wrong gpa!"); continue; }
                done = true;
            }
            while (!done);
            return gpa;
        }
        string? AskCountry(string? suggested)
        {
            bool done = false;
            string? country = null;
            do
            {
                Console.WriteLine("Enter Country: " + (suggested != null ? "(" + suggested + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (suggested != null) { return suggested; }
                    return null;
                }
                try
                {
                    EntityService.ValidateCountry(stringFromConsole);
                    country = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong Country!"); continue; }
                done = true;
            }
            while (!done);
            return country;
        }
        string? AskForeignPassportNumber(string? suggested)
        {
            bool done = false;
            string? foreignPassportNumber = null;
            do
            {
                Console.WriteLine("Enter Foreign passport number: " + (suggested != null ? "(" + suggested + ")" : ""));
                string? stringFromConsole = Console.ReadLine();
                if (stringFromConsole == null || stringFromConsole == "")
                {
                    if (suggested != null) { return suggested; }
                    return null;
                }
                try
                {
                    EntityService.ValidateForeignPassportNumber(stringFromConsole);
                    foreignPassportNumber = stringFromConsole;
                }
                catch (Exception) { Console.WriteLine("Wrong Foreign passport number!"); continue; }
                done = true;
            }
            while (!done);
            return foreignPassportNumber;
        }
        public void MainMenu()
        {
            while (nextState != "finish")
            {
                switch (nextState)
                {
                    case "main":
                        ShowMain();
                        break;
                    case "list":
                        ShowEntities();
                        break;
                    case "entity":
                        ShowEntity();
                        break;
                    case "add":
                        AddEntity();
                        break;
                    case "search":
                        ShowSearch();
                        break;
                    case "db-settings":
                        ShowDBSettings();
                        break;
                }
            }
        }
    }
}
