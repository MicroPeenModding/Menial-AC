using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Menial_AC.Source
{
    public class Player
    {
        public IntPtr address { get; set; }

        public Vector3 headPos { get; set; }
        public Vector3 playerPos { get; set; }

        public float yaw { get; set; }
        public float pitch { get; set; }

        public int health { get; set; }
        public string name { get; set; }
        public int team { get; set; }
        public int isDead { get; set; }

        public int weaponID { get; set; }


    }
}
