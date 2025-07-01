using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace task05
{
    public class ClassAnalyzer
    {
        private readonly Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }

        public IEnumerable<string> GetPublicMethods()
        {
            return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(m => m.Name);
        }

        public IEnumerable<string> GetMethodParams(string methodName)
        {
            var method = _type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance) ?? throw new ArgumentException(string.Format("Method '{0}' not found.", methodName));
            return new[] { string.Format("Return Type: {0}", method.ReturnType.Name) }.Concat(method.GetParameters().Select(p => string.Format("Param: {0} ({1})", p.Name, p.ParameterType.Name)));
        }

        public IEnumerable<string> GetAllFields()
        {
            return _type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Select(f => f.Name);
        }

        public IEnumerable<string> GetProperties()
        {
            return _type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            return Attribute.IsDefined(_type, typeof(T));
        }
    }
}