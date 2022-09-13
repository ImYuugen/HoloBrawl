using System;
using Microsoft.Xna.Framework.Input;

namespace VBrawler.Input;

public sealed class GameKeyboard
{
    private static readonly Lazy<GameKeyboard> Lazy = new (() => new GameKeyboard());
    public static GameKeyboard Instance => Lazy.Value;

    private KeyboardState _currentState;
    private KeyboardState _previousState;

    public GameKeyboard()
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