namespace Divine.Plugin.Engine.IO.Source2;

using System;
using System.Text;

public class SerializerId : IEquatable<SerializerId>
{
    private string name;
    private int version;

    public SerializerId(String name, int version)
    {
        this.name = name;
        this.version = version;
    }

    public string getName()
    {
        return name;
    }

    public int getVersion()
    {
        return version;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SerializerId);
    }

    public bool Equals(SerializerId? other)
    {
        return other is not null && version == other.version && name == other.name;
    }

    public override int GetHashCode()
    {
        int result = name.GetHashCode();
        result = 31 * result + version;
        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder(name);
        sb.Append('(');
        sb.Append(version);
        sb.Append(')');
        return sb.ToString();
    }
}