using System;
using System.Linq;
using System.Reflection;

namespace task07.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; }

        public DisplayNameAttribute(string displayName)
        {
            DisplayName = displayName;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; }
        public int Minor { get; }

        public VersionAttribute(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }
    }
}

namespace task07
{
    [Attributes.DisplayName("Пример класса")]
    [Attributes.Version(1, 0)]
    public class SampleClass
    {
        [Attributes.DisplayName("Числовое свойство")]
        public int Number { get; set; }

        [Attributes.DisplayName("Тестовый метод")]
        public void TestMethod()
        {
        }
    }

    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            var displayNameAttr = type.GetCustomAttribute<Attributes.DisplayNameAttribute>();
            var versionAttr = type.GetCustomAttribute<Attributes.VersionAttribute>();

            Console.WriteLine($"Тип: {type.Name}");

            if (displayNameAttr != null)
            {
                Console.WriteLine($"DisplayName: {displayNameAttr.DisplayName}");
            }

            if (versionAttr != null)
            {
                Console.WriteLine($"Версия: {versionAttr.Major}.{versionAttr.Minor}");
            }

            var properties = type.GetProperties()
                .Select(prop => prop.GetCustomAttribute<Attributes.DisplayNameAttribute>()?
                    .DisplayName ?? prop.Name)
                .ToList();

            Console.WriteLine("Свойства:");
            Console.WriteLine(string.Join("\n", properties.Select(p => $"- {p}")));

            var methods = type.GetMethods()
                .Select(method => method.GetCustomAttribute<Attributes.DisplayNameAttribute>()?
                    .DisplayName ?? method.Name)
                .ToList();

            Console.WriteLine("Методы:");
            Console.WriteLine(string.Join("\n", methods.Select(m => $"- {m}")));
        }
    }
}
