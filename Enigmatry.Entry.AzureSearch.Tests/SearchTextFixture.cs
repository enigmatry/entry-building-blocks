﻿using Shouldly;

namespace Enigmatry.Entry.AzureSearch.Tests;

[Category("unit")]
public class SearchTextFixture
{
    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("some", "some")]
    [TestCase("some+", "some+")]
    public void TestAsNotEscaped(string? value, string expectedValue)
    {
        var searchText = SearchText.AsNotEscaped(value);
        searchText.OriginalValue.ShouldBe(value);
        searchText.Value.ShouldBe(expectedValue);
    }

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("some", "some")]
    [TestCase("some+", "some\\+")]
    public void TestAsEscaped(string? value, string expectedValue)
    {
        var searchText = SearchText.AsEscaped(value);
        searchText.OriginalValue.ShouldBe(value);
        searchText.Value.ShouldBe(expectedValue);
    }

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("some", "\"some\"")]
    [TestCase("some+", "\"some\\+\"")]
    [TestCase("first last", "\"first last\"")]
    public void TestAsPhraseSearch(string? value, string expectedValue)
    {
        var searchText = SearchText.AsPhraseSearch(value);
        searchText.OriginalValue.ShouldBe(value);
        searchText.Value.ShouldBe(expectedValue);
    }

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("some", "\"some\" OR some* OR some~1")]
    [TestCase("some+", "\"some\\+\" OR some\\+* OR some\\+~1")]
    [TestCase("first last", "\"first last\" OR (+first +last)")]
    public void TestAsFullSearch(string? value, string expectedValue)
    {
        var searchText = SearchText.AsFullSearch(value);
        searchText.OriginalValue.ShouldBe(value);
        searchText.Value.ShouldBe(expectedValue);
    }
}
