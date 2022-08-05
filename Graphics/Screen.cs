using Microsoft.Xna.Framework.Graphics;

namespace HoloBrawl.Graphics
{
    public sealed class Screen
    {
        
        public GraphicsDevice GraphicsDevice { get; private set; }

        public Screen(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
        }
        
        public void SetFullscreen(bool fullscreen)
        {
        }
    }
}