using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VBrawler.Graphics;

namespace VBrawler.Entities;

public abstract class Entity
{
    public Texture2D Sprite { get; protected set; }
    public string Name { get; set; }
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

    public void Draw(Sprites sprites)
    {
        if (Sprite is null) return;
        sprites?.Draw(Sprite, new Vector2(Sprite.Width/2f, Sprite.Height/2f), Position, Color.White);
    }
    
    public virtual void AddForce(Vector2 force)
    {
        Velocity += force;
    }
}