﻿namespace CsJava;

public static class FileUtil
{
    private const string Base16 = "0123456789abcdef";

    public static string ToBase16(this byte n)
    {
        return Base16[n / 16].ToString() + Base16[n % 16];
    }
    
    public static string ReadBase16(this byte[] bytes, int offset, int read, bool isBigEndian = true)
    {
        string result = "";
        if (bytes.Length - offset - read < 0)
            return "";
        for (int i = 0; i < read; i++)
        {
            byte b = bytes[offset + (isBigEndian ? i : read - i - 1)];
            result += b.ToBase16();
        }
        return result;
    }

    public static ushort ReadUShort(this byte[] bytes, int offset)
    {
        ushort a = bytes[offset];
        a *= 255;
        ushort b = bytes[offset + 1];
        a += b;
        return a;
    }

    public static uint ReadUInt(this byte[] bytes, int offset)
    {
        var pow = (uint) Math.Pow(2, 24) - 1;
        var pow1 = (uint) Math.Pow(2, 16) - 1;
        var pow2 = (uint) Math.Pow(2, 8) - 1;
        
        uint a = bytes[offset] * pow;
        uint b = bytes[offset + 1] * pow1;
        uint c = bytes[offset + 2] * pow2;
        uint d = bytes[offset + 3];
        return a + b + c + d;
    }
}