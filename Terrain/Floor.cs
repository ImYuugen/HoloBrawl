using HoloBrawl.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Terrain;

public sealed class Floor
{
    public float X;
    public float Y;
    public float Width;
    public float Height;

    public float OriginX;
    public float OriginY;
    
    public bool IsPlatform;
    
    public Texture2D Texture;

    public Floor(Texture2D texture, float x, float y, float width, float height, bool isPlatform)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        OriginX = width / 2;
        OriginY = height / 2;
        IsPlatform = isPlatform;
        Texture = texture;
    }

    public void Init(Texture2D texture)
    {
        OriginX = Width / 2;
        OriginY = Height / 2;
        
        Texture = texture;
    }
    
    /// <summary>
    /// Draws a simple rectangle the size of the floor with no texture.
    /// </summary>
    public void Draw(Shapes shapes)
    {
        shapes.DrawFilledRectangle(X, Y, Width, Height, Color.Gray);
    }
    
    public void Draw(Sprites sprites)
    {
        sprites.Draw(Texture, new Vector2(OriginX, OriginY), new Vector2(X, Y), Color.White);
    }
}