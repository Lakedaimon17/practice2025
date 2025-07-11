using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using task10lib;

namespace PluginLoaderApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к директории с плагинами.");
                return;
            }

            string path = args[0];

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Директория {path} не найдена.");
                return;
            }

            var loader = new PluginLoader(path);
            loader.LoadAndRunPlugins();
        }
    }

    public class PluginLoader
    {
        private readonly string _pluginsDirectory;

        public PluginLoader(string pluginsDirectory)
        {
            _pluginsDirectory = pluginsDirectory;
        }

        public void LoadAndRunPlugins()
        {
            var dllFiles = Directory.GetFiles(_pluginsDirectory, "*.dll");

            if (dllFiles.Length == 0)
            {
                Console.WriteLine("Плагины (.dll) не найдены.");
                return;
            }

            var pluginInfos = new Dictionary<string, PluginInfo>();

            foreach (var dll in dllFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(dll);
                    foreach (var type in assembly.GetTypes())
                    {
                        var attr = type.GetCustomAttribute<PluginLoadAttribute>();
                        if (attr != null &&
                            typeof(IPluginCommand).IsAssignableFrom(type) &&
                            type.GetConstructor(Type.EmptyTypes) != null)
                        {
                            var pluginName = type.FullName;
                            pluginInfos[pluginName] = new PluginInfo
                            {
                                TypeName = pluginName,
                                Type = type,
                                Dependencies = attr.Dependencies.ToList(),
                                AssemblyPath = dll
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке {dll}: {ex.Message}");
                }
            }

            if (pluginInfos.Count == 0)
            {
                Console.WriteLine("Не найдено подходящих плагинов.");
                return;
            }

            var orderedPlugins = TopologicalSort(pluginInfos.Values.ToList());

            foreach (var plugin in orderedPlugins)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(plugin.AssemblyPath);
                    var pluginType = assembly.GetType(plugin.TypeName);
                    var instance = Activator.CreateInstance(pluginType);
                    var executeMethod = pluginType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.Public);
                    executeMethod.Invoke(instance, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка выполнения плагина {plugin.TypeName}: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }

        private class PluginInfo
        {
            public string TypeName { get; set; }
            public Type Type { get; set; }
            public List<string> Dependencies { get; set; }
            public string AssemblyPath { get; set; }
        }

        private static List<PluginInfo> TopologicalSort(List<PluginInfo> plugins)
        {
            var visited = new HashSet<string>();
            var result = new List<PluginInfo>();
            var visiting = new HashSet<string>();
            var pluginMap = plugins.ToDictionary(p => p.TypeName);

            foreach (var plugin in plugins)
            {
                Visit(plugin, pluginMap, visited, visiting, result);
            }

            return result;
        }

        private static void Visit(
            PluginInfo plugin,
            Dictionary<string, PluginInfo> pluginMap,
            HashSet<string> visited,
            HashSet<string> visiting,
            List<PluginInfo> result)
        {
            if (visiting.Contains(plugin.TypeName))
                throw new InvalidOperationException($"Циклическая зависимость: {plugin.TypeName}");

            if (visited.Contains(plugin.TypeName))
                return;

            visiting.Add(plugin.TypeName);

            foreach (var depName in plugin.Dependencies)
            {
                if (pluginMap.TryGetValue(depName, out var dependency))
                {
                    Visit(dependency, pluginMap, visited, visiting, result);
                }
                else
                {
                    Console.WriteLine($"Предупреждение: Не найдена зависимость '{depName}'");
                }
            }

            visiting.Remove(plugin.TypeName);
            visited.Add(plugin.TypeName);
            result.Add(plugin);
        }
    }
}
