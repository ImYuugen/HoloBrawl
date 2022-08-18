using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HoloBrawl.Core;
using HoloBrawl.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HoloBrawl.Graphics;
using HoloBrawl.Input;
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
            foreach (var character in Characters.Values)
            {
                character.Init(_textures[character.Name]);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboard = HoloKeyboard.Instance;
            keyboard.Update();
            
            if (keyboard.IsKeyClicked(Keys.Escape))
            {
                Exit();
            }
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

            foreach (var key in Characters.Keys)
            {
                Characters[key].Update(gameTime);
            }

            _camera.FollowPlayers();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _stopwatch.Restart();

            _screen.Set();
            GraphicsDevice.Clear(Color.Black);
            _sprites.Begin(_camera, false);
            
            _shapes.Begin(_camera);
            for (int i = 0; i < 100; i++)
            {
                _shapes.DrawFilledRectangle(-100, i * -100, 200, 100, new Color(i * 10, i * 10, i * 10));
            }
            _shapes.End();

            _shapes.Begin(_camera);
            foreach (var player in Characters.Values)
            {
                player.Draw(_sprites, _shapes);
            }
            _shapes.End();
            
            _sprites.End();
            _screen.Unset();
            _screen.Present(_sprites);
            
            _stopwatch.Stop();
            
            base.Draw(gameTime);
        }
    }
}