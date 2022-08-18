using System;
using HoloBrawl.Entities;
using HoloBrawl.Graphics;
using Microsoft.Xna.Framework;

namespace HoloBrawl.Core;


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
    public Rectangle Zone;
    public HurtboxType Type;
    public Character Owner { get; set; }

    private int BaseX { get; set; }
    private int BaseY { get; set; }
    
    public Hurtbox(Rectangle area, HurtboxType type, Character owner)
    {
        Type = type;
        Owner = owner;
        Zone = new Rectangle(area.X + (int) owner.Position.X,
            area.Y + (int) owner.Position.Y, area.Width, area.Height);
        BaseX = 0;
        BaseY = 0;
    }

    public bool Intersects(Rectangle rect)
    {
        return Zone.Intersects(rect);
    }
    
    public void Update()
    {
        Zone.X = (int) Owner.Position.X + BaseX;
        Zone.Y = (int) Owner.Position.Y + BaseY;
    }
    public void Init(Character owner)
    {
        BaseX = Zone.X;
        BaseY = Zone.Y;
        Owner = owner;
    }
    
    public void Draw(Shapes shapes)
    {
        var color = Type switch
        {
            HurtboxType.Normal => Color.Yellow,
            HurtboxType.Intangible => Color.Blue,
            HurtboxType.Invincible => Color.Purple,
            _ => Color.White
        };

        shapes.DrawFilledRectangle(Zone, new Color(color, .1f));
        shapes.DrawRectangle(Zone, 1f, color);
    }
}

public struct Hitbox
{
    public Rectangle Zone { get; }
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
        
        Zone = area;
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
        
        shapes.DrawFilledRectangle(Zone, new Color(color, .1f));
        shapes.DrawRectangle(Zone, 1f, color);
    }
    
    public bool Intersects(Rectangle rect)
    {
        return Zone.Intersects(rect);
    }
}