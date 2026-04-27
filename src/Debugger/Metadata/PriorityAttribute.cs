namespace Debugger.Metadata;

using System;
using System.Collections.Generic;
using System.Text;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class PriorityAttribute(int value) : Attribute
{
    public int Value { get; init; } = value;
}
