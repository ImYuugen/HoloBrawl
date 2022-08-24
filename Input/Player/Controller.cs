using HoloBrawl.Entities;
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
    
    public string ProfileName { get; private set; }
    public Keys[] Controls { get; private set; }

    public Controller() { ProfileName = "Default"; Controls = new[] {Keys.W, Keys.A, Keys.S, Keys.D, Keys.Space}; } // If not instantiated in Json, it's default

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
        if (keyboard.IsKeyDown(Controls[0]))
        {
            // Up
        }
        if (keyboard.IsKeyDown(Controls[1]))
        {
            controlledCharacter.AddForceBuffer(-controlledCharacter.Speed, 0);
        }
        if (keyboard.IsKeyDown(Controls[2]))
        {
            // Down
        }
        if (keyboard.IsKeyDown(Controls[3]))
        {
            controlledCharacter.AddForceBuffer(controlledCharacter.Speed, 0);
        }
        if (keyboard.IsKeyClicked(Controls[4]))
        {
            //TODO: Add checks
            controlledCharacter.AddForceBuffer(0, controlledCharacter.JumpSpeed);
        }
    }
}