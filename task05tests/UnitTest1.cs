using System;
using Xunit;
using Moq;
using task05;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;
        private string _privateField;

        public int Property { get; set; }

        public void Method(int x, string y) { }
    }

    [Serializable]
    public class AttributedClass { }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsAllProperties()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var properties = analyzer.GetProperties();

            Assert.Contains("Property", properties);
        }

        [Fact]
        public void GetMethodParams_ReturnsCorrectParametersAndReturnType()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var parameters = analyzer.GetMethodParams("Method");

            Assert.Contains("Param: x (Int32)", parameters);
            Assert.Contains("Param: y (String)", parameters);
            Assert.Contains("Return Type: Void", parameters);
        }

        [Fact]
        public void HasAttribute_ReturnsTrueForAttributedClass()
        {
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));
            var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

            Assert.True(hasAttribute);
        }

        [Fact]
        public void HasAttribute_ReturnsFalseIfNoAttribute()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));
            var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

            Assert.False(hasAttribute);
        }

        [Fact]
        public void GetMethodParams_ThrowsIfMethodNotFound()
        {
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            var exception = Assert.Throws<ArgumentException>(() =>
                analyzer.GetMethodParams("UnknownMethod"));

            Assert.Contains("not found", exception.Message);
        }
    }
}
