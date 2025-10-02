using Dataport.Terminfinder.Common.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.Terminfinder.Common.Tests.Extension;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void FirstCharacterToLower_Bla_bla()
    {
        Assert.AreEqual("bla", "Bla".FirstCharacterToLower());
    }

    [TestMethod]
    public void FirstCharacterToLower_BlaF_blaF()
    {
        Assert.AreEqual("blaF", "BlaF".FirstCharacterToLower());
    }

    [TestMethod]
    public void FirstCharacterToLower_FirstCharacterAlreadyLowerChar_doNothing()
    {
        Assert.AreEqual("blaF", "blaF".FirstCharacterToLower());
    }

    [TestMethod]
    public void FirstCharacterToLower_StringEmpty_StringEmpty()
    {
        Assert.AreEqual("", "".FirstCharacterToLower());
    }
}