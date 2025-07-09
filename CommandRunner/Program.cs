using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner;

class Program
{
    static void Main(string[] args)
    {
        string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

        if (!File.Exists(dllPath))
        {
            Console.WriteLine("DLL not found.");
            return;
        }

        Assembly assembly = Assembly.LoadFrom(dllPath);

        var commandTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract);

        foreach (var type in commandTypes)
        {
            try
            {
                if (type.GetConstructor(Type.EmptyTypes) != null)
                {
                    var command = (ICommand)Activator.CreateInstance(type);
                    Console.WriteLine($"\nExecuting command: {type.Name}");
                    command.Execute();
                }
                else if (type == typeof(DirectorySizeCommand))
                {
                    var command = (ICommand)Activator.CreateInstance(type, new object[] { "." });
                    Console.WriteLine($"\nExecuting command: {type.Name}");
                    command.Execute();
                }
                else if (type == typeof(FindFilesCommand))
                {
                    var command = (ICommand)Activator.CreateInstance(type, new object[] { ".", "*.cs" });
                    Console.WriteLine($"\nExecuting command: {type.Name}");
                    command.Execute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating instance of {type.Name}: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}
