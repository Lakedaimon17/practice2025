using System;
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

            Console.WriteLine("Свойства:");
            var propertiesQuery = from prop in type.GetProperties()
                                  let attr = prop.GetCustomAttribute<Attributes.DisplayNameAttribute>()
                                  select attr?.DisplayName ?? prop.Name;

            foreach (var name in propertiesQuery)
            {
                Console.WriteLine($"- {name}");
            }

            Console.WriteLine("Методы:");
            var methodsQuery = from method in type.GetMethods()
                               let attr = method.GetCustomAttribute<Attributes.DisplayNameAttribute>()
                               select attr?.DisplayName ?? method.Name;

            foreach (var name in methodsQuery)
            {
                Console.WriteLine($"- {name}");
            }
        }
    }
}
