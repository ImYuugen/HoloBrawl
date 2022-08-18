using System;
using HoloBrawl.Core;
using HoloBrawl.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Entities;

public abstract class Entity
{
    public Texture2D Sprite { get; protected set; }
    public string Name { get; private set; }
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public float Rotation { get; protected set; }

    protected Entity(Texture2D sprite, string name, Vector2 position)
    {
        Name = name;
        Sprite = sprite;
        Position = position;
        Velocity = Vector2.Zero;
        Rotation = 0f;
    }
    
    public virtual void Update(GameTime gameTime)
    {
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public virtual void DrawEntity(Sprites sprites)
    {
        sprites?.Draw(Sprite, new Vector2(Sprite.Height/2f, Sprite.Width/2f), Position, Color.White);
    }
    
    public virtual void Draw(Sprites sprites)
    {
        DrawEntity(sprites);
    }
    
    public virtual void AddForce(Vector2 force)
    {
        Velocity += force;
    }
}