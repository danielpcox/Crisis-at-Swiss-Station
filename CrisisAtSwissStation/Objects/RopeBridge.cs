using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CrisisAtSwissStation.Common;

namespace CrisisAtSwissStation
{
    public class RopeBridge : BoxObject
    {
        private float width;
        private float spacing;

        /**
         * Creates a new rope bridge with plank texture 'texture'
         * at height y, from x-position x1 to x-position x2, using
         * specified density, friction, and restitution coefficient.
         */
        public RopeBridge(World world, Texture2D texture, float y, float x1, float x2, float density, float friction, float restitution)
            : base(world, "DISABLED", density, friction, restitution, 1, false)
        {
            width = texture.Width / CASSWorld.SCALE;
    
            int nLinks = (int)((x2 - x1) / width);
            if (nLinks <= 0)
            {
                Position = new Vector2(x2, y);
                return;
            }

            spacing = (x2 - x1) - nLinks * width;
            spacing /= nLinks;

            Position = new Vector2(x1, y);

            children.Add(new RopeBridge(world, texture, y, x1 + width + spacing, x2, density, friction, restitution)); 
        }

        /** 
         * Recursively sets up the joints of the bridge
         */
        public override void SetupJoints(World world)
        {
            CreateEndJoint(world, true);
            SetupJointsHelper(world);
        }

        private void SetupJointsHelper(World world)
        {
            if (children.Count == 0)
            {
                //If there are no children left, set up the final plank and stop recursing
                CreateEndJoint(world, false);
                return;
            }

            //nextBoard is the next piece of the bridge
            RopeBridge nextBoard = children[0] as RopeBridge;

            // Attach this board's body to nextBoard's body with a joint
            /////////////////////////////////////////////////

            RevoluteJointDef joint = new RevoluteJointDef();
            Vector2 anchor = Position + new Vector2(width / 2, 0);
            joint.Initialize(Body, nextBoard.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            /////////////////////////////////////////////////
          
            nextBoard.SetupJointsHelper(world);
        }

        /** Handles anchoring the first and last pieces of the bridge
         * to the world versus to other bridge pieces
         */
        private void CreateEndJoint(World world, bool leftEnd)
        {
            RevoluteJointDef joint = new RevoluteJointDef();
            Vector2 anchor = Position - new Vector2(width/2, 0);
            if (!leftEnd)
                anchor = Position + new Vector2(width / 2, 0);
            joint.Initialize(Body, world.GetGroundBody(), Utils.Convert(anchor));
            world.CreateJoint(joint);
        }
    }
}
