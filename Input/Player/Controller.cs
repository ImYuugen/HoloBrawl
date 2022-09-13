using System;
using System.IO;
using HoloBrawl.Core;
using HoloBrawl.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HoloBrawl.Input.Player;

/// <summary>
/// Used to control a Character class' movement.
/// </summary>
/// <remarks>NOT A GAMEPAD</remarks>
public class Controller
{
    private Character controlledCharacter;
    private HoloKeyboard keyboard = HoloKeyboard.Instance;

    public string ProfileName;
    public Keys[] Controls;
    
    public Controller(){}
    public Controller(string name, Keys[] keys) { ProfileName = name; Controls = keys; }
    public Controller(Character controlledCharacter)
    {
        this.controlledCharacter = controlledCharacter;
        keyboard = HoloKeyboard.Instance;
    }
    
    public void Init(Character owner)
    { 
        controlledCharacter = owner;
        keyboard = HoloKeyboard.Instance;
    }

    public void Move()
    {
        Vector2 movement = Vector2.Zero;
        bool dash = false;
        if (controlledCharacter == null) return;

        if (controlledCharacter.IsGrounded)
        {
            if (!controlledCharacter.IsDashing && keyboard.IsKeyClicked(Controls[5]))
            {
                dash = true;
            } //Dash
            if (keyboard.IsKeyDown(Controls[1]))
            {
                movement.X += -controlledCharacter.WalkingSpeed;
                controlledCharacter.IsFacingRight = false;
            } //Left
            if (keyboard.IsKeyDown(Controls[3]))
            {
                movement.X += controlledCharacter.WalkingSpeed;
                controlledCharacter.IsFacingRight = true;
            } //Right
            if (keyboard.IsKeyClicked(Controls[4]))
            {
                controlledCharacter.Accelerate(0, controlledCharacter.JumpSpeed);
                controlledCharacter.State = PlayerState.Jumping;
            } //Jump
        }
        else
        {
            if (keyboard.IsKeyClicked(Controls[5]))
            {
                dash = true;
            } //Dash
            if (keyboard.IsKeyDown(Controls[1]))
            {
                movement.X += -controlledCharacter.AirAcceleration;
            } //Left
            if (keyboard.IsKeyDown(Controls[3]))
            {
                movement.X += controlledCharacter.AirAcceleration;
            } //Right
        }

        if (dash && movement.X != 0)
        {
            Dash(controlledCharacter.IsFacingRight);
        }
        else if (controlledCharacter.IsDashing && controlledCharacter.Lag <= 0)
        {
            controlledCharacter.IsDashing = false;
        }
        
        if (controlledCharacter.IsGrounded)
        {
            controlledCharacter.SetVel(movement);
        }
        else
        {
            controlledCharacter.Accelerate(movement.X, 0f);
        }
    }

    private void Dash(bool right)
    {
        Console.WriteLine($"Player {controlledCharacter.Name} is dashing");
        if (controlledCharacter.Lag > 0 && !controlledCharacter.IsDashing) return; //If the player is in any other lag state than dashing, don't dash

        Console.WriteLine("Initial dash");
        controlledCharacter.Lag = (float) Constants.DashLag.TotalSeconds;
        controlledCharacter.IsDashing = true;
        
        if (right)
        {
            controlledCharacter.SetVel(Vector2.UnitX * controlledCharacter.DashSpeed);
        }
        else
        {
            controlledCharacter.SetVel(-Vector2.UnitX * controlledCharacter.DashSpeed);
        }
    }
}