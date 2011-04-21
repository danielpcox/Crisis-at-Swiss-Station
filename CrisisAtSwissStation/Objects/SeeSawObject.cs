using System;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class SeeSawObject : BoxObject
    {    

        [NonSerialized]
        private Texture2D seeSawTexture;
        private string texturePath;
        private bool hack;
        private World theWholeWideWorld;

        public SeeSawObject(World world, string myTexturePath, float myScale, Vector2 myPosition)
            : base(world, myTexturePath, 1f, .5f, 0.0f, myScale, false)
        {
            hack = true;
            theWholeWideWorld = world;
            texturePath = myTexturePath;
            TextureFilename = texturePath;
            seeSawTexture = GameEngine.TextureList[texturePath];        

        }

        public void reloadNonSerializedAssets()
        {
            seeSawTexture = GameEngine.TextureList[texturePath];           
        }

        
        public override void Update(CASSWorld world, float dt)
        {
            
            if (hack)
            {
                RevoluteJointDef jointDef = new RevoluteJointDef();
                jointDef.Initialize(Body, theWholeWideWorld.GetGroundBody(), Common.Utils.Convert(Position));                
                theWholeWideWorld.CreateJoint(jointDef);
                
                hack = false;
            }          

            base.Update(world, dt);
        }       
    }


}