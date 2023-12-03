using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DAL
{
    [JsonDerivedType(typeof(Entity), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(Student), typeDiscriminator: "student")]
    [JsonDerivedType(typeof(Tailor), typeDiscriminator: "tailor")]
    [JsonDerivedType(typeof(Singer), typeDiscriminator: "singer")]
    [Serializable]
    [XmlInclude(typeof(Student))]
    [XmlInclude(typeof(Tailor))]
    [XmlInclude(typeof(Singer))]
    public class Entity
    {
        [XmlElement]
        public string LastName { get; set; }
        public Entity() { }
        [JsonConstructor]
        public Entity(string? lastName)
        {
            LastName = lastName;
        }
        public virtual string[] Methods { get { return ["Nothing"]; } }
        public string Nothing()
        {
            return LastName + " does nothing";
        }
        public override string ToString() => LastName;
    }
    [Serializable]
    public class Student : Entity, IStudy
    {
        public int? Course { get; set; }
        public string StudentID { get; set; }
        public int? GPA { get; set; }
        public string? Country { get; set; }
        public string? ForeignPassportNumber { get; set; }
        public Student() { }
        public Student(string LastNameInput, string StudentIDInput) : base(LastNameInput)
        {
            StudentID = StudentIDInput;
        }
        [JsonConstructor]
        public Student(int? course, string studentID, int? gpa, string? country, string? foreignPassportNumber, string LastName) : base(LastName)
        {
            (StudentID, GPA, Course, Country, ForeignPassportNumber) = (studentID, gpa, course, country, foreignPassportNumber);
        }
        public Student(string LastName, string StudentID, int? Course, int? GPA, string? Country, string? ForeignPassportNumber) :
            this(Course, StudentID, GPA, Country, ForeignPassportNumber, LastName)
        { }
        public string Study()
        {
            Course = Course == 6 ? 1 : Course + 1;
            return LastName + " is now studing in " + Course + " course";
        }
        public override string[] Methods { get { return base.Methods.Union(new string[] { "Study" }).ToArray(); } }
        public override string ToString() =>
            "Student - " + LastName +
            ", StudentID: " + StudentID +
            ", Course: " + Course +
            ", GPA: " + GPA +
            ", Country: " + Country +
            ", ForeignPassportNumber: " + ForeignPassportNumber;
    }
    [Serializable]
    public class Tailor : Entity, IRepair
    {
        public Tailor() { }
        public Tailor(string LastName) : base(LastName)
        {
        }
        public string Repair()
        {
            return LastName + " repaired something for you!";
        }
        public override string[] Methods { get { return base.Methods.Union(new string[] { "Repair" }).ToArray(); } }
        public override string ToString() => "Tailor - " + LastName;
    }
    [Serializable]
    public class Singer : Entity, ISing
    {
        public Singer() { }
        public Singer(string LastName) : base(LastName)
        {
        }
        public string Sing()
        {
            return LastName + " sings for you!";
        }
        public override string[] Methods { get { return base.Methods.Union(new string[] { "Sing" }).ToArray(); } }
        public override string ToString() => "Singer - " + LastName;
    }
}
