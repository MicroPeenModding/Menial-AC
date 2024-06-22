using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Menial_AC.Source
{
    public class Offsets
    {
        // Pointers
        public int localPlayer = 0x17E0A8;
        public int entityList = 0x18AC04;
        public int viewMatrix = 0x17DFD0; // 0x17DFD0
        public int gameMode = 0x18ABF8;
        public int mapName = 0x17F148;

        // Instructions
        public int moveInstructionAddress = 0xC0AA3;
        public string moveInstructionOriginal = "84 DB";
        public string moveInstructionNop = "90 90";

        // Classes
        public int weaponInfoClass = 0x364; // Top Level
        public int gunInfoClass = 0xC;
        public int currentWeaponClass = 0x14;

        // Weapon Info Class
        public int equippedWeaponID = 0x4;

        // Gun Info Class
        public int weaponName = 0x0; // char[16]
        public int animationSpeed = 0x48; // smaller number = faster fire rate
        public int knockback = 0x54;
        public int recoil = 0x60;

        // Current Weapon Class
        public int gunClipAmmo = 0x0;
        public int gunDelay = 0x24;

        // Player
        public int HeadPos = 0x4; // Vector3
        public int PlayerPos = 0x28; // Vector3
        public int Yaw = 0x34;
        public int Pitch = 0x38;
        public int Health = 0xEC;
        public int Grounded = 0x5D; // 0 = no, 1 = yes. set to 1 to jump in air
        public int isShooting = 0x204;
        public int Name = 0x205; // char[16]
        public int TeamID = 0x30C; // int - in FFA all player are 1. TDM - 0 = CLA, 1 = RVSF, 4 = Spectator
        public int Dead = 0x318;// int(bool) 1 = dead, 0 = alive

        // ac_client.exe+C0AA3 - 84 DB       noclip         

    }
}
