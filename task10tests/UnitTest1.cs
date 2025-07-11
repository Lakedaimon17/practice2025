using System;
using System.IO;
using System.Reflection;
using task10lib;
using Xunit;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

public class PluginTests
{
    private readonly string _testDllPath;

    public PluginTests()
    {
        var testOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "TestPlugins");

        if (Directory.Exists(testOutputDir))
            Directory.Delete(testOutputDir, true);

        Directory.CreateDirectory(testOutputDir);

        _testDllPath = Path.Combine(testOutputDir, "TestPluginLibrary.dll");

        CompileTestAssembly(_testDllPath);
    }

    [Fact]
    public void Assembly_Loads_Correctly()
    {
        var assembly = LoadAssemblyFromFile(_testDllPath);
        Assert.NotNull(assembly);
    }

    [Fact]
    public void Contains_PluginClass_With_Attribute()
    {
        var assembly = LoadAssemblyFromFile(_testDllPath);

        var pluginType = assembly.GetType("TestPluginLibrary.TestPlugin");
        Assert.NotNull(pluginType);

        var attr = pluginType.GetCustomAttribute<PluginLoadAttribute>();
        Assert.NotNull(attr);

        Assert.Single(attr.Dependencies);
        Assert.Equal("DependencyPlugin.DependencyPlugin", attr.Dependencies[0]);
    }

    private Assembly LoadAssemblyFromFile(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return Assembly.Load(bytes);
    }

    private void CompileTestAssembly(string outputPath)
    {
        const string code = @"
using System;
using task10lib;

namespace DependencyPlugin
{
    public class DependencyPlugin : IPluginCommand
    {
        public void Execute() => Console.WriteLine(""Зависимость выполнена"");
    }
}

namespace TestPluginLibrary
{
    [PluginLoad(""DependencyPlugin.DependencyPlugin"")]
    public class TestPlugin : IPluginCommand
    {
        public void Execute() => Console.WriteLine(""Тестовый плагин выполнен"");
    }
}";

        var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")),
            MetadataReference.CreateFromFile("task10lib.dll")
        };

        var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
            Path.GetFileNameWithoutExtension(outputPath),
            new[] { syntaxTree },
            references,
            new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
            throw new Exception("Ошибка компиляции:\n" + string.Join("\n", result.Diagnostics));

        File.WriteAllBytes(outputPath, ms.ToArray());
    }
}
