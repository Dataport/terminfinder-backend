using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject.Tests.JsonSerializer;

[TestClass]
[ExcludeFromCodeCoverage]
public class DateConverterTests
{
    private CultureInfo _culture;

    [TestInitialize]
    public void SetUp()
    {
        _culture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
    }

    [TestCleanup]
    public void CleanUp()
    {
        Thread.CurrentThread.CurrentCulture = _culture;
    }

    [TestMethod]
    public void WriteJson_dateTimeValue_serializedDateValue()
    {
        DateTime? dt = new DateTime(2015, 12, 1);
        var sut = new DateConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, dt, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("\"2015-12-01\"", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_dateTimeValueIsNull_null()
    {
        var sut = new DateConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, null, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_NonDateTimeValue_serializedDateValue()
    {
        var sut = new DateConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, 500, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void ReadJson_NonValidDateValueAsString_deserializedDateValue_false()
    {
        var dt = "99.99.2015";
        var sut = new DateConverter();
        using var sr = new StringReader($"\"{dt}\"");
        using var jsonReader = new JsonTextReader(sr);
        try
        {
            jsonReader.Read();
            sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());
            Assert.Fail("An Exception should be thrown");
        }
        catch (FormatException)
        {
            // Assert
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void ReadJson_dateValueAsString_deserializedDateValue()
    {
        var dt = "2015-10-02";
        var sut = new DateConverter();
        using var sr = new StringReader($"\"{dt}\"");
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsInstanceOfType(result, typeof(DateTime?));
        var dtResult = (DateTime?)result;
        Assert.AreEqual("2015-10-02", dtResult.Value.ToString(DateConverter.DateFormat));
    }

    [TestMethod]
    public void ReadJson_dateValueIsNull_null()
    {
        var sut = new DateConverter();
        using var sr = new StringReader(string.Empty);
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ReadJson_dateValueAsNullStringIsNull_True()
    {
        var sut = new DateConverter();
        using var sr = new StringReader("null");
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void CanConvert_NullableDate_true()
    {
        var sut = new DateConverter();
        Assert.IsTrue(sut.CanConvert(typeof(DateTime?)));
    }

    [TestMethod]
    public void CanConvert_Date_true()
    {
        var sut = new DateConverter();
        Assert.IsTrue(sut.CanConvert(typeof(DateTime)));
    }

    [TestMethod]
    public void CanRead_nothing_true()
    {
        var sut = new DateConverter();
        Assert.IsTrue(sut.CanRead);
    }
}