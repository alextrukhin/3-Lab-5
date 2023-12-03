using DAL;

namespace BLL.Tests
{
    public class TestProvider<T> : IProvider<T> where T : class
    {
        public List<T> data = new List<T>();
        public TestProvider(string fileName)
        {
        }
        public List<T> Load()
        {
            return data;
        }
        public void Save(List<T> listToSave)
        {
            data = listToSave;
        }
    }
    public class TestEntityContext<T> : EntityContext<T> where T : class, new()
    {
        public TestEntityContext()
        {
            DBType = "test";
            Provider = new TestProvider<T>(DBFile);
        }
        public new void SetProvider(string dbType, string dbName)
        {
            DBType = dbType;
            DBName = dbName;
        }
    }
    [TestClass()]
    public class EntityServiceTests
    {
        EntityService entityService = new(new TestEntityContext<Entity>());
        Student student = new Student("Test student", "ÊÂ12345678");
        [TestMethod()]
        public void InsertTest()
        {
            // Arrange
            int length = entityService.Length();
            // Act
            entityService.Insert(student);
            // Assert
            Assert.AreEqual(length + 1, entityService.Length());
            Assert.AreEqual(entityService[length], student);
            // Clear traces
            entityService.Delete(length);
        }
        [TestMethod()]
        public void UpdateTest()
        {
            // Arrange
            int length = entityService.Length();
            // Act
            entityService.Insert(new Entity());
            entityService.Update(student, length);
            // Assert
            Assert.AreEqual(length + 1, entityService.Length());
            Assert.AreEqual(entityService[length], student);
            // Clear traces
            entityService.Delete(length);
        }
        [TestMethod()]
        public void DeleteTest()
        {
            // Arrange
            int length = entityService.Length();
            // Act
            entityService.Insert(student);
            entityService.Delete(length);
            // Assert
            Assert.AreEqual(length, entityService.Length());
        }
        [TestMethod()]
        public void SetProviderTest()
        {
            // Arrange
            string testProviderName = "test-provider";
            string testProviderType = "json";
            // Act
            entityService.SetProvider(testProviderType, testProviderName);
            // Assert
            Assert.AreEqual(testProviderName + "." + testProviderType, entityService.DBFile);
            Assert.AreEqual(testProviderName, entityService.DBName);
            Assert.AreEqual(testProviderType, entityService.DBType);
        }
        public static bool TestValidator<T>(Action<T?> validator, T[] toTest)
        {
            bool result = false;
            foreach (T test in toTest)
            {
                try
                {
                    validator(test);
                    result = true;
                }
                catch (ArgumentException)
                {
                }
            }
            return result;
        }
        [TestMethod()]
        public void ValidateNameTest()
        {
            // Arrange
            string[] wrongNames = ["test-name", "³ì'ÿ", "test1", "", null];
            string[] correctNames = ["json", "testHere"];
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateName, wrongNames);
            bool correctPassed = TestValidator(EntityService.ValidateName, correctNames);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateIDTest()
        {
            // Arrange
            string[] wrongNames = ["ÊÂ", "123", "KB12312311", "ÊÂ123123123", "ÊÂ123123123", "", null];
            string[] correctNames = ["ÊÂ00000000"];
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateID, wrongNames);
            bool correctPassed = TestValidator(EntityService.ValidateID, correctNames);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateCourseTest()
        {
            // Arrange
            int?[] wrongValues = [-1, 0, 7, null];
            int?[] correctValues = [1, 2, 3, 4, 5];
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateCourse, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateCourse, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateMarkTest()
        {
            // Arrange
            int?[] wrongValues = [-1, 6, null];
            int?[] correctValues = [0, 1, 2, 3, 4, 5];
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateMark, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateMark, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateCountryTest()
        {
            // Arrange
            string[] wrongValues = ["", null, "Earth123"];
            string[] correctValues = ["Ukraine", "USA", "United States"];
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateCountry, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateCountry, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateForeignPassportNumberTest()
        {
            // Arrange
            string[] wrongValues = ["", null];
            string[] correctValues = ["123456", "ET76211"];
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateForeignPassportNumber, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateForeignPassportNumber, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void SearchTest()
        {
            // Arrange
            EntityService entityServiceForSearch = new(new TestEntityContext<Entity>());
            Student[] students = [
                new Student("lastname1", "ÊÂ12345678", 1, 5, "-", "123"),
                new Student("lastname2", "ÊÂ12345678", 2, 5, "-", "123"),
                new Student("lastname3", "ÊÂ12345678", 2, 3, "-", "123"),
                new Student("lastname4", "ÊÂ12345678", 1, 5, "-", "123")
                ];
            foreach (Student student in students)
            {
                entityServiceForSearch.Insert(student);
            }
            // Act
            List<Tuple<int, Entity>> results = entityServiceForSearch.Search((Entity input) =>
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
            // Assert
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(0, results[0].Item1);
            Assert.AreEqual(students[0], results[0].Item2);
            Assert.AreEqual(3, results[1].Item1);
            Assert.AreEqual(students[3], results[1].Item2);
        }
        [TestMethod()]
        public void EntityServiceTest()
        {
            // Arrange
            // Act
            entityService.Insert(student);
            // Assert
            Assert.AreEqual(student, entityService[entityService.Length() - 1]);
        }
    }
}
