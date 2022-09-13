using System;
using Microsoft.Xna.Framework;

namespace VBrawler.Core;

public static class Utils
{
    
    /// <summary>
    /// Clamps a value between a minimum and maximum value.
    /// </summary>
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
    
    /// <summary>
    /// Normalizes a vector to a length of 1.
    /// </summary>
    /// <param name="x">The x coordinate of the vector</param>
    /// <param name="y">Blahblah y blahblah</param>
    public static void Normalize(ref float x, ref float y)
    {
        var invSqrt = FastInvSqrt(x*x + y*y); // 1 / sqrt(x*x + y*y) using Quake's fast inverse square root
        x *= invSqrt;
        y *= invSqrt;
    }
    
    /// <summary>
    /// Computes the inverse square root of x.
    /// </summary>
    private static float FastInvSqrt(float x)
    {
        var xHalf = 0.5f * x;
        var i = BitConverter.SingleToInt32Bits(x);
        i = 0x5f3759df - (i >> 1); // What the fuck?
        x = BitConverter.Int32BitsToSingle(i);
        x *= (1.5f - xHalf * x * x);
        return x;
    }

    public static void ToggleFullscreen(GraphicsDeviceManager graphicsDeviceManager)
    {
        graphicsDeviceManager.HardwareModeSwitch = false; // False is borderless fullscreen
        graphicsDeviceManager.ToggleFullScreen();
    }
    public static void SetFullscreen(GraphicsDeviceManager graphicsDeviceManager, bool fullscreen)
    {
        graphicsDeviceManager.HardwareModeSwitch = false; // False is borderless fullscreen
        graphicsDeviceManager.IsFullScreen = fullscreen;
    }
    
    
    public static Vector2 Transform(Vector2 position, Transform2D transform) 
        => new (position.X * transform.CosScaleX - position.Y * transform.SinScaleY + transform.PosX,
            position.X * transform.SinScaleX + position.Y * transform.CosScaleY + transform.PosY);

    /// <summary>
    /// Takes a list of points and returns a list of lines between them.
    /// </summary>
    public static int[] Triangulate(Vector2[] vertices)
    {
        var triangleIndices = new int[(vertices.Length - 2) * 3];
        var index = 0;
        for (var i = 0; i < vertices.Length - 2; i++)
        {
            triangleIndices[index++] = 0;
            triangleIndices[index++] = i + 1;
            triangleIndices[index++] = i + 2;
        }

        return triangleIndices;
    }
}

public struct Transform2D
{
    public float PosX;
    public float PosY;

    public float CosScaleX;
    public float CosScaleY;
    public float SinScaleX;
    public float SinScaleY;

    public Transform2D(Vector2 position, float rotation, Vector2 scale)
    {
        float sin = (float) Math.Sin(rotation),
            cos = (float) Math.Cos(rotation);

        PosX = position.X;
        PosY = position.Y;
        CosScaleX = scale.X * cos;
        CosScaleY = scale.Y * cos;
        SinScaleX = scale.X * sin;
        SinScaleY = scale.Y * sin;
    }

    public Transform2D(Vector2 position, float rotation, float scale)
    {
        float sin = (float) Math.Sin(rotation),
            cos = (float) Math.Cos(rotation);

        PosX = position.X;
        PosY = position.Y;
        CosScaleX = scale * cos;
        CosScaleY = scale * cos;
        SinScaleX = scale * sin;
        SinScaleY = scale * sin;
    }

    public Matrix ToMatrix() =>
        new(
            CosScaleX, -SinScaleY, 0, PosX,
            SinScaleX, CosScaleY, 0, PosY,
            0, 0, 1, 0,
            0, 0, 0, 1);
}

public readonly struct Circle
{
    public readonly Vector2 Center;
    public readonly float Radius;
    
    public Circle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }
    
    public Circle(float x, float y, float radius)
    {
        Center = new Vector2(x, y);
        Radius = radius;
    }
    
    public bool Intersects(Circle other)
    {
        var distance = Vector2.Distance(Center, other.Center);
        return distance < Radius + other.Radius;
    }
}