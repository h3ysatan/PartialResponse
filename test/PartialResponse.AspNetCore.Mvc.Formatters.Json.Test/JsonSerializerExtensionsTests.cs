using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal;
using Xunit;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json
{
    public class JsonSerializerExtensionsTests
    {
        private readonly JsonSerializer jsonSerializer = JsonSerializer.Create(new JsonSerializerSettings());
        private readonly StringBuilder result = new StringBuilder();
        private readonly JsonWriter jsonWriter;

        public JsonSerializerExtensionsTests()
        {
             this.jsonWriter = new JsonTextWriter(new StringWriter(this.result));
        }

        [Fact]
        public void TheSerializeMethodShouldFilterArrayElements()
        {
            // Arrange
            var value = new List<dynamic> { new { foo = "bar", baz = "qux" } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo");

            // Assert
            Assert.Equal("[{\"foo\":\"bar\"}]", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldRemoveEmptyArray()
        {
            // Arrange
            var value = new { foo = new List<dynamic> { new { bar = "baz" } }, qux = "quux" };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "qux");

            // Assert
            Assert.Equal("{\"qux\":\"quux\"}", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldNotRemoveEmptyArrayIfRoot()
        {
            // Arrange
            var value = new List<dynamic> { new { foo = "bar" } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => false);

            // Assert
            Assert.Equal("[]", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldFilterObjectPropertiesInsideArrayElement()
        {
            // Arrange
            var value = new List<dynamic> { new { foo = new { bar = "baz", qux = "quux" } } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo" ||  _ == "foo/bar");

            // Assert
            Assert.Equal("[{\"foo\":{\"bar\":\"baz\"}}]", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldFilterObjectProperties()
        {
            // Arrange
            var value = new { foo = "bar", baz = "qux" };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo");

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldRemoveEmptyObject()
        {
            // Arrange
            var value = new { foo = new { bar = "baz" }, qux = "quux" };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "qux");

            // Assert
            Assert.Equal("{\"qux\":\"quux\"}", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldNotRemoveEmptyObjectIfRoot()
        {
            // Arrange
            var value = new { foo = "bar" };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => false);

            // Assert
            Assert.Equal("{}", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldFilterObjectPropertiesInsideObjectProperties()
        {
            // Arrange
            var value = new { foo = new { bar = "baz", qux = "quux" } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo" || _ == "foo/bar");

            // Assert
            Assert.Equal("{\"foo\":{\"bar\":\"baz\"}}", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldFilterArrayElementsInsideObjectProperties()
        {
            // Arrange
            var value = new { foo = new List<dynamic> { new { bar = "baz", qux = "quux" } } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo" || _ == "foo/bar");

            // Assert
            Assert.Equal("{\"foo\":[{\"bar\":\"baz\"}]}", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldFilterArrayElementInsideArrayElement()
        {
            // Arrange
            var value = new List<dynamic> { new { foo = new List<dynamic> { new { bar = "baz", qux = "quux" } } } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo" ||  _ == "foo/bar");

            // Assert
            Assert.Equal("[{\"foo\":[{\"bar\":\"baz\"}]}]", result.ToString());
        }

        [Fact]
        public void TheSerializeMethodShouldApplyCaching()
        {
            // Arrange
            var value = new List<dynamic> { new { foo = "bar" }, new { foo = "bar" } };
            var count = 0;

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => { count++; return  _ == "foo"; });

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public void TheSerializeMethodFilterCorrectlyUsingCache()
        {
            // Arrange
            var value = new List<dynamic> { new { foo = "bar", baz = "qux" }, new { foo = "bar", baz = "qux" } };

            // Act
            jsonSerializer.Serialize(this.jsonWriter, value, _ => _ == "foo");

            // Assert
            Assert.Equal("[{\"foo\":\"bar\"},{\"foo\":\"bar\"}]", result.ToString());
        }
    }
}