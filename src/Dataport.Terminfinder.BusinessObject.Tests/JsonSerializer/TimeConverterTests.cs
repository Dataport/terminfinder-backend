using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject.Tests.JsonSerializer;

[TestClass]
[ExcludeFromCodeCoverage]
public class TimeConverterTests
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
    public void WriteJson_dateTimeOffsetValue_serializedTimeValue()
    {
        DateTimeOffset? dt = new DateTimeOffset(2015, 1, 1, 12, 34, 56, new TimeSpan(0, 0, 0));
        var sut = new TimeConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, dt, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("\"12:34:56.000+00:00\"", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_dateTimeValueIsNull_null()
    {
        var sut = new TimeConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, null, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void WriteJson_NonDateTimeValue_serializedTimeValue()
    {
        var sut = new TimeConverter();
        var sb = new StringBuilder();
        using var sw = new StringWriter(sb);
        using var jsonWriter = new JsonTextWriter(sw);
        sut.WriteJson(jsonWriter, 500, Newtonsoft.Json.JsonSerializer.CreateDefault());
        Assert.AreEqual("null", sb.ToString());
    }

    [TestMethod]
    public void ReadJson_timeValueAsString_deserializedTimeValue()
    {
        var dt = new DateTimeOffset(2015, 1, 24, 12, 34, 56, new TimeSpan(1, 0, 0));
        var sut = new TimeConverter();
        using var sr = new StringReader($"\"{dt.ToString(TimeConverter.TimeFormat)}\"");
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsInstanceOfType(result, typeof(DateTimeOffset?));
        var dtResult = (DateTimeOffset?)result;
        Assert.AreEqual("11:34:56.000", dtResult.Value.ToString("HH:mm:ss.fff"));
    }

    [TestMethod]
    public void ReadJson_timeValueIsNull_null()
    {
        var sut = new TimeConverter();
        using var sr = new StringReader(string.Empty);
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ReadJson_timeValueAsStringIsNull_null()
    {
        var sut = new TimeConverter();
        using var sr = new StringReader("null");
        using var jsonReader = new JsonTextReader(sr);
        jsonReader.Read();
        var result = sut.ReadJson(jsonReader, null, null, Newtonsoft.Json.JsonSerializer.CreateDefault());

        Assert.IsNull(result);
    }

    [TestMethod]
    public void CanConvert_NullableTimeSpan_true()
    {
        var sut = new TimeConverter();
        Assert.IsTrue(sut.CanConvert(typeof(DateTimeOffset?)));
    }

    [TestMethod]
    public void CanConvert_TimeSpan_true()
    {
        var sut = new TimeConverter();
        Assert.IsTrue(sut.CanConvert(typeof(DateTimeOffset)));
    }

    [TestMethod]
    public void CanConvert_DateTime_false()
    {
        var sut = new TimeConverter();
        Assert.IsFalse(sut.CanConvert(typeof(DateTime)));
    }

    [TestMethod]
    public void CanRead_nothing_true()
    {
        var sut = new TimeConverter();
        Assert.IsTrue(sut.CanRead);
    }
}