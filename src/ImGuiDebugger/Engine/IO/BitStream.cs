namespace Divine.Plugin.Engine.IO;

using System;
using System.Text;

using Divine.Common.Log;
using Divine.Plugin.Engine.IO.Source2;

using Google.Protobuf;

internal sealed class BitStream
{
    private const int COORD_INTEGER_BITS = 14;
    private const int COORD_FRACTIONAL_BITS = 5;
    private const float COORD_RESOLUTION = (1.0f / (1 << COORD_FRACTIONAL_BITS));

    private const int COORD_INTEGER_BITS_MP = 11;
    private const int COORD_FRACTIONAL_BITS_MP_LOWPRECISION = 3;
    private const int COORD_DENOMINATOR_LOWPRECISION = (1 << COORD_FRACTIONAL_BITS_MP_LOWPRECISION);
    private const float COORD_RESOLUTION_LOWPRECISION = (1.0f / COORD_DENOMINATOR_LOWPRECISION);

    private const int NORMAL_FRACTIONAL_BITS = 11;
    private const float NORMAL_FRACTIONAL_RESOLUTION = (1.0f / ((1 << NORMAL_FRACTIONAL_BITS) - 1));

    public static long[] MASKS = {
        0x0L,               0x1L,                0x3L,                0x7L,
        0xfL,               0x1fL,               0x3fL,               0x7fL,
        0xffL,              0x1ffL,              0x3ffL,              0x7ffL,
        0xfffL,             0x1fffL,             0x3fffL,             0x7fffL,
        0xffffL,            0x1ffffL,            0x3ffffL,            0x7ffffL,
        0xfffffL,           0x1fffffL,           0x3fffffL,           0x7fffffL,
        0xffffffL,          0x1ffffffL,          0x3ffffffL,          0x7ffffffL,
        0xfffffffL,         0x1fffffffL,         0x3fffffffL,         0x7fffffffL,
        0xffffffffL,        0x1ffffffffL,        0x3ffffffffL,        0x7ffffffffL,
        0xfffffffffL,       0x1fffffffffL,       0x3fffffffffL,       0x7fffffffffL,
        0xffffffffffL,      0x1ffffffffffL,      0x3ffffffffffL,      0x7ffffffffffL,
        0xfffffffffffL,     0x1fffffffffffL,     0x3fffffffffffL,     0x7fffffffffffL,
        0xffffffffffffL,    0x1ffffffffffffL,    0x3ffffffffffffL,    0x7ffffffffffffL,
        0xfffffffffffffL,   0x1fffffffffffffL,   0x3fffffffffffffL,   0x7fffffffffffffL,
        0xffffffffffffffL,  0x1ffffffffffffffL,  0x3ffffffffffffffL,  0x7ffffffffffffffL,
        0xfffffffffffffffL, 0x1fffffffffffffffL, 0x3fffffffffffffffL, 0x7fffffffffffffffL,
        -1L
    };

    private static int[] UBV_COUNT = { 0, 4, 8, 28 };
    private static int[] UBVFP_COUNT = { 2, 4, 10, 17, 31 };

    private int _len;
    private int _pos;
    private byte[] stringTemp = new byte[32768];

    private BufferB64 buffer;

    public BitStream(BufferB64 buffer)
    {
        this.buffer = buffer;
    }

    public static BitStream createBitStream(ByteString input)
    {
        return new BitStream(new(input.Span))
        {
            _len = input.Length * 8
        };
    }

    private int peekBit(int pos)
    {
        return (int)(buffer.get(pos >> 6) >> (pos & 63) & 1);
    }

    public int readUBitInt(int n)
    {
        int start = _pos >> 6;
        int end = (_pos + n - 1) >> 6;
        int s = _pos & 63;
        _pos += n;
        return (int)(((buffer.get(start) >>> s) | (buffer.get(end) << (64 - s))) & MASKS[n]);
    }

    public long readUBitLong(int n)
    {
        int start = _pos >> 6;
        int end = (_pos + n - 1) >> 6;
        int s = _pos & 63;
        _pos += n;
        return ((buffer.get(start) >>> s) | (buffer.get(end) << (64 - s))) & MASKS[n];
    }

    public void readBitsIntoByteArray(byte[] dest, int n)
    {
        int o = 0;
        while (n > 8)
        {
            dest[o++] = (byte)readUBitInt(8);
            n -= 8;
        }
        if (n > 0)
        {
            dest[o] = (byte)readUBitInt(n);
        }
    }

    public FieldOp readFieldOp()
    {
        int offs = _pos >> 6;
        int s = _pos & 63;
        int i = 0;
        long v = buffer.get(offs);
        while (true)
        {
            _pos++;
            i = FieldOpHuffmanTree.tree[i][(int)((v >>> s) & 1L)];
            if (i < 0)
            {
                return FieldOpHuffmanTree.ops[-i - 1];
            }
            if (++s == 64)
            {
                v = buffer.get(++offs);
                s = 0;
            }
        }
    }

    public int len()
    {
        return _len;
    }

    public int pos()
    {
        return _pos;
    }

    public void pos(int pos)
    {
        if (pos >= _len)
        {
            throw new ArgumentException("pos >= len");
        }

        _pos = pos;
    }

    public int remaining()
    {
        return _len - _pos;
    }

    public void skip(int n)
    {
        _pos = _pos + n;
    }

    public int readBit()
    {
        return peekBit(_pos++);
    }

    public bool readBitFlag()
    {
        return peekBit(_pos++) != 0;
    }

    public long readSBitLong(int n)
    {
        long v = readUBitLong(n);
        return (v & (1L << (n - 1))) == 0 ? v : v | (MASKS[64 - n] << n);
    }

    public int readSBitInt(int n)
    {
        int v = readUBitInt(n);
        return (v & (1 << (n - 1))) == 0 ? v : v | ((int)MASKS[32 - n] << n);
    }

    public unsafe string readString(int n)
    {
        int o = 0;
        while (o < n)
        {
            byte c = (byte)readUBitInt(8);
            if (c == 0)
            {
                break;
            }

            stringTemp[o] = c;
            o++;
        }

        try
        {
            fixed (byte* fixedStringTemp = stringTemp)
            {
                return new string((sbyte*)fixedStringTemp, 0, o, Encoding.UTF8);
            }
        }
        catch (Exception e)
        {
            LogManager.Error(e);
            return string.Empty;
        }
    }

    public long readVarU(int max)
    {
        int m = ((max + 6) / 7) * 7;
        int s = 0;
        long v = 0L;
        long b;
        while (true)
        {
            b = readUBitInt(8);
            v |= (b & 0x7FL) << s;
            s += 7;
            if ((b & 0x80L) == 0L || s == m)
            {
                return v;
            }
        }
    }

    public long readVarS(int max)
    {
        long v = readVarU(max);
        return (v >>> 1) ^ -(v & 1L);
    }

    public long readVarULong()
    {
        return readVarU(64);
    }

    public long readVarSLong()
    {
        return readVarS(64);
    }

    public int readVarUInt()
    {
        return (int)readVarU(32);
    }

    public int readVarSInt()
    {
        return (int)readVarS(32);
    }

    public int readUBitVar()
    {
        // Thanks to Robin Dietrich for providing a clean version of this code :-)

        // The header looks like this: [XY00001111222233333333333333333333] where everything > 0 is optional.
        // The first 2 bits (X and Y) tell us how much (if any) to read other than the 6 initial bits:
        // Y set -> read 4
        // X set -> read 8
        // X + Y set -> read 28

        int v = readUBitInt(6);
        int a = v >> 4;
        if (a == 0)
        {
            return v;
        }
        else
        {
            return (v & 15) | (readUBitInt(UBV_COUNT[a]) << 4);
        }
    }

    public int readUBitVarFieldPath()
    {
        int i = -1;
        while (++i < 4)
        {
            if (readBitFlag())
                break;
        }
        return readUBitInt(UBVFP_COUNT[i]);
    }

    public float readBitCoord()
    {
        bool i = readBitFlag(); // integer component present?
        bool f = readBitFlag(); // fractional component present?
        float v = 0.0f;
        if (!(i || f))
            return v;
        bool s = readBitFlag();
        if (i)
            v = (float)(readUBitInt(COORD_INTEGER_BITS) + 1);
        if (f)
            v += readUBitInt(COORD_FRACTIONAL_BITS) * COORD_RESOLUTION;
        return s ? -v : v;
    }

    public float readCellCoord(int n, bool integral, bool lowPrecision)
    {
        float v = (float)(readUBitInt(n));
        if (integral)
        {
            // TODO: something weird is going on here in alice, we might need to adjust the sign?
            return v;
        }

        if (lowPrecision)
        {
            throw new ArgumentException(null, nameof(lowPrecision));
        }

        return v + readUBitInt(COORD_FRACTIONAL_BITS) * COORD_RESOLUTION;
    }

    public float readCoordMp(BitStream stream, bool integral, bool lowPrecision)
    {
        int i = 0;
        int f = 0;
        bool sign = false;
        float value = 0.0f;

        bool inBounds = stream.readBitFlag();
        if (integral)
        {
            if (readBitFlag())
            {
                sign = stream.readBitFlag();
                value = stream.readUBitInt(inBounds ? COORD_INTEGER_BITS_MP : COORD_INTEGER_BITS) + 1;
            }
        }
        else
        {
            if (readBitFlag())
            {
                sign = stream.readBitFlag();
                i = stream.readUBitInt(inBounds ? COORD_INTEGER_BITS_MP : COORD_INTEGER_BITS) + 1;
            }
            else
            {
                sign = stream.readBitFlag();
            }
            f = stream.readUBitInt(lowPrecision ? COORD_FRACTIONAL_BITS_MP_LOWPRECISION : COORD_FRACTIONAL_BITS);
            value = i + ((float)f * (lowPrecision ? COORD_RESOLUTION_LOWPRECISION : COORD_RESOLUTION));
        }
        return sign ? -value : value;
    }

    public float readBitAngle(int n)
    {
        return readUBitInt(n) * 360.0f / (1 << n);
    }

    public float readBitNormal()
    {
        bool s = readBitFlag();
        float v = (float)readUBitInt(NORMAL_FRACTIONAL_BITS) * NORMAL_FRACTIONAL_RESOLUTION;
        return s ? -v : v;
    }

    public float[] read3BitNormal()
    {
        float[] v = new float[3];
        bool x = readBitFlag();
        bool y = readBitFlag();
        if (x)
            v[0] = readBitNormal();
        if (y)
            v[1] = readBitNormal();
        bool s = readBitFlag();
        float p = v[0] * v[0] + v[1] * v[1];
        if (p < 1.0f)
            v[2] = float.Sqrt(1.0f - p);
        if (s)
            v[2] = -v[2];
        return v;
    }

    public override string ToString()
    {
        StringBuilder buf = new StringBuilder();
        buf.Append('[');
        buf.Append(_pos);
        buf.Append('/');
        buf.Append(_len);
        buf.Append(']');
        buf.Append(' ');
        int prefixLen = buf.Length;
        int min = Math.Max(0, (_pos - 32));
        int max = Math.Min(_len - 1, _pos + 64);
        for (int i = min; i <= max; i++)
        {
            buf.Append(peekBit(i));
        }

        buf.Insert(_pos - min + prefixLen, '*');
        return buf.ToString();
    }

    public string toString(int from, int to)
    {
        StringBuilder buf = new StringBuilder();
        for (int i = from; i < to; i++)
        {
            buf.Append(peekBit(i));
        }
        return buf.ToString();
    }
}