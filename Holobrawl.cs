using System.Collections.Generic;
using HoloBrawl.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static HoloBrawl.Data.Data;

namespace HoloBrawl
{
    public class Holobrawl : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Sprites _sprites;
        private Dictionary<string, Texture2D> _textures;

        public Holobrawl()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Data.Data.Load();

            _graphics.PreferredBackBufferWidth = Data.Data.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Data.Data.ScreenHeight;
            _graphics.ApplyChanges();

            _sprites = new Sprites(this);
            
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

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _sprites.Begin(false);
            foreach (var sprite in _textures)
            {
                var texture2D = sprite.Value;
                _sprites.Draw(texture2D,new Vector2(
                    texture2D.Width / 2f, texture2D.Height / 2f),
                    new Vector2(ScreenWidth / 2f, ScreenHeight / 2f), 
                    Color.White);
            }
            _sprites.End();

            base.Draw(gameTime);
        }
    }
}