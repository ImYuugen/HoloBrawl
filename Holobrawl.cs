using System;
using System.Collections.Generic;
using System.Linq;
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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Screen _screen;
        private Sprites _sprites;
        private Shapes _shapes;
        private Dictionary<string, Texture2D> _textures;
        
        private Random rand = new();

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
            _graphics.ApplyChanges();

            _screen = new Screen(this, ScreenWidth, ScreenHeight);
            _sprites = new Sprites(this);
            _shapes = new Shapes(this);

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
                Exit();
            if (keyboard.IsKeyClicked(Keys.F3))
            {
                Console.WriteLine($"[INFO]");
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _screen.Set();
            GraphicsDevice.Clear(Color.Black);
            
            _shapes.Begin();
            _shapes.End();

            _sprites.Begin(false);
            foreach (var texture2D in _textures.Select(sprite => sprite.Value))
            {
                _sprites.Draw(texture2D, new Vector2(0, 0), new Vector2(ScreenWidth/2f, ScreenHeight/2f), Color.White);
            }
            _sprites.End();

            
            _screen.Unset();
            _screen.Present(_sprites);
            
            base.Draw(gameTime);
        }
    }
}