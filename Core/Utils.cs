using System;

namespace HoloBrawl.Core;

public static class Utils
{
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
        if (min.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(nameof(min), "min must be <= max.");
        
        if (value.CompareTo(min) < 0)
            return min;
        else if (value.CompareTo(max) > 0)
            return max;
        else
            return value;
    }
    
    public static void Normalize(ref float x, ref float y)
    {
        var invSqrt = FastInvSqrt(x*x + y*y); // 1 / sqrt(x*x + y*y) using Quake's fast inverse square root
        x *= invSqrt;
        y *= invSqrt;
    }
    
    /// <summary>
    /// Computes the inverse square root of x.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private static float FastInvSqrt(float x)
    {
        var xHalf = 0.5f * x;
        var i = BitConverter.SingleToInt32Bits(x);
        i = 0x5f3759df - (i >> 1); // What the fuck?
        x = BitConverter.Int32BitsToSingle(i);
        x = x * (1.5f - xHalf * x * x);
        return x;
    }
}