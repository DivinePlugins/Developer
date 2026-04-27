namespace Divine.Plugin.Engine.Model.Source2;

using System;

internal static class LongFieldPathFormat
{
    private static readonly int[] BITS_PER_COMPONENT = { 11, 11, 11, 8, 8, 4, 4 };

    public static readonly int MAX_FIELDPATH_LENGTH = BITS_PER_COMPONENT.Length;

    private static readonly long[] CLEAR_MASK = new long[MAX_FIELDPATH_LENGTH - 1];
    private static readonly long[] PRESENT_BIT = new long[MAX_FIELDPATH_LENGTH - 1];
    private static readonly long[] VALUE_SHIFT = new long[MAX_FIELDPATH_LENGTH];
    private static readonly long[] VALUE_MASK = new long[MAX_FIELDPATH_LENGTH];
    private static readonly long[] OFFSET = new long[MAX_FIELDPATH_LENGTH];
    private static readonly long PRESENT_MASK;

    static LongFieldPathFormat()
    {
        int bitCount = -1;
        for (int i = 0; i < MAX_FIELDPATH_LENGTH; i++)
        {
            bitCount += BITS_PER_COMPONENT[i] + 1;
        }
        if (bitCount > 63)
        {
            throw new InvalidOperationException("too many bits used");
        }

        int cur = bitCount;
        long presentMaskAkku = 0L;
        for (int i = 0; i < MAX_FIELDPATH_LENGTH; i++)
        {
            OFFSET[i] = i == 0 ? 1L : 0L;
            if (i != 0)
            {
                CLEAR_MASK[i - 1] = -1L << cur & (1L << bitCount) - 1;
                cur--;
                PRESENT_BIT[i - 1] = 1L << cur;
                presentMaskAkku |= 1L << cur;
            }
            cur -= BITS_PER_COMPONENT[i];
            VALUE_SHIFT[i] = cur;
            VALUE_MASK[i] = (1L << BITS_PER_COMPONENT[i]) - 1L << cur;
        }
        PRESENT_MASK = presentMaskAkku;
    }

    public static long set(long id, int i, int v)
    {
        return id & ~VALUE_MASK[i] | v + OFFSET[i] << (int)VALUE_SHIFT[i];
    }

    public static int get(long id, int i)
    {
        return (int)(((id & VALUE_MASK[i]) >> (int)VALUE_SHIFT[i]) - OFFSET[i]);
    }

    public static long down(long id)
    {
        return id | PRESENT_BIT[last(id)];
    }

    public static long up(long id, int n)
    {
        return id & CLEAR_MASK[last(id) - n];
    }

    public static int last(long id)
    {
        return (int)long.PopCount(id & PRESENT_MASK);
    }

    public static int hashCode(long id)
    {
        return id.GetHashCode();
    }

    public static int compareTo(long id1, long id2)
    {
        return id1.CompareTo(id2);
    }
}