using System;
using System.IO;
using System.Reflection;
using SampleLibrary;

namespace task09
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к DLL-файлу.");
                return;
            }

            string dllPath = args[0];

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Файл не найден: {dllPath}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"Анализируем сборку: {assembly.FullName}\n");

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass)
                    {
                        PrintTypeInfo(type);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сборки: {ex.Message}");
            }
        }

        public static void PrintTypeInfo(Type type)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Класс: {type.FullName}");

            PrintDisplayNameAttribute(type);
            PrintVersionAttribute(type);

            Console.WriteLine("\nКонструкторы:");
            foreach (var ctor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public))
            {
                PrintConstructor(ctor);
            }

            Console.WriteLine("\nМетоды:");
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            {
                PrintMethod(method);
            }

            Console.WriteLine("\nСвойства:");
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                PrintProperty(prop);
            }

            Console.WriteLine();
        }

        private static void PrintDisplayNameAttribute(Type type)
        {
            var attr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                Console.WriteLine($"Отображаемое имя: {attr.DisplayName}");
        }

        private static void PrintVersionAttribute(Type type)
        {
            var attr = type.GetCustomAttribute<VersionAttribute>();
            if (attr != null)
                Console.WriteLine($"Версия: {attr.Major}.{attr.Minor}");
        }

        private static void PrintConstructor(ConstructorInfo ctor)
        {
            Console.WriteLine($"- {ctor.Name} ({ctor.DeclaringType?.Name})");
            foreach (var param in ctor.GetParameters())
            {
                Console.WriteLine($"  Параметр: {param.Name} ({param.ParameterType})");
            }
        }

        private static void PrintMethod(MethodInfo method)
        {
            Console.WriteLine($"- {method.Name} ({method.ReturnType})");
            var attr = method.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                Console.WriteLine($"  Отображаемое имя: {attr.DisplayName}");
            foreach (var param in method.GetParameters())
            {
                Console.WriteLine($"  Параметр: {param.Name} ({param.ParameterType})");
            }
        }

        private static void PrintProperty(PropertyInfo property)
        {
            Console.WriteLine($"- {property.Name} ({property.PropertyType})");
            var attr = property.GetCustomAttribute<DisplayNameAttribute>();
            if (attr != null)
                Console.WriteLine($"  Отображаемое имя: {attr.DisplayName}");
        }
    }
}
