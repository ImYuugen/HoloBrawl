using System;
using System.Linq;
using HoloBrawl.Core;
using HoloBrawl.Graphics;
using HoloBrawl.Input.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Entities;

public class Character : Entity
{
     private const float MaxVelY = 10000f;

     private Vector2 _acceleration;
     
     private float _gravity;
     private float _gravityModifier;
     private float _dragCoeff;
     //TODO: Add attributes for speed, acceleration, etc.
     
     private Controller _controller;

     public Vector2 Acceleration => _acceleration;
     
     public bool IsGrounded { get; private set; }
     public bool IsJumping { get; private set; }
     public bool IsDashing { get; set; }
     public bool IsFacingRight { get; set; }
     public PlayerState State { get; set; }
     
     public float Lag { get; set; }

     public float JumpSpeed { get; set; }
     public float DashSpeed { get; set; }
     public float RunningSpeed { get; set; }
     public float WalkingSpeed { get; set; }
     public float AirAcceleration { get; set; }

     public Hurtbox[] Hurtboxes { get; set; }
     public float Percentage { get; private set; }
     private bool DrawHurtbox { get; set; }

     public Character(string name, Vector2 position, bool drawHurtbox = false) : base(null, name, position)
     {
          _gravity = _gravityModifier * Physics.Gravity;
          DrawHurtbox = drawHurtbox;
          Hurtboxes = Array.Empty<Hurtbox>();
          _controller = new Controller(this);
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
     }

     public override void AddForce(Vector2 force)
     {
          var velY = MathHelper.Clamp(Velocity.Y + force.Y, -MaxVelY, MaxVelY);
          Velocity = new Vector2(Velocity.X, velY);
     }
     
     public void AddForce(float forceX, float forceY)
     {
          AddForce(new Vector2(forceX, forceY));
     }

     public void Accelerate(float forceX, float forceY)
     {
          _acceleration.X += forceX;
          _acceleration.Y += forceY;
     }

     public void SetVel(Vector2 vel)
     {
          Velocity = new Vector2(vel.X, Velocity.Y);
     }

     public override void Update(GameTime gameTime)
     {
          IsGrounded = false;
          for (var index = 0; index < Hurtboxes.Length; index++)
          {
               foreach (var floor in Data.LoadedTerrain.Floors)
               {
                    IsGrounded |= Hurtboxes[index].Intersects(floor.X, floor.Y, floor.Width, floor.Height);
                    if (IsGrounded) break;
               }
          }
          if (!IsGrounded)
          {
               _gravity = _gravityModifier * Physics.Gravity;
               Accelerate(0f, _gravity); // Gravity
          }
          else
          {
               Velocity = new Vector2(Velocity.X, 0);
          }
          _controller.Move();
          AddForce(_acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds); // Apply acceleration
          _acceleration = Vector2.Zero;
          base.Update(gameTime);
          for (var index = 0; index < Hurtboxes.Length; index++)
          {
               Hurtboxes[index].Update();
          }
          if (Lag > 0)
          {
               Console.WriteLine($"Lag of {Name}: {Lag}");
               Lag -= (float) gameTime.ElapsedGameTime.TotalSeconds;
          }
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

public enum PlayerState
{
     Stopping,
     Walking,
     Running,
     Jumping,
     AirDashing,
     HitStun,
     EndLag
}