using System;

namespace task10lib
{
    public interface IPluginCommand
    {
        void Execute();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute
    {
        public string[] Dependencies { get; }

        public PluginLoadAttribute(params string[] dependencies)
        {
            Dependencies = dependencies;
        }
    }
}
