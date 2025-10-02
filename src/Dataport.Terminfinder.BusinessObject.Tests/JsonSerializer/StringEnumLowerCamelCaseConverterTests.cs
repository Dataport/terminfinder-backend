using Dataport.Terminfinder.BusinessObject.JsonSerializer;
using System.Text;

namespace Dataport.Terminfinder.BusinessObject.Tests.JsonSerializer;

[TestClass]
public class StringEnumLowerCamelCaseConverterTests
{
    [TestMethod]
    public void WriteJson_valueAnyValue_anyValue()
    {
        var sut = new StringEnumLowerCamelCaseConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, UtEnumValue.Value, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("\"value\"", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_valueIsNull_null()
    {
        var sut = new StringEnumLowerCamelCaseConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, null, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void ReadJson_valueIsAnyValue_deserializedValue()
    {
        var value = UtEnumValue.Value;
        var sut = new StringEnumLowerCamelCaseConverter();
        using var sr = new StringReader($"\"{value.ToString().ToLower()}\"");
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, typeof(UtEnumValue), null,
            Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsInstanceOfType(result, typeof(UtEnumValue));
        var resultEnum = (UtEnumValue)result;
        Assert.AreEqual(UtEnumValue.Value, resultEnum);
    }

    [TestMethod]
    public void ReadJson_valueIsNull_undefined()
    {
        var sut = new StringEnumLowerCamelCaseConverter();
        using var sr = new StringReader("\"null\"");
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, typeof(UtEnumValue), null,
            Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsInstanceOfType(result, typeof(UtEnumValue));
        var resultEnum = (UtEnumValue)result;
        Assert.AreEqual(UtEnumValue.Undefined, resultEnum);
    }

    [TestMethod]
    public void CanConvert_EnumType_true()
    {
        var sut = new StringEnumLowerCamelCaseConverter();
        Assert.IsTrue(sut.CanConvert(typeof(UtEnumValue)));
    }

    [TestMethod]
    public void CanRead_nothing_true()
    {
        var sut = new StringEnumLowerCamelCaseConverter();
        Assert.IsTrue(sut.CanRead);
    }

    private enum UtEnumValue
    {
        Undefined = 0,
        Value = 500
    }
}