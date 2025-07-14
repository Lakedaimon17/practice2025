using System;
using System.IO;
using task13;
using Xunit;

public class task13tests
{
    [Fact]
    public void Serialize_Deserialize_ShouldPreserveData()
    {
        var student = new Student
        {
            FirstName = "Иван",
            LastName = "Иванов",
            BirthDate = new DateTime(2000, 1, 1),
            Grades = new List<Subject>
            {
                new Subject { Name = "Математика", Grade = 90 },
                new Subject { Name = "История", Grade = 85 }
            }
        };

        string json = StudentService.Serialize(student);
        var result = StudentService.Deserialize(json);

        Assert.Equal(student.FirstName, result.FirstName);
        Assert.Equal(student.LastName, result.LastName);
        Assert.Equal(student.BirthDate.ToString("yyyy-MM-dd"), result.BirthDate.ToString("yyyy-MM-dd"));
        Assert.Equal(student.Grades.Count, result.Grades.Count);
    }

    [Fact]
    public void Save_Load_File_ShouldWork()
    {
        var tempFilePath = Path.GetTempFileName();
        var student = new Student
        {
            FirstName = "Эрнест",
            LastName = "Резерфорд",
            BirthDate = new DateTime(2001, 2, 2),
            Grades = new List<Subject>
            {
                new Subject { Name = "Физика", Grade = 100 }
            }
        };

        StudentService.SaveToFile(tempFilePath, student);
        var loadedStudent = StudentService.LoadFromFile(tempFilePath);

        Assert.Equal(student.FirstName, loadedStudent.FirstName);
        Assert.Equal(student.LastName, loadedStudent.LastName);
        Assert.Equal(student.BirthDate.Date, loadedStudent.BirthDate.Date);
        Assert.Equal(student.Grades.Count, loadedStudent.Grades.Count);

        File.Delete(tempFilePath); 
    }

    [Fact]
    public void Deserialize_WithEmptyName_ShouldThrow()
    {
        string json = "{\"FirstName\": \"\", \"LastName\": \"Сидоров\", \"BirthDate\": \"2002-03-04\", \"Grades\": []}";

        var exception = Assert.Throws<InvalidDataException>(() => StudentService.Deserialize(json));
        Assert.Contains("Имя студента не может быть пустым", exception.Message);
    }

    [Fact]
    public void Deserialize_WithNoGrades_ShouldThrow()
    {
        string json = "{\"FirstName\": \"Алексей\", \"LastName\": \"Николаев\", \"BirthDate\": \"2003-04-05\", \"Grades\": []}";

        var exception = Assert.Throws<InvalidDataException>(() => StudentService.Deserialize(json));
        Assert.Contains("Список оценок не должен быть пустым", exception.Message);
    }
}
