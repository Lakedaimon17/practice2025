using System.Reflection;
using System.IO;
using Xunit;
using task07;
using task07.Attributes;

namespace task07tests
{
    public class AttributeReflectionTests
    {
        [Fact]
        public void Class_HasDisplayNameAttribute()
        {
            var type = typeof(SampleClass);
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attribute);
            Assert.Equal("Пример класса", attribute.DisplayName);
        }

        [Fact]
        public void Method_HasDisplayNameAttribute()
        {
            var method = typeof(SampleClass).GetMethod("TestMethod");
            var attribute = method.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attribute);
            Assert.Equal("Тестовый метод", attribute.DisplayName);
        }

        [Fact]
        public void Property_HasDisplayNameAttribute()
        {
            var prop = typeof(SampleClass).GetProperty("Number");
            var attribute = prop.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(attribute);
            Assert.Equal("Числовое свойство", attribute.DisplayName);
        }

        [Fact]
        public void Class_HasVersionAttribute()
        {
            var type = typeof(SampleClass);
            var attribute = type.GetCustomAttribute<VersionAttribute>();
            Assert.NotNull(attribute);
            Assert.Equal(1, attribute.Major);
            Assert.Equal(0, attribute.Minor);
        }
    }
    public class ReflectionHelperTests
    {
        [Fact]
        public void PrintTypeInfo_WritesExpectedOutputToConsole()
        {
            var output = new StringWriter();
            Console.SetOut(output);

            var type = typeof(SampleClass);

            ReflectionHelper.PrintTypeInfo(type);

            var consoleOutput = output.ToString();
            
            Assert.Contains("Тип: SampleClass", consoleOutput);
            Assert.Contains("DisplayName: Пример класса", consoleOutput);
            Assert.Contains("Версия: 1.0", consoleOutput);
            Assert.Contains("Свойства:", consoleOutput);
            Assert.Contains("- Числовое свойство", consoleOutput);
            Assert.Contains("Методы:", consoleOutput);
            Assert.Contains("- Тестовый метод", consoleOutput);
        }
    }
}
