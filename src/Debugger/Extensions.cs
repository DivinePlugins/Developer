namespace Debugger;

using System;

using Divine.Numerics;

internal static class Extensions
{
    public static Color SetAlpha(this Color color, int alpha)
    {
        return new Color(color.R, color.G, color.B, alpha);
    }

    public static string ToCopyFormat(this object obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        if (obj is Enum)
        {
            return obj.GetType().Name + "." + obj;
        }

        if (obj is Vector3 v3)
        {
            return (int)v3.X + ", " + (int)v3.Y + ", " + (int)v3.Z;
        }

        return obj.ToString();
    }
}
