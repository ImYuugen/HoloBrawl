using System;
using HoloBrawl.Core;
using HoloBrawl.Graphics;
using HoloBrawl.Input.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Entities;

public class Character : Entity
{
     private const float MaxVelX = 500f;
     private const float MaxVelY = 1000f;

     private Vector2 _forceBuffer;
     
     private bool _isGrounded;
     private float _gravity;
     private float _gravityModifier;
     private float _dragCoeff;
     //TODO: Add attributes for speed, acceleration, etc.
     
     private Controller _controller;

     public float Speed { get; set; }
     public float JumpSpeed { get; set; }
     
     public Hurtbox[] Hurtboxes { get; set; }
     public float Percentage { get; private set; }
     private bool DrawHurtbox { get; set; }

     public Character(string name, Vector2 position, bool drawHurtbox = false) : base(null, name, position)
     {
          _gravity = _gravityModifier * Physics.Gravity;
          DrawHurtbox = drawHurtbox;
          Hurtboxes = Array.Empty<Hurtbox>();
          _controller = new Controller();
     }

     public void Init(Texture2D sprite, string profile)
     {
          _controller = Data.LoadProfile(profile);
          _controller.Init(this);
          Sprite = sprite;
          for (var index = 0; index < Hurtboxes.Length; index++)
          { 
               Hurtboxes[index].Init(this);
          }
          
          _gravityModifier = 1;
          _dragCoeff = 0f;
          Speed = 500;
          JumpSpeed = 1000;
     }

     public override void AddForce(Vector2 force)
     {
          var velX = MathHelper.Clamp(Velocity.X + force.X, -MaxVelX, MaxVelX);
          var velY = MathHelper.Clamp(Velocity.Y + force.Y, -MaxVelY, MaxVelY);
          Velocity = new Vector2(velX, velY);
     }
     public void AddForce(float forceX, float forceY)
     {
          AddForce(new Vector2(forceX, forceY));
     }

     public void AddForceBuffer(float forceX, float forceY)
     {
          _forceBuffer.X += forceX;
          _forceBuffer.Y += forceY;
     }

     public override void Update(GameTime gameTime)
     {
          for (var index = 0; index < Hurtboxes.Length; index++)
          {
               foreach (var floor in Data.LoadedTerrain.Floors)
               {
                    _isGrounded = Hurtboxes[index].Intersects(floor.X, floor.Y, floor.Width, floor.Height);
                    if (_isGrounded) break;
               }
               Hurtboxes[index].Update();
          }

          if (!_isGrounded)
          {
               _gravity = _gravityModifier * Physics.Gravity;
               AddForceBuffer(0f, _gravity); // Gravity
          }
          else
          {
               Velocity = new Vector2(Velocity.X, 0);
          }
          
          _controller.Move();
          
          AddForce(_forceBuffer * (float) gameTime.ElapsedGameTime.TotalSeconds);
          _forceBuffer = Vector2.Zero;
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