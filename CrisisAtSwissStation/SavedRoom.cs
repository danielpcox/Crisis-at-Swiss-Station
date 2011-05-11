using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrisisAtSwissStation
{
    [Serializable]
    class SavedRoom
    {
        public List<PhysicsObject> objects = new List<PhysicsObject>();
        public string backgroundName;
        public string musicName;
        public DudeObject dude;
        public WinDoorObject winDoor;
        public Box2DX.Dynamics.World world;
    }
}
