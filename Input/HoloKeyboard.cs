using System;
using Microsoft.Xna.Framework.Input;

namespace HoloBrawl.Input;

public sealed class HoloKeyboard
{
    private static readonly Lazy<HoloKeyboard> Lazy = new Lazy<HoloKeyboard>(() => new HoloKeyboard());
    public static HoloKeyboard Instance => Lazy.Value;

    private KeyboardState _currentState;
    private KeyboardState _previousState;

    public HoloKeyboard()
    {
        _previousState = Keyboard.GetState();
        _currentState = _previousState;
    }

    public void Update()
    {
        _previousState = _currentState;
        _currentState = Keyboard.GetState();
    }
    
    public bool IsKeyDown(Keys key) => _currentState.IsKeyDown(key);
    public bool IsKeyUp(Keys key) => _currentState.IsKeyUp(key);
    public bool IsKeyClicked(Keys key) => _currentState.IsKeyDown(key) && _previousState.IsKeyUp(key);
}