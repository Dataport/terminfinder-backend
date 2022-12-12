using System.Collections;
using System.Collections.Generic;
using Dataport.Terminfinder.Common.Extension;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.Common.Tests.Extension;

[TestClass]
[ExcludeFromCodeCoverage]
public class EnumerableExtensionsTests
{
    [TestMethod]
    public void IsNullOrEmpty_emptyList_true()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var input = new List<string>();
        Assert.IsTrue(input.IsNullOrEmpty());
    }

    [TestMethod]
    public void IsNullOrEmpty_nonEmptyList_false()
    {
        var input = new List<string> { null };
        Assert.IsFalse(input.IsNullOrEmpty());
    }

    [TestMethod]
    public void IsNullOrEmpty_nonEmptyHs_false()
    {
        var input = new UtEnumerable { null };
        Assert.IsFalse(input.IsNullOrEmpty());
    }

    [TestMethod]
    public void IsNullOrEmpty_emptyEnumerable_true()
    {
        var input = new UtEnumerable();
        Assert.IsTrue(input.IsNullOrEmpty());
    }

    private class UtEnumerable : IEnumerable<string>
    {
        private readonly List<string> _values = new ();

        public void Add(string value)
        {
            _values.Add(value);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_values).GetEnumerator();
        }
    }
}