using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HoloBrawl.Graphics
{
    public sealed class Sprites : IDisposable
    {

        private bool _disposed;
        private readonly Game _game;
        private readonly SpriteBatch _sprites;
        private readonly BasicEffect _effect;
        
        public Sprites(Game game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _disposed = false;
            
            _sprites = new SpriteBatch(game.GraphicsDevice);
            _effect = new BasicEffect(game.GraphicsDevice)
            {
                FogEnabled = false,
                TextureEnabled = true,
                LightingEnabled = false,
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity,
                Projection = Matrix.Identity
            };
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _sprites?.Dispose();
            _disposed = true;
        }

        public void Begin(Camera camera, bool textureFiltering = true)
        {

            if (camera is null)
            {
                var vp = _game.GraphicsDevice.Viewport;
                _effect.Projection = 
                    Matrix.CreateOrthographicOffCenter(0, vp.Width, 0, vp.Height, 0, 1);
                _effect.View = Matrix.Identity;
            }
            else
            {
                camera.UpdateMatrices();
                _effect.View = camera.View;
                _effect.Projection = camera.Projection;
            }

            _sprites.Begin(
                blendState:BlendState.AlphaBlend, 
                samplerState:textureFiltering ? SamplerState.LinearClamp : SamplerState.PointClamp, 
                rasterizerState:RasterizerState.CullNone, 
                effect:_effect);
        }

        public void End()
        {
            _sprites.End();
        }

        #region Draw methods
        
        public void Draw(Texture2D texture, Vector2 origin, Vector2 position, Color color)
        {
            _sprites.Draw(texture, position, null, color, 0f, origin, 1f, SpriteEffects.FlipVertically, 0f);
        }
        
        public void Draw(Texture2D texture, Rectangle? source, Vector2 origin, Vector2 position, float rotation, Vector2 scale, Color color)
        {
            _sprites.Draw(texture, position, source, color, rotation, origin, scale, SpriteEffects.FlipVertically, 0f);
        }

        public void Draw(Texture2D texture, Rectangle? source, Rectangle destination, Color color)
        {
            _sprites.Draw(texture, destination, source, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0f);
        }

        #endregion
    }
}