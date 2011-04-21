using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

using System;
using System.Diagnostics;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class BackgroundObject
    {
        [NonSerialized]
        private Texture2D displayTexture;
        private string displayTextureName;
        private Vector2 position;
        private ScrollingWorld world;

        public BackgroundObject(ScrollingWorld myWorld, string myDisplayTextureName, Vector2 myPosition)
        {
            displayTexture = GameEngine.TextureList[myDisplayTextureName];
            displayTextureName = myDisplayTextureName;
            world = myWorld;
            position = myPosition;

        }

        public void reloadNonSerializedAssets()
        {
            displayTexture = GameEngine.TextureList[displayTextureName];
           
        }

        public void Draw(GraphicsDevice device, Matrix camera)
        {
            float guyPos = world.getCameraCoords().X;
            Matrix cameraTransform = Matrix.CreateTranslation(guyPos, 0.0f, 0.0f);

            GameEngine.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);       
            GameEngine.Instance.SpriteBatch.Draw(displayTexture, position * CASSWorld.SCALE, Color.White);
            GameEngine.Instance.SpriteBatch.End();
        }

    }
}