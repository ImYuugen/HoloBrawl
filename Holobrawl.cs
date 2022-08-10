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
        public List<Character> Players { get; private set; }

        private Random _random;
        private Stopwatch _stopwatch;

        private float angle = 0f;
        
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
                {"fauna", Content.Load<Texture2D>("Sprites/fauna")},
                {"delta", Content.Load<Texture2D>("Sprites/delta")},
                {"p1", Content.Load<Texture2D>("Sprites/p1")},
                {"p2", Content.Load<Texture2D>("Sprites/p2")},
            };
            
            Players = new List<Character>
            {
                // new Character(_textures["fauna"], Vector2.UnitX * 100),
                // new Character(_textures["delta"], Vector2.UnitX * -100),
                new (_textures["p1"], "p1", Vector2.UnitX * 100),
                new (_textures["p2"], "p2", Vector2.UnitX * -100),
            };
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

            foreach (var player in Players)
            {
                player.Update(gameTime);
            }
            
            angle = MathHelper.PiOver4 * (float)gameTime.TotalGameTime.TotalSeconds;

            _camera.FollowPlayers();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _stopwatch.Restart();
            
            _screen.Set();
            GraphicsDevice.Clear(Color.Black);

            _shapes.Begin(_camera);
            for (int i = 0; i < 100; i++)
            {
                _shapes.DrawFilledRectangle(-100, i * -100, 200, 100, new Color(i * 10, i * 10, i * 10));
            }
            _shapes.End();

            _sprites.Begin(_camera, false);
            
            foreach (var player in Players)
            {
                player.DrawEntity(_sprites);
            }
            
            _sprites.End();
            
            _screen.Unset();
            _screen.Present(_sprites);
            
            _stopwatch.Stop();
            
            base.Draw(gameTime);
        }
    }
}