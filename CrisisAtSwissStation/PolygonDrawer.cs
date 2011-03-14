using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace CrisisAtSwissStation
{
    /**
     * Class for drawing simple polygons.
     * You can use this in your game, if you'd
     * like.  You'll probably want to change the
     * above namespace from "CrisisAtSwissStation" to something
     * more appropriate, though.
     * 
     * If you want me to add features, feel free
     * to contact the course staff.
     * 
     *  -Don
     */
    public class PolygonDrawer : IDisposable
    {
        protected BasicEffect effect;
        protected VertexDeclaration decl;

        public PolygonDrawer(GraphicsDevice device, int width, int height)
        {
            effect = new BasicEffect(device);
            effect.View = Matrix.Identity;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);

            effect.TextureEnabled = true;
            effect.LightingEnabled = false;
        }

        public void Dispose()
        {
            effect.Dispose();
        }

        public void DrawPolygons(Vector2 position, float angle, float scale,
            Texture2D texture, VertexPositionTexture[] vertices, BlendState blendMode)
        {
            
            effect.World = Matrix.CreateRotationZ(angle) *
                Matrix.CreateScale(scale) *
                Matrix.CreateTranslation(new Vector3(position, 0));
            effect.Texture = texture;

            GraphicsDevice device = GameEngine.Instance.GraphicsDevice;
            
            if (blendMode == BlendState.AlphaBlend)
            {
                device.BlendState = BlendState.AlphaBlend;
            }
            else if (blendMode == BlendState.Additive)
            {
                device.BlendState = BlendState.Additive;
            }
            SamplerState s = new SamplerState();
            s.AddressU = TextureAddressMode.Wrap;
            s.AddressV = TextureAddressMode.Wrap;
            device.SamplerStates[0] = s;
            
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
                //pass.Apply();
            }
        }
    }
}