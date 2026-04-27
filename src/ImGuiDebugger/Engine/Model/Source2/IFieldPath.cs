namespace Divine.Plugin.Engine.Model.Source2;

using System;
using System.Text;

internal interface IFieldPath
{
    int get(int i);
    int last();

    public string asString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i <= last(); i++)
        {
            if (i != 0)
            {
                sb.Append('/');
            }
            sb.Append(get(i));
        }
        return sb.ToString();
    }
}

internal interface IFieldPath<T> : IFieldPath, IComparable<T>
    where T : IFieldPath
{
}