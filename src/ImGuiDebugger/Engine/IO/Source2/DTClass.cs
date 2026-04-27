namespace Divine.Plugin.Engine.IO.Source2;

using System;
using System.Text;

using Divine.Plugin.Engine.IO.Decoder;
using Divine.Plugin.Engine.IO.Source2.Fields;
using Divine.Plugin.Engine.Model.Source2;
using Divine.Plugin.Engine.Model.State;

internal sealed class DTClass : IDTClass
{
    private SerializerField field;
    private int classId = -1;

    public DTClass(SerializerField field)
    {
        this.field = field;
    }

    public int getClassId()
    {
        return classId;
    }

    public void setClassId(int classId)
    {
        this.classId = classId;
    }

    public string getDtName()
    {
        return field.getSerializer().getId().getName();
    }

    public Serializer getSerializer()
    {
        return field.getSerializer();
    }

    public IEntityState getEmptyState()
    {
        return new NestedArrayEntityState(field);
    }

    public Field getFieldForFieldPath(IFieldPath fp)
    {
        switch (fp.last())
        {
            case 0:
                return field.getChild(fp.get(0));
            case 1:
                return field.getChild(fp.get(0)).getChild(fp.get(1));
            case 2:
                return field.getChild(fp.get(0)).getChild(fp.get(1)).getChild(fp.get(2));
            case 3:
                return field.getChild(fp.get(0)).getChild(fp.get(1)).getChild(fp.get(2)).getChild(fp.get(3));
            case 4:
                return field.getChild(fp.get(0)).getChild(fp.get(1)).getChild(fp.get(2)).getChild(fp.get(3)).getChild(fp.get(4));
            case 5:
                return field.getChild(fp.get(0)).getChild(fp.get(1)).getChild(fp.get(2)).getChild(fp.get(3)).getChild(fp.get(4)).getChild(fp.get(5));
            case 6:
                return field.getChild(fp.get(0)).getChild(fp.get(1)).getChild(fp.get(2)).getChild(fp.get(3)).getChild(fp.get(4)).getChild(fp.get(5)).getChild(fp.get(6));
            default:
                throw new NotSupportedException();
        }
    }

    public IDecoder? getDecoderForFieldPath(IFieldPath fp)
    {
        Field f = getFieldForFieldPath(fp);
        return f != null ? f.getDecoder() : null;
    }

    public FieldType? getTypeForFieldPath(IFieldPath fp)
    {
        Field f = getFieldForFieldPath(fp);
        return f != null ? f.getType() : null;
    }

    public string? getNameForFieldPath(IFieldPath fp)
    {
        var sb = new StringBuilder();

        Field currentField = field;
        int i = 0;
        int last = fp.last();
        while (true)
        {
            int idx = fp.get(i);
            var segment = currentField.getChildNameSegment(idx);
            if (segment == null)
            {
                return null;
            }
            if (i != 0)
                sb.Append('.');
            sb.Append(segment);
            if (i == last)
            {
                return sb.ToString();
            }
            currentField = currentField.getChild(idx);
            i++;
        }
    }

    public IFieldPath? getFieldPathForName(string fieldName)
    {
        IModifiableFieldPath fp = IModifiableFieldPath.newInstance();

        Field currentField = field;
        var search = fieldName;
        while (true)
        {
            int dotIdx = search.IndexOf('.');
            bool last = (dotIdx == -1);
            var segment = last ? search : search[..dotIdx];
            int? fieldIdx = currentField.getChildIndex(segment);
            if (fieldIdx == null)
            {
                return null;
            }

            fp.cur(fieldIdx.Value);
            if (last)
            {
                return fp.unmodifiable();
            }

            fp.down();
            currentField = currentField.getChild(fieldIdx.Value);
            search = search[(segment.Length + 1)..];
        }
    }

    public override string ToString()
    {
        return string.Format("{0} ({1})", field.getSerializer().getId(), classId);
    }
}