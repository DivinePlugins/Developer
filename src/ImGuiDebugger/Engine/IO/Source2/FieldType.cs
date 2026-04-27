namespace Divine.Plugin.Engine.IO.Source2;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public sealed partial class FieldType
{
    [GeneratedRegex("^(.*?)(< (.*) >)?(\\*)?(\\[(.*?)\\])?$")]
    private static partial Regex FIELD_TYPE_PATTERN();

    private string baseType;
    private FieldType? genericType;
    private bool pointer;
    private string? elementCount;
    private FieldType? elementType;

    public FieldType(string typeString)
    {
        var m = FIELD_TYPE_PATTERN().Match(typeString);
        if (!m.Success)
        {
            throw new Exception("cannot parse field type");
        }

        baseType = m.Groups[1].Value;
        genericType = m.Groups[3].Success ? forString(m.Groups[3].Value) : null;
        pointer = m.Groups[4].Success;
        elementCount = m.Groups[6].Success ? m.Groups[6].Value : null;

        if (elementCount == null)
        {
            elementType = null;
        }
        else
        {
            elementType = forString(toString(true));
        }
    }

    public string getBaseType()
    {
        return baseType;
    }

    public FieldType? getGenericType()
    {
        return genericType;
    }

    public bool isPointer()
    {
        return pointer;
    }

    public string? getElementCount()
    {
        return elementCount;
    }

    public FieldType? getElementType()
    {
        return elementType;
    }

    public override string ToString()
    {
        return toString(false);
    }

    private string toString(bool omitElementCount)
    {
        var sb = new StringBuilder();
        sb.Append(baseType);
        if (genericType != null)
        {
            sb.Append("< ");
            sb.Append(genericType.ToString());
            sb.Append(" >");
        }
        if (pointer)
        {
            sb.Append('*');
        }
        if (!omitElementCount && elementCount != null)
        {
            sb.Append('[');
            sb.Append(elementCount);
            sb.Append(']');
        }
        return sb.ToString();
    }

    private static readonly Dictionary<string, FieldType> FIELD_TYPE_MAP = new();

    public static FieldType forString(string fieldTypeString)
    {
        if (!FIELD_TYPE_MAP.TryGetValue(fieldTypeString, out var result))
        {
            if (!FIELD_TYPE_MAP.TryGetValue(fieldTypeString, out result))
            {
                result = new FieldType(fieldTypeString);
                FIELD_TYPE_MAP.Add(fieldTypeString, result);
            }
        }

        return result;
    }
}