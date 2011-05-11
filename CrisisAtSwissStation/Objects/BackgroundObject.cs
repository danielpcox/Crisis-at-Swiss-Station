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
    public class BackgroundObject : BoxObject
    {
        [NonSerialized]
        private Texture2D displayTexture;
        private string displayTextureName;
        private Vector2 position;
        private ScrollingWorld world;
        private bool destroyedBody;

        public BackgroundObject(World myWorld, ScrollingWorld myScrollingWorld, string myDisplayTextureName, Vector2 myPosition)
            : base(myWorld, myDisplayTextureName, 0f, .5f, 0.0f, 1, false)
        {
            displayTexture = GameEngine.TextureList[myDisplayTextureName];
            displayTextureName = myDisplayTextureName;
            world = myScrollingWorld;
            position = myPosition;
            destroyedBody = false;
            shapes[0].IsSensor = true;
        }

        public void reloadNonSerializedAssets()
        {
            displayTexture = GameEngine.TextureList[displayTextureName];
           
        }

        public override void Update(CASSWorld world, float dt)
        {
            if (!destroyedBody)
            { 
                
                RemoveFromWorld();
                destroyedBody = true;
            }
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