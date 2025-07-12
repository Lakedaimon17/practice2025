using System;
using System.IO;
using System.Linq;
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

                var outputLines = assembly.GetTypes()
                    .Where(t => t.IsClass)
                    .SelectMany(FormatTypeInfo)
                    .ToList();

                Console.WriteLine(string.Join(Environment.NewLine, outputLines));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки сборки: {ex.Message}");
            }
        }

        private static string[] FormatTypeInfo(Type type)
        {
            var result = new System.Collections.Generic.List<string>
            {
                new string('-', 50),
                $"Класс: {type.FullName}"
            };

            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            var versionAttr = type.GetCustomAttribute<VersionAttribute>();

            if (displayNameAttr is not null)
                result.Add($"Отображаемое имя: {displayNameAttr.DisplayName}");

            if (versionAttr is not null)
                result.Add($"Версия: {versionAttr.Major}.{versionAttr.Minor}");

            result.Add("\nКонструкторы:");
            result.AddRange(type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Select(c => $"- {c.Name}")
                .DefaultIfEmpty("  Нет публичных конструкторов"));

            result.Add("\nМетоды:");
            result.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Select(m => $"- {m.Name}")
                .DefaultIfEmpty("  Нет публичных методов"));

            result.Add("\nСвойства:");
            result.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p => $"- {p.Name}")
                .DefaultIfEmpty("  Нет публичных свойств"));

            result.Add("");
            return result.ToArray();
        }

        public static void PrintTypeInfo(Type type)
        {
            Console.WriteLine(new string('-', 50));
            Console.WriteLine($"Класс: {type.FullName}");

            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr is not null)
                Console.WriteLine($"Отображаемое имя: {displayNameAttr.DisplayName}");

            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr is not null)
                Console.WriteLine($"Версия: {versionAttr.Major}.{versionAttr.Minor}");
        }
    }
}