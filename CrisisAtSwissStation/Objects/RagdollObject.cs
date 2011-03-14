using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrisisAtSwissStation
{
    public class RagdollObject : CircleObject
    {
        // Layout of ragdoll
        //
        // o = joint
        //                   ___
        //                  |   |
        //                  |_ _|
        //   ______ ______ ___o___ ______ ______
        //L |______o______o       o______o______|   R
        //                |       |
        //                |       |
        //                |_______|
        //                | o | o |
        //                |   |   |
        //                |___|___|
        //                | o | o |
        //                |   |   |
        //                |   |   |
        //                |___|___|
        //

        private const float DENSITY = 1.0f;
        private const float TORSO_OFFSET = 2.5f; // Distance between torso center and face center
        private const float ARM_YOFFSET = 1.2f; // Y-distance between torso center and arm center
        private const float ARM_XOFFSET = 2.0f; // X-distance between torso center and arm center
        private const float FOREARM_OFFSET = 1.75f; // Distance between center of arm and center of forearm
        private const float THIGH_XOFFSET = 0.5f; // X-distance from center of torso to center of leg
        private const float THIGH_YOFFSET = 2.0f; // Y-distance from center of torso to center of thigh
        private const float SHIN_OFFSET = 1.75f;

        BoxObject leftArm;
        BoxObject leftForearm;
        BoxObject rightArm;
        BoxObject rightForearm;
        BoxObject torso;
        BoxObject leftThigh;
        BoxObject leftShin;
        BoxObject rightThigh;
        BoxObject rightShin;

        private int bubbleTimer = 0;
        private const int BUBBLE_INTERVAL = 200;
        ParticleGenerator bubbleGenerator;

        public RagdollObject(World world, Texture2D faceTexture,
            Texture2D torsoTexture, Texture2D armTexture,
            Texture2D forearmTexture, Texture2D thighTexture,
            Texture2D shinTexture, Texture2D bubbleTexture, Vector2 pos)
            : base(world, faceTexture, DENSITY, 0, 0)
        {
            // Head
            Position = pos;

            // Torso
            torso = new BoxObject(world, torsoTexture, DENSITY, 0, 0);
            torso.Position = pos + new Vector2(0, TORSO_OFFSET);
            children.Add(torso);

            // Arms
            leftArm = new BoxObject(world, armTexture, DENSITY, 0, 0);
            leftArm.Position = torso.Position + new Vector2(-ARM_XOFFSET, -ARM_YOFFSET);
            children.Add(leftArm);

            rightArm = new BoxObject(world, armTexture, DENSITY, 0, 0);
            rightArm.Position = torso.Position + new Vector2(ARM_XOFFSET, -ARM_YOFFSET);
            rightArm.Angle = MathHelper.Pi;
            children.Add(rightArm);

            // Forearms
            leftForearm = new BoxObject(world, forearmTexture, DENSITY, 0, 0);
            leftForearm.Position = leftArm.Position + new Vector2(-FOREARM_OFFSET, 0);
            children.Add(leftForearm);

            rightForearm = new BoxObject(world, forearmTexture, DENSITY, 0, 0);
            rightForearm.Position = rightArm.Position + new Vector2(FOREARM_OFFSET, 0);
            rightForearm.Angle = MathHelper.Pi;
            children.Add(rightForearm);

            // Thighs
            leftThigh = new BoxObject(world, thighTexture, DENSITY, 0, 0);
            leftThigh.Position = torso.Position + new Vector2(-THIGH_XOFFSET, THIGH_YOFFSET);
            children.Add(leftThigh);

            rightThigh = new BoxObject(world, thighTexture, DENSITY, 0, 0);
            rightThigh.Position = torso.Position + new Vector2(THIGH_XOFFSET, THIGH_YOFFSET);
            children.Add(rightThigh);

            // Shins
            leftShin = new BoxObject(world, shinTexture, DENSITY, 0, 0);
            leftShin.Position = leftThigh.Position + new Vector2(0, SHIN_OFFSET);
            children.Add(leftShin);

            rightShin = new BoxObject(world, shinTexture, DENSITY, 0, 0);
            rightShin.Position = rightThigh.Position + new Vector2(0, SHIN_OFFSET);
            children.Add(rightShin);

            //Bubble Particles
            bubbleGenerator = new ParticleGenerator(bubbleTexture);
        }

        public override void Update(DemoWorld world, float dt)
        {
            base.Update(world, dt);

            //Handles generation of bubble particles
            if (bubbleTimer == 0)
            {
                bubbleGenerator.addParticle(Body.GetWorldPoint(new Box2DX.Common.Vec2(14.5f / DemoWorld.SCALE, -64 / DemoWorld.SCALE)));
                bubbleTimer = BUBBLE_INTERVAL;
            }
            else
                bubbleTimer--;

            bubbleGenerator.Update();
        }

        public override void Draw(Vector2 offset)
        {
            base.Draw(offset);

            //Signals particle generator to draw
            bubbleGenerator.Draw(GameEngine.Instance.SpriteBatch);
        }

        public override void SetupJoints(World world)
        {
            // Implement Ragdoll Joints here
            ////////////////////////////////////////

            RevoluteJointDef joint = new RevoluteJointDef();

            // head to torso
            Vector2 anchor = Position + new Vector2(0, TORSO_OFFSET / 2f);
            joint.Initialize(Body, torso.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // torso to right upper arm
            anchor = torso.Position + new Vector2(ARM_XOFFSET/2f, -ARM_YOFFSET);
            joint.Initialize(torso.Body, rightArm.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // torso to left upper arm
            anchor = torso.Position + new Vector2(-ARM_XOFFSET / 2f, -ARM_YOFFSET);
            joint.Initialize(torso.Body, leftArm.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // torso to right upper leg
            anchor = torso.Position + new Vector2(THIGH_XOFFSET, THIGH_YOFFSET/2f);
            joint.Initialize(torso.Body, rightThigh.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // torso to left upper leg
            anchor = torso.Position + new Vector2(-THIGH_XOFFSET, THIGH_YOFFSET/2f);
            joint.Initialize(torso.Body, leftThigh.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // right lower leg to right upper leg
            anchor = rightThigh.Position + new Vector2(0, SHIN_OFFSET / 2f);
            joint.Initialize(rightShin.Body, rightThigh.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // left lower leg to left upper leg
            anchor = leftThigh.Position + new Vector2(0, SHIN_OFFSET / 2f);
            joint.Initialize(leftShin.Body, leftThigh.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // right forearm to right upper arm
            anchor = rightArm.Position + new Vector2(FOREARM_OFFSET / 2f, 0);
            joint.Initialize(rightArm.Body, rightForearm.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            // left forearm to left upper arm
            anchor = leftArm.Position + new Vector2(-FOREARM_OFFSET / 2f, 0);
            joint.Initialize(leftArm.Body, leftForearm.Body, Utils.Convert(anchor));
            world.CreateJoint(joint);

            ////////////////////////////////////////

            
        }
    }
}