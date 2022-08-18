using System;
using HoloBrawl.Core;
using HoloBrawl.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Entities;

public class Character : Entity
{
     private const float MaxVelX = 10f;
     private const float MaxVelY = 500f;

     private float _gravity;
     private float _gravityModifier;
     private float _dragCoeff;

     public Hurtbox[] Hurtboxes { get; set; }
     public float Percentage { get; private set; }
     public bool DrawHurtbox { get; set; }

     public Character(string name, Vector2 position, bool drawHurtbox = false) : base(null, name, position)
     {
          _gravity = _gravityModifier * Physics.Gravity;
          DrawHurtbox = drawHurtbox;
          Hurtboxes = Array.Empty<Hurtbox>();
     }

     public void Init(Texture2D sprite)
     {
          Sprite = sprite;
          for (var index = 0; index < Hurtboxes.Length; index++)
          { 
               Hurtboxes[index].Init(this);
          }

          _gravityModifier = 1;
          _dragCoeff = 0f;
     }

     public override void AddForce(Vector2 force)
     {
          var velX = MathHelper.Clamp(Velocity.X + force.X, -MaxVelX, MaxVelX);
          var velY = MathHelper.Clamp(Velocity.Y + force.Y, -MaxVelY, MaxVelY);
          Velocity = new Vector2(velX, velY);
     }

     public override void Update(GameTime gameTime)
     {
          _gravity = _gravityModifier * Physics.Gravity;
          AddForce(new Vector2(0f, _gravity * (float) gameTime.ElapsedGameTime.TotalSeconds)); // Gravity
          for (var index = 0; index < Hurtboxes.Length; index++)
          {
               Hurtboxes[index].Update();
          }

          base.Update(gameTime);
     }
     
     public void Draw(Sprites sprites, Shapes shapes)
     {
          if (sprites is not null) base.Draw(sprites);
          if (shapes is null || !DrawHurtbox) return;
          
          foreach (var hurtbox in Hurtboxes)
          {
               hurtbox.Draw(shapes);
          }
     }
}