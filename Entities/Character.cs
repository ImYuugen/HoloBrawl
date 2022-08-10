using System;
using HoloBrawl.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HoloBrawl.Entities;

public class Character : Entity
{
     private const float MaxVelX = 10f;
     private const float MaxVelY = 100f;
     
     private float _gravity;

     public float Percentage { get; private set; } 
     
     private float GravityModifier { get; }

     public Keys[] Keys { get; set; }

     public Character(Texture2D sprite, string name, Vector2 position) : base(sprite, name, position)
     {
          Percentage = 0;
          GravityModifier = 1f;
          _gravity = Data.Gravity * GravityModifier;
     }

     public void AddForce(Vector2 force)
     {
          var velX = MathHelper.Clamp(Velocity.X + force.X, -MaxVelX, MaxVelX);
          var velY = MathHelper.Clamp(Velocity.Y + force.Y, -MaxVelY, MaxVelY);
          Velocity = new Vector2(velX, velY);
     }

     public override void Update(GameTime gameTime)
     {
          AddForce(Vector2.UnitY * _gravity); //TODO fix this shit;
          Console.WriteLine($"{Name} velocity: {Velocity}");
          base.Update(gameTime);
     }
}