using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HoloBrawl.Graphics;
using HoloBrawl.Input;
using HoloBrawl.Core;
using HoloBrawl.Entities;
using HoloBrawl.Terrain;
using static HoloBrawl.Core.Data;

namespace HoloBrawl
{
    public class Holobrawl : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private Screen _screen;
        private Camera _camera;
        
        private Sprites _sprites;
        private Shapes _shapes;
        private Dictionary<string, Texture2D> _textures;

        private bool _paused;
        
        private Random _random;
        private Stopwatch _stopwatch;

        public Holobrawl()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            LoadData();

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.IsFullScreen = Fullscreen;
            _graphics.ApplyChanges();
            
            _screen = new Screen(this, ScreenWidth, ScreenHeight);
            _camera = new Camera(_screen);
            
            _sprites = new Sprites(this);
            _shapes = new Shapes(this);
            
            _random = new Random();
            _stopwatch = new Stopwatch();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _textures = new Dictionary<string, Texture2D>
            {
                // {"fauna", Content.Load<Texture2D>("Sprites/fauna")},
                // {"delta", Content.Load<Texture2D>("Sprites/delta")},
                {"P1", Content.Load<Texture2D>("Sprites/p1")},
                {"P2", Content.Load<Texture2D>("Sprites/p2")},
            };
            
            
            LoadCharacter("P1");
            LoadCharacter("P2");
            Characters["P1"].Init(_textures["P1"], "P1");
            Characters["P2"].Init(_textures["P2"], "Default");
            LoadAndSetMap("Test");
        }

        private void Simulate(GameTime gameTime, HoloKeyboard keyboard)
        {

            foreach (var key in Characters.Keys)
            {
                Characters[key].Update(gameTime);
            }

            _camera.FollowPlayers();
        }
        
        protected override void Update(GameTime gameTime)
        {
            var keyboard = HoloKeyboard.Instance;
            keyboard.Update();
            
            if (keyboard.IsKeyClicked(Keys.P))
            {
                _paused = !_paused;
                Console.WriteLine($"[INFO] {(_paused?"Paused":"Unpaused")}");
            }
            
            if (!_paused)
                Simulate(gameTime, keyboard);
            
            if (keyboard.IsKeyClicked(Keys.Escape))
            {
                Exit();
            }
            #region debug
#if DEBUG
            if (keyboard.IsKeyClicked(Keys.F3))
            {
                Console.WriteLine($"[INFO BATCH] @ game time {gameTime.TotalGameTime}.");
                Console.WriteLine($"  ├ FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds:#00.0}, Slow : {gameTime.IsRunningSlowly}");
                Console.WriteLine($"  ├ Last draw time: {_stopwatch.Elapsed}");
                Console.WriteLine($"  ├ Memory: {GC.GetTotalMemory(false) / 1024 / 1024:#####.#####} MB");
                Console.WriteLine($"  ├ Screen: {ScreenWidth}x{ScreenHeight}, Fullscreen: {_graphics.IsFullScreen}");
                _camera.GetExtents(out Vector2 min, out var max);
                Console.WriteLine($"  └ Camera: {_camera.Position}, Z : {_camera.Z}, Zoom : {_camera.Zoom}, Extents : {min} - {max}\n");

            } //Game info
            if (keyboard.IsKeyClicked(Keys.Q))
            {
                _camera.ZoomIn();
            }
            if (keyboard.IsKeyClicked(Keys.E))
            {
                _camera.ZoomOut();
            }
            if (keyboard.IsKeyClicked(Keys.F))
            {
                Utils.ToggleFullscreen(_graphics);
            }
#endif
            #endregion
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _stopwatch.Restart();

            _screen.Set();
            GraphicsDevice.Clear(Color.Black);

            _shapes.Begin(_camera);
            _sprites.Begin(_camera, false);
            foreach (var player in Characters.Values)
            {
                player.Draw(_sprites, _shapes);
            }
            foreach (var floors in LoadedTerrain.Floors)
            {
                floors.Draw(_shapes);
            }
            _sprites.End();
            _shapes.End();
            
            _screen.Unset();
            _screen.Present(_sprites);
            
            _stopwatch.Stop();
            
            base.Draw(gameTime);
        }
    }
}