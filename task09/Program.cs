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
            var lines = new System.Collections.Generic.List<string>
            {
                new string('-', 50),
                $"Класс: {type.FullName}"
            };

            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr is not null)
                lines.Add($"Отображаемое имя: {displayNameAttr.DisplayName}");

            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr is not null)
                lines.Add($"Версия: {versionAttr.Major}.{versionAttr.Minor}");

            lines.Add("\nКонструкторы:");

            lines.AddRange(type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .SelectMany(ctor => new[]
                {
                    $"- {ctor.Name} ({ctor.DeclaringType?.Name})",
                }.Concat(ctor.GetParameters().Select(p =>
                    $"  Параметр: {p.Name} ({p.ParameterType})"
                ))));

            lines.Add("\nМетоды:");

            lines.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SelectMany(method =>
                {
                    var methodLine = $"- {method.Name} ({method.ReturnType})";

                    var attr = method.GetCustomAttribute<DisplayNameAttribute>();
                    var displayNameLine = attr != null
                        ? new[] { methodLine, $"  Отображаемое имя: {attr.DisplayName}" }
                        : new[] { methodLine };

                    return displayNameLine.Concat(
                        method.GetParameters().Select(p =>
                            $"  Параметр: {p.Name} ({p.ParameterType})"
                        )
                    );
                }));

            lines.Add("\nСвойства:");

            lines.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(property =>
                {
                    var propertyLine = $"- {property.Name} ({property.PropertyType})";

                    var attr = property.GetCustomAttribute<DisplayNameAttribute>();
                    if (attr is null)
                        return propertyLine;

                    return $"{propertyLine}\n  Отображаемое имя: {attr.DisplayName}";
                }));

            lines.Add(string.Empty);

            return lines.ToArray();
        }

        public static void PrintTypeInfo(Type type)
        {
            var output = new System.Collections.Generic.List<string>
            {
                new string('-', 50),
                $"Класс: {type.FullName}"
            };

            var displayNameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr is not null)
                output.Add($"Отображаемое имя: {displayNameAttr.DisplayName}");

            var versionAttr = type.GetCustomAttribute<VersionAttribute>();
            if (versionAttr is not null)
                output.Add($"Версия: {versionAttr.Major}.{versionAttr.Minor}");

            output.Add("\nКонструкторы:");

            output.AddRange(type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .SelectMany(ctor => new[]
                {
                    $"- {ctor.Name} ({ctor.DeclaringType?.Name})",
                }.Concat(ctor.GetParameters().Select(p =>
                    $"  Параметр: {p.Name} ({p.ParameterType})"
                ))));

            output.Add("\nМетоды:");

            output.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .SelectMany(method =>
                {
                    var methodLine = $"- {method.Name} ({method.ReturnType})";

                    var attr = method.GetCustomAttribute<DisplayNameAttribute>();
                    var displayNameLine = attr != null
                        ? new[] { methodLine, $"  Отображаемое имя: {attr.DisplayName}" }
                        : new[] { methodLine };

                    return displayNameLine.Concat(
                        method.GetParameters().Select(p =>
                            $"  Параметр: {p.Name} ({p.ParameterType})"
                        )
                    );
                }));

            output.Add("\nСвойства:");

            output.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(property =>
                {
                    var propertyLine = $"- {property.Name} ({property.PropertyType})";

                    var attr = property.GetCustomAttribute<DisplayNameAttribute>();
                    if (attr is null)
                        return propertyLine;

                    return $"{propertyLine}\n  Отображаемое имя: {attr.DisplayName}";
                }));

            Console.WriteLine(string.Join(Environment.NewLine, output));
        }
    }
}
