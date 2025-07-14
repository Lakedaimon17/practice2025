using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13
{
    public class Subject
    {
        public string Name { get; set; }
        public int Grade { get; set; }
    }

    public class Student
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public List<Subject> Grades { get; set; }
    }

    public class DateOnlyJsonConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString()!, DateFormat, null);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }

    public static class StudentService
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new DateOnlyJsonConverter() }
        };

        public static string Serialize(Student student)
        {
            return JsonSerializer.Serialize(student, Options);
        }

        public static Student Deserialize(string json)
        {
            var student = JsonSerializer.Deserialize<Student>(json, Options);

            if (student == null)
                throw new InvalidDataException("Ошибка десериализации.");

            if (string.IsNullOrWhiteSpace(student.FirstName))
                throw new InvalidDataException("Имя студента не может быть пустым.");

            if (student.Grades == null || student.Grades.Count == 0)
                throw new InvalidDataException("Список оценок не должен быть пустым.");

            return student;
        }

        public static void SaveToFile(string path, Student student)
        {
            var json = Serialize(student);
            File.WriteAllText(path, json);
        }

        public static Student LoadFromFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Файл не найден.", path);

            var json = File.ReadAllText(path);
            return Deserialize(json);
        }
    }
}
