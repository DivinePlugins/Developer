namespace Divine.Plugin.Engine.Model.State;

using System;
using System.Collections.Generic;

using ConsoleTableExt;

using Divine.Plugin.Engine.Model.Source2;

internal interface IEntityState
{
    IEntityState copy();

    // returns true if capacity has changed
    bool setValueForFieldPath(IFieldPath fp, object value);

    T getValueForFieldPath<T>(IFieldPath fp);

    IEnumerator<IFieldPath> fieldPathIterator();

    string dump(string title, Func<IFieldPath, string> nameResolver)
    {
        var table = ConsoleTableBuilder.From(new ConsoleTableBaseData()
        {
            Column = new() { "FP", "Property", "Value" }
        })
        .WithTitle(title)
        .WithMinLength(new()
        {
            { 0, 5 },
            { 1, 50 },
            { 2, 45 }
        });

        int i = 0;

        var iterator = fieldPathIterator();
        while (iterator.MoveNext())
        {
            var fp = iterator.Current;

            table.AddRow(fp?.ToString() ?? "-", nameResolver(fp), getValueForFieldPath<object>(fp).ToString() ?? "-");

            i++;
        }

        return table.Export().ToString();
    }
}