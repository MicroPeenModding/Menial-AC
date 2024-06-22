using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Menial_AC.Source
{
    public class Entity
    {
        public IntPtr address { get; set; }

        public Vector3 headPos { get; set; }
        public Vector3 playerPos { get; set; }

        public Vector3 chestPos { get; set; }

        public float yaw { get; set; }
        public float pitch { get; set; }

        public int health { get; set; }
        public string name { get; set; }
        public int team { get; set; }

        public int isGrounded { get; set; }

        public string teamName { get; set; }
        public int isDead { get; set; }

        public int weaponID { get; set; }

        public string weaponName { get; set; }

        public int[] weaponInfo { get; set; } // int[0] - animation speed, int[1] - knockback, int[2] - recoil

        public Vector2 originScreenPosition { get; set; }

        public Vector2 absScreenPosition { get; set; }

        public Vector2 chestScreenPosition { get; set; }

        public float magnitude { get; set; }
        public float pixelDistance { get; set; }
        public float pixelDistanceChest { get; set; }
    }
}
