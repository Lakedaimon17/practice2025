using System;
using System.IO;
using System.Reflection;
using Xunit;
using task09;

namespace task09tests
{
    public class MetadataExplorerTests
    {
        private readonly TextWriter originalConsoleOut;

        public MetadataExplorerTests()
        {
            originalConsoleOut = Console.Out;
        }

        [Fact]
        public void PrintTypeInfo_ShouldOutputClassMetadataCorrectly()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var sampleDllPath = GetSampleLibraryDllPath();
            var assembly = Assembly.LoadFrom(sampleDllPath);
            var type = assembly.GetType("SampleLibrary.SampleClass");

            Program.PrintTypeInfo(type);

            var output = writer.ToString();

            Assert.Contains("Класс: SampleLibrary.SampleClass", output);
            Assert.Contains("Отображаемое имя: Пример класса", output);
            Assert.Contains("Версия: 1.0", output);
            Assert.Contains("Параметр: name (System.String)", output);
            Assert.Contains("Отображаемое имя: Тестовый метод", output);

            Console.SetOut(originalConsoleOut);
        }

        [Fact]
        public void Main_WithValidDllPath_ShouldPrintMetadata()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var sampleDllPath = GetSampleLibraryDllPath();
            var tempDllCopy = Path.Combine(Path.GetTempPath(), "SampleLibrary.dll");

            File.Copy(sampleDllPath, tempDllCopy, overwrite: true);

            Program.Main(new[] { tempDllCopy });

            var output = writer.ToString();
            Assert.Contains("Анализируем сборку: SampleLibrary,", output);
            Assert.Contains("Пример класса", output);
            Assert.Contains("Тестовый метод", output);

            Console.SetOut(originalConsoleOut);
        }

        [Fact]
        public void Main_WithInvalidDllPath_ShouldPrintError()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            Program.Main(new[] { "nonexistent.dll" });

            var output = writer.ToString();
            Assert.Contains("Файл не найден: nonexistent.dll", output);

            Console.SetOut(originalConsoleOut);
        }

        [Fact]
        public void Main_WithNoArguments_ShouldPromptForDllPath()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            Program.Main(Array.Empty<string>());

            var output = writer.ToString();
            Assert.Contains("Укажите путь к DLL-файлу.", output);

            Console.SetOut(originalConsoleOut);
        }

        private string GetSampleLibraryDllPath()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var solutionRoot = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName;

            string configuration = Environment.GetEnvironmentVariable("CONFIGURATION") ?? "Release";

            var dllPath = Path.Combine(solutionRoot, "SampleLibrary", "bin", configuration, "net9.0", "SampleLibrary.dll");

            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException($"DLL не найдена: {dllPath}");
            }

            return dllPath;
        }

        [Fact]
        public void PrintTypeInfo_ShouldOutputPropertyMetadataCorrectly()
        {
            var writer = new StringWriter();
            Console.SetOut(writer);

            var sampleDllPath = GetSampleLibraryDllPath();
            var assembly = Assembly.LoadFrom(sampleDllPath);
            var type = assembly.GetType("SampleLibrary.SampleClass");

            Program.PrintTypeInfo(type);

            var output = writer.ToString();

            Assert.Contains("Свойства:", output);
            Assert.Contains("Числовое свойство", output);
            Assert.Contains("Number (System.Int32)", output);

            Console.SetOut(originalConsoleOut);
        }
    }
}
