using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HoloBrawl.Core;
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
                {"fauna", Content.Load<Texture2D>("Sprites/fauna")},
                {"delta", Content.Load<Texture2D>("Sprites/delta")}
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _stopwatch.Restart();
            
            _screen.Set();
            GraphicsDevice.Clear(Color.Black);

            var transform = new Transform2D(new Vector2(0f, -100f), 0f, 1f);
            
            _shapes.Begin(_camera);
            _shapes.DrawPolygon(
                new Vector2[] {
                    new Vector2(0, 200), 
                    new Vector2(100, -50), 
                    new Vector2(20, -10), 
                    new Vector2(-20, -10), 
                    new Vector2(-100f, -50f)}, 
                transform, 0f, Color.White);
            _shapes.End();

            _sprites.Begin(_camera, false);
            // foreach (var texture2D in _textures.Select(sprite => sprite.Value))
            // {
            //     _sprites.Draw(texture2D, null, new Vector2(texture2D.Width/2f, texture2D.Height/2f),
            //         Vector2.Zero, 0, Vector2.One, Color.White);
            // }
            _sprites.End();

            
            _screen.Unset();
            _screen.Present(_sprites);
            
            _stopwatch.Stop();
            
            base.Draw(gameTime);
        }
    }
}