using Microsoft.Xna.Framework;
using VBrawler.Entities;
using VBrawler.Graphics;

namespace VBrawler.Core;


public enum HurtboxType
{
    Normal,
    Intangible,
    Invincible
} // 0 = Normal, 1 = Intangible, 2 = Invincible

public enum HitboxType
{
    Attack,
    Grab,
    Special,
} // 0 = Attack, 1 = Grab, 2 = Special


public struct Hurtbox
{
    public float X;
    public float Y;
    public float Width;
    public float Height;
    
    public HurtboxType Type;
    public Character Owner { get; set; }

    private float BaseX { get; set; }
    private float BaseY { get; set; }
    
    public Hurtbox(float x, float y, float height, float width, HurtboxType type, Character owner)
    {
        Type = type;
        Owner = owner;
        
        X = x + owner.Position.X;
        Y = y + owner.Position.Y;
        Width = width;
        Height = height;
        
        BaseX = 0;
        BaseY = 0;
    }

    public bool Intersects(float x, float y, float width, float height)
    {
        return X + Width >= x && X <= x + width && Y + Height >= y && Y <= y + height;
    }
    public bool Intersects(Hurtbox other) => Intersects(other.X, other.Y, other.Width, other.Height);
    public bool Intersects(Hitbox other) => Intersects(other.X, other.Y, other.Width, other.Height);

    public void Update()
    {
        X = (int) Owner.Position.X + BaseX;
        Y = (int) Owner.Position.Y + BaseY;
    }
    public void Init(Character owner)
    {
        BaseX = X;
        BaseY = Y;
        Owner = owner;
    }
    
    public void Draw(Shapes shapes)
    {
        var color = Type switch
        {
            HurtboxType.Normal => Color.Red,
            HurtboxType.Intangible => Color.Blue,
            HurtboxType.Invincible => Color.Purple,
            _ => Color.White
        };

        shapes.DrawFilledRectangle(X, Y, Width, Height, new Color(color, .1f));
        shapes.DrawRectangle(X, Y, Width, Height, 1f, color);
    }
}

public struct Hitbox
{
    public float X;
    public float Y;
    public float Width;
    public float Height;
    
    public HitboxType Type;

    public float Damage { get; }
    public float Knockback { get; }
    public bool FixedKnockback { get; } // If true, the knockback is fixed and doesn't change no matter the hitbox's damage
    private float defaultKnockback;
    public float Angle { get; }

    public float Hitstun { get; } // Hit stun effect caused by this hitbox
    public float Blockstun { get; }
    public float Hitlag { get; }

    public Hitbox(Rectangle area, HitboxType type, float damage, float defaultKnockback, bool fixedKnockback, float angle)
    { 
        Damage = damage;
        FixedKnockback = fixedKnockback;
        this.defaultKnockback = defaultKnockback;
        Angle = angle;
        Knockback = fixedKnockback ? defaultKnockback : defaultKnockback * damage / 10;
        
        X = area.X;
        Y = area.Y;
        Width = area.Width;
        Height = area.Height;
        Type = type; 
        Hitstun = 0;
        Blockstun = 0;
        Hitlag = 0;
    }
    
    public void Draw(Shapes shapes)
    {
        var color = Type switch
        {
            HitboxType.Attack => Color.Red,
            HitboxType.Special => Color.Green,
            HitboxType.Grab => Color.Blue,
            _ => Color.Transparent
        };
        
        shapes.DrawFilledRectangle(X, Y, Width, Height, new Color(color, .1f));
        shapes.DrawRectangle(X, Y, Width, Height, 1f, color);
    }
    
    public bool Intersects (float x, float y, float width, float height) =>
        x < this.X + this.Width && this.X < x + width && y + height < this.Y && this.Y + this.Height < y;
    public bool Intersects (Hurtbox other) => Intersects(other.X, other.Y, other.Width, other.Height);
    public bool Intersects (Hitbox other) => Intersects(other.X, other.Y, other.Width, other.Height);
}