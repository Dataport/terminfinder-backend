using System.IO;
using System.Text;
using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject.Tests.JsonSerializer;

[TestClass]
[ExcludeFromCodeCoverage]
public class GuidNullConverterTests
{

    [TestMethod]
    public void WriteJson_GuidValue_serializedStringValue()
    {
        Guid guid = new("c1c2474b-488a-4ecf-94e8-47387bb715d5");
        var sut = new GuidNullConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, guid, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("\"c1c2474b-488a-4ecf-94e8-47387bb715d5\"", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_GuidValueIsEmpty_null()
    {
        var sut = new GuidNullConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, null, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_NonGuidValue_serializedGuidValue()
    {
        var sut = new GuidNullConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, 500, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void ReadJson_GuidValueAsString_deserializedGuidValue()
    {
        Guid guid = new("c1c2474b-488a-4ecf-94e8-47387bb715d5");
        var sut = new GuidNullConverter();
#pragma warning disable IDE0071 // Simplify interpolation
        using var sr = new StringReader($"\"{guid.ToString()}\"");
#pragma warning restore IDE0071 // Simplify interpolation
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsInstanceOfType(result, typeof(Guid));
        var dtResult = (Guid)result;
        Assert.AreEqual("c1c2474b-488a-4ecf-94e8-47387bb715d5", dtResult.ToString());
    }

    [TestMethod]
    public void ReadJson_GuidValueIsNull_Empty()
    {
        var sut = new GuidNullConverter();
        using var sr = new StringReader(string.Empty);
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.AreEqual(Guid.Empty, result);
    }

    [TestMethod]
    public void CanConvert_Guid_true()
    {
        var sut = new GuidNullConverter();
        Assert.IsTrue(sut.CanConvert(typeof(Guid)));
    }

    [TestMethod]
    public void CanRead_nothing_true()
    {
        var sut = new GuidNullConverter();
        Assert.IsTrue(sut.CanRead);
    }
}