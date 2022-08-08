﻿using System;
using Microsoft.Xna.Framework;

namespace HoloBrawl.Core;

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
        x = x * (1.5f - xHalf * x * x);
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
    
    
    public static Vector2 Transform(Vector2 position, Transform2D tranform)
    {
        //Scale
        var sx = position.X * tranform.ScaleX;
        var sy = position.Y * tranform.ScaleY;
        
        //Rotation
        var rx = sx * tranform.Cos - sy * tranform.Sin;
        var ry = sx * tranform.Sin + sy * tranform.Cos;
        
        //Translation
        var tx = rx + tranform.PosX;
        var ty = ry + tranform.PosY;
        
        return new Vector2(tx, ty);
    }
}

public struct Transform2D
{
    public float PosX;
    public float PosY;
    public float Sin;
    public float Cos;
    public float ScaleX;
    public float ScaleY;
    
    public Transform2D(Vector2 position, float rotation, Vector2 scale)
    {
        PosX = position.X;
        PosY = position.Y;
        Sin = (float)Math.Sin(rotation);
        Cos = (float)Math.Cos(rotation);
        ScaleX = scale.X;
        ScaleY = scale.Y;
    }
    
    public Transform2D(Vector2 position, float rotation, float scale)
    {
        PosX = position.X;
        PosY = position.Y;
        Sin = (float)Math.Sin(rotation);
        Cos = (float)Math.Cos(rotation);
        ScaleX = scale;
        ScaleY = scale;
    }
}