using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HoloBrawl.Core;

namespace HoloBrawl.Graphics
{
    public sealed class Screen : IDisposable
    {
        private const int MinDim = 144;
        private const int MaxDim = 4096;

        private bool _isSet;
        private bool _isDisposed;
        private readonly Game _game;
        private readonly RenderTarget2D _renderTarget;

        public int Width => _renderTarget.Width;
        public int Height => _renderTarget.Height;
        
        public Screen(Game game, int width, int height)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game), "Game was null. |Screen Initializer|");
            _renderTarget = new RenderTarget2D(
                game.GraphicsDevice, 
                Utils.Clamp(width, MinDim, MaxDim), 
                Utils.Clamp(height, MinDim, MaxDim));
            _isSet = false;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            
            _renderTarget?.Dispose();
            _isDisposed = true;
        }

        public void Set()
        {
            if (_isSet) throw new InvalidOperationException("Render target is already set. |Screen Set|");
            
            _game.GraphicsDevice.SetRenderTarget(_renderTarget);
            _isSet = true;
        }
        
        public void Unset()
        {
            if (!_isSet) throw new InvalidOperationException("Render target is not set. |Screen Unset|");
            
            _game.GraphicsDevice.SetRenderTarget(null);
            _isSet = false;
        }

        public void Present(Sprites sprites, bool textureFiltering = true)
        {
            if (sprites is null) throw new ArgumentNullException(nameof(sprites), "Sprites was null. |Screen Present|");
#if DEBUG            
            _game.GraphicsDevice.Clear(Color.LimeGreen);
#else
            _game.GraphicsDevice.Clear(Color.Black);
#endif
            sprites.Begin(null, textureFiltering);
            sprites.Draw(_renderTarget, null, GetDestinationRectangle(), Color.White);
            sprites.End();
        }
        
        private Rectangle GetDestinationRectangle()
        { 
            Rectangle backBufferBounds = _game.GraphicsDevice.PresentationParameters.Bounds;
            var backBufferRatio = (float)backBufferBounds.Width / backBufferBounds.Height;
            var screenRatio = (float)Width / Height;
            
            float rx = 0f, ry = 0f;
            float rw = backBufferBounds.Width, rh = backBufferBounds.Height;

            if (backBufferRatio > screenRatio)
            {
                rw = rh * screenRatio;
                rx = (backBufferBounds.Width - rw) / 2f;
            }
            else if (backBufferRatio < screenRatio)
            {
                rh = rw / screenRatio;
                ry = (backBufferBounds.Height - rh) / 2f;
            }
            
            return new Rectangle((int)rx, (int)ry, (int)rw, (int)rh);
        }
    }
}