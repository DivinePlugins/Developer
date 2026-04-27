namespace Divine.Plugin.Engine.IO;

using System;
using System.Runtime.InteropServices;

public sealed class BufferB64
{
    private long[] data;

    public unsafe BufferB64(ReadOnlySpan<byte> input)
    {
        this.data = new long[(input.Length + 15) >> 3];
        //Array.Copy(input, 0, data, 0, input.Length);

        fixed (byte* fixedInput = input)
        fixed (long* fixedData = this.data)
        {
            NativeMemory.Copy(fixedInput, fixedData, (nuint)input.Length);
        }
    }

    public long get(int n)
    {
        return data[n];
    }
}