namespace Divine.Plugin.Engine.IO.Source2;

using Divine.Plugin.Engine.IO.Source2.Fields;

internal sealed class Serializer
{

    private SerializerId id;
    private Field[] fields;
    private string[] fieldNames;

    public Serializer(SerializerId id, Field[] fields, string[] fieldNames)
    {
        this.id = id;
        this.fields = fields;
        this.fieldNames = fieldNames;
    }

    public SerializerId getId()
    {
        return id;
    }

    public int getFieldCount()
    {
        return fields.Length;
    }

    public Field getField(int idx)
    {
        return fields[idx];
    }

    public string getFieldName(int idx)
    {
        return fieldNames[idx];
    }

    public int? getFieldIndex(string name)
    {
        int searchHash = name.GetHashCode();
        for (int i = 0; i < fields.Length; i++)
        {
            var fieldName = fieldNames[i];
            if (searchHash != fieldName.GetHashCode())
                continue;
            if (name.Equals(fieldName))
                return i;
        }
        return null;
    }

}