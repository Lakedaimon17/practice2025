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

            if (!dllFiles.Any())
            {
                Console.WriteLine("Плагины (.dll) не найдены.");
                return;
            }

            var pluginInfos = dllFiles
                .Select(LoadAssemblyFromDll)
                .Where(asm => asm != null)
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(type =>
                        type.GetCustomAttribute<PluginLoadAttribute>() != null &&
                        typeof(IPluginCommand).IsAssignableFrom(type) &&
                        type.GetConstructor(Type.EmptyTypes) != null)
                    .Select(type => new { assembly, type }))
                .Select(x =>
                {
                    var attr = x.type.GetCustomAttribute<PluginLoadAttribute>();
                    return new PluginInfo
                    {
                        TypeName = x.type.FullName,
                        Type = x.type,
                        Dependencies = attr.Dependencies.ToList(),
                        AssemblyPath = x.assembly.Location
                    };
                })
                .ToDictionary(p => p.TypeName);

            if (!pluginInfos.Any())
            {
                Console.WriteLine("Не найдено подходящих плагинов.");
                return;
            }

            var pluginList = pluginInfos.Values.ToList();

            var orderedPlugins = TopologicalSort(pluginList);

            orderedPlugins.ToList().ForEach(RunPlugin);
        }

        private void RunPlugin(PluginInfo plugin)
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

        private Assembly LoadAssemblyFromDll(string dll)
        {
            try
            {
                return Assembly.LoadFrom(dll);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке {dll}: {ex.Message}");
                return null;
            }
        }

        private class PluginInfo
        {
            public string TypeName { get; set; }
            public Type Type { get; set; }
            public List<string> Dependencies { get; set; }
            public string AssemblyPath { get; set; }
        }

        private IEnumerable<PluginInfo> TopologicalSort(List<PluginInfo> plugins)
        {
            var visited = new HashSet<string>();
            var visiting = new HashSet<string>();
            var result = new List<PluginInfo>();
            var pluginMap = plugins.ToDictionary(p => p.TypeName);

            plugins.ToList().ForEach(plugin =>
            {
                Visit(plugin, pluginMap, visited, visiting, result);
            });

            return result;
        }

        private void Visit(
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

            plugin.Dependencies.ToList().ForEach(depName =>
            {
                if (pluginMap.TryGetValue(depName, out var dependency))
                {
                    Visit(dependency, pluginMap, visited, visiting, result);
                }
                else
                {
                    Console.WriteLine($"Предупреждение: Не найдена зависимость '{depName}'");
                }
            });

            visiting.Remove(plugin.TypeName);
            visited.Add(plugin.TypeName);
            result.Add(plugin);
        }
    }
}
