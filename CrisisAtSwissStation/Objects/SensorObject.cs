using Box2DX.Dynamics;

using Microsoft.Xna.Framework.Graphics;

namespace CrisisAtSwissStation
{
    public class SensorObject : BoxObject
    {
        public SensorObject(World world, Texture2D texture)
            : base(world, texture, 0, 0, 0,1,false)
        {
            shapes[0].IsSensor = true;
        }
    }
}