using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enigmatry.Entry.Core.Helpers;

namespace Enigmatry.Entry.AzureSearch;

// Helper class for building Azure Search Filters - https://learn.microsoft.com/en-us/azure/search/search-filters
// https://learn.microsoft.com/en-us/azure/search/search-query-odata-filter#examples
public class AzureSearchFilterBuilder
{
    private readonly StringBuilder _searchFilter = new();

    public void AddStatements<T>(IReadOnlyCollection<T> values, Func<T, string> toStatement)
    {
        if (values.Count <= 0)
        {
            return;
        }

        AppendAnd();
        OpenBracket();
        Append(values, toStatement);
        CloseBracket();
    }

    public void AddStatement(string statement)
    {
        if (statement.HasContent())
        {
            AppendAnd();
            OpenBracket();
            Append(statement);
            CloseBracket();
        }
    }

    private void Append<T>(IEnumerable<T> statements, Func<T, string> func)
    {
        var statement = statements.Select(func).JoinStringWithOnlyValuesWithContent(" or ");
        Append(statement);
    }

    private void CloseBracket() => _searchFilter.Append(')');

    private void OpenBracket() => _searchFilter.Append('(');

    private void AppendAnd()
    {
        if (_searchFilter.Length > 0)
        {
            _searchFilter.Append(" and ");
        }
    }

    private void Append(string condition) => _searchFilter.Append(condition);

    public string Build() => _searchFilter.ToString();
}
