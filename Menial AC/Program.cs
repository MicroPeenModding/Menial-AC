




using ClickableTransparentOverlay;
using ImGuiNET;
using Menial_AC.Source;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Swed32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using Veldrid;
using Vortice.Mathematics;

namespace Menial_AC.Source
{
    public class Program : Overlay
    {
        public Program() : base(RandomName())
        {
            // change overlay window name
        }

        public static string RandomName()
        {
            string s;
            Random rnd = new Random();
            int num = rnd.Next(0, 10);
            switch (num)
            {
                default:
                    s = "Menial";
                    break;
                case 0:
                    s = "MalwareBytes";
                    break;
                case 1:
                    s = "Norton360";
                    break;
                case 2:
                    s = "Google Chrome";
                    break;
                case 3:
                    s = "Opera GX";
                    break;
                case 4:
                    s = "Edge";
                    break;
                case 5:
                    s = "Riot Games";
                    break;
                case 6:
                    s = "Epic Games";
                    break;
                case 7:
                    s = "Discord";
                    break;
                case 8:
                    s = "File Explorer";
                    break;
                case 9:
                    s = "Origin";
                    break;
                case 10:
                    s = "UPlay";
                    break;
            }

            return s;
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; public int Top; public int Right; public int Bottom;
        }

        public static RECT GetWindowRect(IntPtr hwnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hwnd, out rect);
            return rect;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        ImDrawListPtr drawList;
        Offsets offsets = new Offsets();
        Menu menu = new Menu();
        Swed mem = new Swed("ac_client");
        IntPtr client = IntPtr.Zero;

        Encoding encoding = Encoding.ASCII;

        // Console
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        // Menu
        Vector4 green = new Vector4(0, 1, 0, 1);
        Vector4 red = new Vector4(1, 0, 0, 1);
        Vector4 pink = new Vector4(1, 0, 1, 1);
        Vector4 white = new Vector4(1, 1, 1, 1);
        Vector4 blue = new Vector4(0, 0, 1, 1);

        Vector2 MenuSize = new Vector2(275, 500);
        Vector2 MenuPos = new Vector2(20, 100);
        bool FirstLaunch = true;
        bool ShowConsole = true;
        bool ShowMenu = true;

        // Hotkeys
        int Mouse5 = 0x06; // mouse 5,
        int Mouse4 = 0x05; // mouse 4
        int CapsLock = 0x14; // Caps Lock
        int Insert = 0x2D; // insert key
        int Home = 0x24; // home key
        int End = 0x23; // end key
        int PageUp = 0x21; // page up key
        int PageDown = 0x22; // page down key
        int SpaceBar = 0x20; // spacebar
        int W = 0x57; // W key
        int A = 0x41; // A key
        int S = 0x53; // S key
        int D = 0x44; // D key
        int C = 0x43; // C key

        // Features
        Entity localPlayer = new Entity();
        List<Entity> friendlies = new List<Entity>();
        List<Entity> enemies = new List<Entity>();

        bool doDebug = true;

        bool aimbot = false;
        bool distanceAimbot = false;
        bool fovAimbot = false;
        float fovAimSize = 30f;
        float fovPos = 10f;
        bool aimAtHead = true;
        bool aimAtChest = false;
        // Define a smooth factor, e.g., 0.1 for slow smoothing, 1 for instant aiming
        float smoothFactor = 1f;

        float nameOffset = 20f;

        public bool triggerBot = true;


        bool esp = true;
        bool teamLine = false;
        bool teamLineBottom = false;
        bool teamLineTop = false;
        bool teamLineCrosshair = false;
        bool teamBox = false;
        bool teamBox3D = true;
        bool teamDotOptions = false;
        bool teamDotFeet = false;
        bool teamDotChest = false;
        bool teamDotHead = true;
        bool teamHealth = false;
        bool teamName = true;

        bool enemyLine = true;
        bool enemyLineBottom = true;
        bool enemyLineTop = false;
        bool enemyLineCrosshair = false;
        bool enemyBox = false;
        bool enemyBox3D = true;
        bool enemyDotOptions = false; 
        bool enemyDotFeet = false;
        bool enemyDotChest = false;
        bool enemyDotHead = true;
        bool enemyHealth = true;
        bool enemyName = true;

        Vector4 teamColor = new Vector4(0, 1, 0, 1);
        Vector4 enemyColor = new Vector4(1, 0, 0, 1);

        // Trainer Features
        bool godmode = false;
        bool infiniteAmmo = false;
        bool weaponCooldown = false;
        bool fireRate = false;
        int fireRateSpeed;
        bool noRecoil = false;
        bool knockbackOptions = false;
        int knockbackAmount;
        bool teleport = false;
        Vector3 teleportPosition = new Vector3();
        string teleportPositionMapName = "Unkown";
        string mapName = "Unknown";
        bool noClip = false;
        float moveSpeed = 1f;
        static double degreeToRadian = Math.PI / 180;
        double anglesOffset = 90 * degreeToRadian;
        bool teleportToCrosshair = false;
        bool teleportTeamToCrosshair = false;
        bool teleportEnemiesToCrosshair = true;
        bool jumpHack = false;

        

        protected override void Render()
        {
            DrawMenu();
            DrawOverlay();
            ESP();

            ImGui.End();
        }

        // Sort menu out
        // 3D esp boxes - done
        // health bar esp - done
        // work out why no recoil breaks aimbot - done

        public void MainLogic()
        {
            IntPtr handle = GetConsoleWindow();
            menu.LoadMenuStyle();

            var window = GetWindowRect(mem.GetProcess().MainWindowHandle);
            menu.windowLocation = new Vector2(window.Left, window.Top);
            menu.windowSize = Vector2.Subtract(new Vector2(window.Right, window.Bottom), menu.windowLocation);
            menu.lineOrigin = new Vector2(menu.windowLocation.X + menu.windowSize.X / 2, window.Bottom);
            menu.windowCenter = new Vector2(menu.lineOrigin.X, window.Bottom - menu.windowSize.Y / 2);

            client = mem.GetModuleBase(".exe");

            localPlayer.address = mem.ReadPointer(client, offsets.localPlayer);

            while (true)
            {
                if (ShowConsole)
                {
                    ShowWindow(handle, SW_SHOW);
                    doDebug = true;
                }
                else
                {
                    ShowWindow(handle, SW_HIDE);
                    doDebug = false;
                }

                ReloadEntities();

                Godmode(godmode);
                UnlimitedAmmo(infiniteAmmo);
                NoRecoil(localPlayer);
                //NoKnockback(localPlayer, knockbackAmount);
                //Firerate(localPlayer, fireRateSpeed);

                Aimbot();

                if (jumpHack)
                {
                    JumpHack();
                }


                Thread.Sleep(2);
            }

        }

        void DrawOverlay()
        {
            ImGui.SetNextWindowSize(menu.windowSize);
            ImGui.SetNextWindowPos(menu.windowLocation);

            ImGui.Begin("overlay", ImGuiWindowFlags.NoMove
                | ImGuiWindowFlags.NoResize
                | ImGuiWindowFlags.NoInputs
                | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoCollapse
                | ImGuiWindowFlags.NoDecoration
                | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoScrollbar
                | ImGuiWindowFlags.NoScrollWithMouse);

        }


        void DrawMenu()
        {
            if (!ShowMenu) return;

            if (FirstLaunch)
            {
                ImGui.SetNextWindowPos(Vector2.Add(menu.windowLocation, MenuPos));
                ImGui.SetNextWindowSize(MenuSize);
                FirstLaunch = false;
            }

            ImGui.Begin("Menial AC", ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);
            ImGui.TextColored(blue, $"Gamemode: {GetGamemode()} | Map: {mapName}");

            if (ImGui.BeginTabBar("Tabs"))
            {
                if (ImGui.BeginTabItem("General"))
                {
                    ImGui.Separator();
                    ImGui.Checkbox("Aimbot", ref aimbot);
                    ImGui.SetItemTooltip("Bound to: Mouse5");
                    if (aimbot)
                    {
                        ImGui.SeparatorText("Aim at");

                        ImGui.Checkbox("Head", ref aimAtHead);
                        ImGui.SameLine();
                        ImGui.Checkbox("Chest", ref aimAtChest);

                        ImGui.SliderFloat("Smooth", ref smoothFactor, 0.1f, 1f);

                        ImGui.SeparatorText("Aim type");
                        ImGui.Checkbox("Distance", ref distanceAimbot);
                        ImGui.SameLine();
                        ImGui.Checkbox("FOV", ref fovAimbot);
                        if (fovAimbot)
                        {
                            ImGui.SliderFloat("FOV", ref fovAimSize, 10, 200);
                        }
                        ImGui.Separator();
                    }
                    
                    ImGui.Checkbox("ESP", ref esp);
                    if (esp)
                    {
                        ImGui.SeparatorText("Team Options");

                        ImGui.Checkbox("Line", ref teamLine);
                        if (teamLine)
                        {
                            ImGui.SameLine();
                            ImGui.Checkbox("Bottom", ref teamLineBottom);
                            ImGui.SameLine();
                            ImGui.Checkbox("Top", ref teamLineTop);
                            ImGui.SameLine();
                            ImGui.Checkbox("X-Hair", ref teamLineCrosshair);
                        }

                        ImGui.Checkbox("Dot", ref teamDotOptions);
                        if (teamDotOptions)
                        {
                            ImGui.SameLine();
                            ImGui.Checkbox("Head", ref teamDotHead);
                            ImGui.SameLine();
                            ImGui.Checkbox("Chest", ref teamDotChest);
                            ImGui.SameLine();
                            ImGui.Checkbox("Feet", ref teamDotFeet);
                        }

                        ImGui.Checkbox("3D Box", ref teamBox3D);
                        ImGui.SameLine();
                        ImGui.Checkbox("Box", ref teamBox);

                        ImGui.Checkbox("Health", ref teamHealth);
                        ImGui.SameLine();
                        ImGui.Checkbox("Name", ref teamName);

                        ImGui.SeparatorText("Enemy Options");

                        ImGui.Checkbox("Line", ref enemyLine);
                        if (enemyLine)
                        {
                            ImGui.SameLine();
                                ImGui.Checkbox("Bottom", ref enemyLineBottom);
                                ImGui.SameLine();
                                ImGui.Checkbox("Top", ref enemyLineTop);
                                ImGui.SameLine();
                                ImGui.Checkbox("Crosshair", ref enemyLineCrosshair);
                        }

                        ImGui.Checkbox("Dot", ref enemyDotOptions);
                        if (enemyDotOptions)
                        {
                            ImGui.SameLine();
                                ImGui.Checkbox("Head", ref enemyDotHead);
                                ImGui.SameLine();
                                ImGui.Checkbox("Chest", ref enemyDotChest);
                                ImGui.SameLine();
                                ImGui.Checkbox("Feet", ref enemyDotFeet);
                        }

                        ImGui.Checkbox("3D Box", ref enemyBox3D);
                        ImGui.SameLine();
                        ImGui.Checkbox("Box", ref enemyBox);

                        ImGui.Checkbox("Health", ref enemyHealth);
                        ImGui.SameLine();
                        ImGui.Checkbox("Name", ref enemyName);
                        ImGui.Separator();
                    }
                    ImGui.Checkbox("No Recoil", ref noRecoil);
                    /*ImGui.Checkbox("Knockback Options", ref knockbackOptions);
                    if (knockbackOptions)
                    {
                        ImGui.SliderInt("Knockback Amount", ref knockbackAmount, 0, 999);
                        if (ImGui.Button("Mega Knockback"))
                        {
                            knockbackAmount = 999;
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("No Knockback"))
                        {
                            knockbackAmount = 0;
                        }
                        ImGui.Separator();
                    }*/


                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Trainer"))
                {
                    ImGui.Checkbox("Godmode", ref godmode);
                    ImGui.Checkbox("Infinite Ammo", ref infiniteAmmo);
                    ImGui.Checkbox("Jump Hack", ref jumpHack);
                    /*ImGui.Checkbox("Fire Rate", ref fireRate);
                    if (fireRate)
                    {
                        ImGui.SliderInt("Fire Rate", ref fireRateSpeed, 0, 999);
                        ImGui.SetItemTooltip("Faster <-> Slower");
                        if (ImGui.Button("Mega Firerate"))
                        {
                            fireRateSpeed = 0;
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Slow-Mo"))
                        {
                            fireRateSpeed = 999;
                        }
                        ImGui.Separator();
                    }*/
                    ImGui.Checkbox("Teleport Options", ref teleport);
                    if (teleport)
                    {
                        ImGui.Text("Current Position: \n" + localPlayer.playerPos.ToString());
                        if (teleportPosition != Vector3.Zero)
                        {
                            ImGui.Separator();
                            ImGui.Text("Saved Position: \n" + teleportPosition.ToString());
                        }
                        ImGui.Separator();
                        if (ImGui.Button("Save Position"))
                        {
                            SavePosition(localPlayer);
                        }
                        ImGui.SetItemTooltip("Bound to: PageUp");
                        ImGui.SameLine();
                        if (ImGui.Button("Load Position"))
                        {
                            LoadPosition(localPlayer);
                        }
                        ImGui.SetItemTooltip("Bound to: PageDown");
                    }
                    ImGui.Checkbox("No-Clip", ref noClip);
                    ImGui.SetItemTooltip("Bound to: Mouse4\nWASD to Move");
                    if (noClip)
                    {
                        ImGui.SliderFloat("Speed", ref moveSpeed, 1f, 10f);
                        ImGui.SameLine();
                        if(ImGui.Button("Reset"))
                        {
                            moveSpeed = 1f;
                        }
                    }
                    ImGui.Checkbox("TP to X-Hair", ref teleportToCrosshair);
                    if (teleportToCrosshair)
                    {
                        ImGui.SameLine();
                        ImGui.Checkbox("Team", ref teleportTeamToCrosshair);
                        ImGui.SameLine();
                        ImGui.Checkbox("Enemies", ref teleportEnemiesToCrosshair);
                    }

                    ImGui.EndTabItem();
                }
            }


        }


        void Init_Menu()
        {
            var window = GetWindowRect(mem.GetProcess().MainWindowHandle);
            menu.windowLocation = new Vector2(window.Left, window.Top);
            menu.windowSize = Vector2.Subtract(new Vector2(window.Right, window.Bottom), menu.windowLocation);
            menu.lineOrigin = new Vector2(menu.windowLocation.X + menu.windowSize.X / 2, window.Bottom);
            menu.windowCenter = new Vector2(menu.lineOrigin.X, window.Bottom - menu.windowSize.Y / 2);

        }

        void Init_Hotkeys()
        {

            while (true)
            {
                if (GetAsyncKeyState(Insert) < 0)
                {
                    ShowMenu = !ShowMenu;
                    Console.WriteLine("-> Toggled Menu");
                    Thread.Sleep(200);
                }

                if (GetAsyncKeyState(Home) < 0)
                {
                    ShowConsole = !ShowConsole;
                    Console.WriteLine("-> Toggled Console");
                    Thread.Sleep(200);
                }

                if (GetAsyncKeyState(End) < 0)
                {
                    Console.WriteLine("-> Closing Menial AC...");
                    Thread.Sleep(200);
                    this.Close();
                }

                if (teleport)
                {
                    if(GetAsyncKeyState(PageUp) < 0)
                    {
                        SavePosition(localPlayer);
                        Thread.Sleep(200);
                    }

                    if (GetAsyncKeyState(PageDown) < 0)
                    {
                        LoadPosition(localPlayer);
                        Thread.Sleep(200);
                    }
                }

                if (noClip)
                {
                    if (GetAsyncKeyState(Mouse4) < 0)
                    {
                        mem.WriteBytes(client + offsets.moveInstructionAddress, offsets.moveInstructionNop);
                        Noclip(localPlayer);
                        Thread.Sleep(10);
                    }
                    else
                    {
                        mem.WriteBytes(client + offsets.moveInstructionAddress, offsets.moveInstructionOriginal);
                    }
                }

                if (teleportToCrosshair)
                {
                    if (GetAsyncKeyState(CapsLock) < 0)
                    {
                        TeleportToCrosshair(teleportTeamToCrosshair, teleportEnemiesToCrosshair);
                    }
                }

                Thread.Sleep(2);
            }
        }
        void Godmode(bool enabled)
        {
            if (enabled)
            {
                mem.WriteInt(localPlayer.address, offsets.Health, 999);
            }
            else
            {
                return;
            }
        }
        void UnlimitedAmmo(bool enabled)
        {
            if (enabled)
            {
                IntPtr weaponInfoClass = mem.ReadPointer(localPlayer.address, offsets.weaponInfoClass, offsets.currentWeaponClass);
                mem.WriteInt(weaponInfoClass, offsets.gunClipAmmo, 999);
            }
        }
        void SavePosition(Entity entity)
        {
            teleportPosition = entity.playerPos;
            Console.WriteLine($"Saved: {teleportPosition.ToString()} for {entity.name}");
            teleportPositionMapName = mapName;
        }
        void LoadPosition(Entity entity)
        {
            mem.WriteVec(entity.address, offsets.PlayerPos, teleportPosition);
            Console.WriteLine($"Teleported to: {teleportPosition.ToString()} for {entity.name}");
        }
        void Noclip(Entity entity)
        {
            bool forward = GetAsyncKeyState(W) < 0;
            bool backward = GetAsyncKeyState(S) < 0;
            bool left = GetAsyncKeyState(A) < 0;
            bool right = GetAsyncKeyState(D) < 0;
            bool up = GetAsyncKeyState(SpaceBar) < 0;
            bool down = GetAsyncKeyState(C) < 0;
            if (entity.isDead == 1)
                return;
            Vector2 viewAngles = new Vector2(entity.yaw, entity.pitch);

            // Variables for addition, set them to current.
            float newX = entity.playerPos.X;
            float newY = entity.playerPos.Y;
            float newZ = entity.playerPos.Z;

            // Calculate new locations
            float forwardX = (float)(moveSpeed * Math.Cos(viewAngles.X * degreeToRadian - anglesOffset));
            float forwardY = (float)(moveSpeed * Math.Sin(viewAngles.X * degreeToRadian - anglesOffset));
            float forwardZ = (float)(moveSpeed * Math.Sin(viewAngles.Y * degreeToRadian)); // no need to offset

            // sideways

            float rightX = (float)(moveSpeed * Math.Sin(viewAngles.X * degreeToRadian - anglesOffset));
            float rightY = (float)(moveSpeed * Math.Cos(viewAngles.X * degreeToRadian - anglesOffset));
            if (forward)
            {
                newX += forwardX;
                newY += forwardY;
                newZ += forwardZ;
            }

            if (backward)
            {
                newX -= forwardX;
                newY -= forwardY;
                newZ -= forwardZ;
            }

            if (right)
            {
                newX -= rightX;
                newY += rightY;
            }

            if (left)
            {
                newX += rightX;
                newY -= rightY;
            }

            Vector3 newPosition = new Vector3(newX, newY, newZ);
            mem.WriteVec(entity.address, offsets.PlayerPos, newPosition);
        }

        void NoRecoil(Entity entity)
        {
            if (entity.isDead == 1) return;

            int[] original = GetWeaponInfo(null, entity.weaponID);
            IntPtr gunInfoClass = mem.ReadPointer(entity.address, offsets.weaponInfoClass, offsets.gunInfoClass);

            if (noRecoil)
            {
                mem.WriteInt(gunInfoClass, offsets.recoil, 0);
            }
            else
            {
                mem.WriteInt(gunInfoClass, offsets.recoil, original[2]);
            }
        }

        void NoKnockback(Entity entity, int knockbackAmount)
        {
            if (entity.isDead == 1) return;

            int[] original = GetWeaponInfo(null, entity.weaponID);
            IntPtr gunInfoClass = mem.ReadPointer(entity.address, offsets.weaponInfoClass, offsets.gunInfoClass);

            if (knockbackOptions)
            {
                mem.WriteInt(gunInfoClass, offsets.knockback, knockbackAmount);
            }
            else
            {
                mem.WriteInt(gunInfoClass, offsets.knockback, original[1]);
                knockbackAmount = original[1];
            }
        }

        void Firerate(Entity entity, int animSpeed)
        {
            if (entity.isDead == 1) return;

            int[] original = GetWeaponInfo(null, entity.weaponID);
            IntPtr gunInfoClass = mem.ReadPointer(entity.address, offsets.weaponInfoClass, offsets.gunInfoClass);

            if (fireRate)
            {
                mem.WriteInt(gunInfoClass, offsets.animationSpeed, animSpeed);
            }
            else
            {
                mem.WriteInt(gunInfoClass, offsets.animationSpeed, original[0]);
                fireRateSpeed = original[0];
            }


        }

        string GetWeaponName(Entity entity)
        {
            string weaponName = "Unknown";

            if (entity.address == 0) return weaponName;
            if (entity.isDead == 1) return weaponName;

            switch(entity.weaponID)
            {
                default:
                    weaponName = "Unknown";
                    break;
                case 0:
                    weaponName = "Knife";
                    break;
                case 1:
                    weaponName = "Pistol";
                    break;
                case 2:
                    weaponName = "Carbine";
                    break;
                case 3:
                    weaponName = "Shotgun";
                    break;
                case 4:
                    weaponName = "Subgun";
                    break;
                case 5:
                    weaponName = "Sniper";
                    break;
                case 6:
                    weaponName = "Assault";
                    break;
                case 7:
                    weaponName = "Grenade";
                    break;
                case 8:
                    weaponName = "Akimbo";
                    break;
            }

            return weaponName;
        }

        int[] GetWeaponInfo(string? weaponName, int? weaponID)
        {
            int animationSpeed; int knockback; int recoil;

            int[] weaponInfo = new int[3];

            if (weaponName != null)
            {
                switch (weaponName)
                {
                    default:
                        break;
                    case "Knife":
                        animationSpeed = 500;
                        knockback = 1;
                        recoil = 0;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Pistol":
                        animationSpeed = 160;
                        knockback = 10;
                        recoil = 58;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Carbine":
                        animationSpeed = 720;
                        knockback = 60;
                        recoil = 60;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Shotgun":
                        animationSpeed = 880;
                        knockback = 35;
                        recoil = 140;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Subgun":
                        animationSpeed = 80;
                        knockback = 15;
                        recoil = 50;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Sniper":
                        animationSpeed = 1500;
                        knockback = 50;
                        recoil = 85;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Assault":
                        animationSpeed = 120;
                        knockback = 30;
                        recoil = 50;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Grenade":
                        animationSpeed = 650;
                        knockback = 1;
                        recoil = 0;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case "Akimbo":
                        animationSpeed = 80;
                        knockback = 10;
                        recoil = 25;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                }
            }

            if (weaponID != null)
            {
                switch (weaponID)
                {
                    default:
                        break;
                    case 0:
                        animationSpeed = 500;
                        knockback = 1;
                        recoil = 0;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 1:
                        animationSpeed = 160;
                        knockback = 10;
                        recoil = 58;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 2:
                        animationSpeed = 720;
                        knockback = 60;
                        recoil = 60;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 3:
                        animationSpeed = 880;
                        knockback = 35;
                        recoil = 140;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 4:
                        animationSpeed = 80;
                        knockback = 15;
                        recoil = 50;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 5:
                        animationSpeed = 1500;
                        knockback = 50;
                        recoil = 85;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 6:
                        animationSpeed = 120;
                        knockback = 30;
                        recoil = 50;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 7:
                        animationSpeed = 650;
                        knockback = 1;
                        recoil = 0;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                    case 8:
                        animationSpeed = 80;
                        knockback = 10;
                        recoil = 25;
                        weaponInfo[0] = animationSpeed;
                        weaponInfo[1] = knockback;
                        weaponInfo[2] = recoil;
                        break;
                }

            }

            return weaponInfo;
        }



        string GetGamemode()
        {
            int gamemodeID = mem.ReadInt(client, offsets.gameMode);
            string gamemode = "Unknown";
            switch(gamemodeID)
            {
                default:
                    break;
                case 7:
                    gamemode = "TDM";
                    break;
                case 8:
                    gamemode = "DM";
                    break;
                case 21:
                    gamemode = "TOSOK";
                    break;
                case 20:
                    gamemode = "Team Survivor";
                    break;
                case 19:
                    gamemode = "Last Swiss Standing";
                    break;
                case 18:
                    gamemode = "Pistol Frenzy";
                    break;
                case 12:
                    gamemode = "OSOK";
                    break;
                case 5:
                    gamemode = "CTF";
                    break;

            }

            return gamemode;
        }

        string GetTeamName(Entity entity)
        {
            string teamName = "Unknown";

            switch(entity.team)
            {
                default:
                    break;
                case 0:
                    teamName = "CLA";
                    break;
                case 1:
                    teamName = "RVSF";
                    break;
                case 4:
                    teamName = "Spectator";
                    break;
            }

            return teamName;
        }

        void TeleportToCrosshair(bool team, bool enemy)
        {
            Vector3 crosshairPos = new Vector3(localPlayer.playerPos.X, localPlayer.playerPos.Y + 10f, localPlayer.playerPos.Z);
            try
            {
                if (team)
                {
                    foreach (var entity in friendlies.ToList())
                    {
                        mem.WriteVec(entity.address, offsets.PlayerPos, crosshairPos);
                    }
                }
                if (enemy)
                {
                    foreach (var entity in enemies.ToList())
                    {
                        mem.WriteVec(entity.address, offsets.PlayerPos, crosshairPos);
                    }
                }
            }
            catch { }
        }


        void UpdateEntity(Entity entity)
        {
            entity.isDead = mem.ReadInt(entity.address, offsets.Dead);

            if (entity.isDead == 1)
                return;

            entity.headPos = mem.ReadVec(entity.address, offsets.HeadPos);
            entity.playerPos = mem.ReadVec(entity.address, offsets.PlayerPos);
            entity.chestPos = GetChestPos(entity);
            entity.isGrounded = mem.ReadInt(entity.address, offsets.Grounded);
            entity.health = mem.ReadInt(entity.address, offsets.Health);
            entity.team = mem.ReadInt(entity.address, offsets.TeamID);
            entity.teamName = GetTeamName(entity);

            entity.pitch = mem.ReadFloat(entity.address, offsets.Pitch);
            entity.yaw = mem.ReadFloat(entity.address, offsets.Yaw);

            IntPtr entityWeaponClass = mem.ReadPointer(entity.address, offsets.weaponInfoClass);
            entity.weaponID = mem.ReadInt(entityWeaponClass, offsets.equippedWeaponID);
            entity.weaponName = GetWeaponName(entity);
            entity.weaponInfo = GetWeaponInfo(null, entity.weaponID);

            string nameBytes = encoding.GetString(mem.ReadBytes(entity.address, offsets.Name, 16));
            entity.name = RemoveSpecialCharacters(nameBytes);

            var currentViewMatrix = ReadMatrix();

            
            entity.originScreenPosition = Vector2.Add(World2Screen(currentViewMatrix, entity.playerPos, (int)menu.windowSize.X, (int)menu.windowSize.Y), menu.windowLocation);
            entity.absScreenPosition = Vector2.Add(World2Screen(currentViewMatrix, entity.headPos, (int)menu.windowSize.X, (int)menu.windowSize.Y), menu.windowLocation);

            entity.chestScreenPosition = Vector2.Add(World2Screen(currentViewMatrix, GetChestPos(entity), (int)menu.windowSize.X, (int)menu.windowSize.Y), menu.windowLocation);

            entity.pixelDistance = Vector2.Distance(entity.absScreenPosition, menu.windowCenter);
            entity.pixelDistanceChest = Vector2.Distance(entity.chestScreenPosition, menu.windowCenter);

            entity.magnitude = CalculateMagnitude(localPlayer, entity);

            if (doDebug)
            {
                IntPtr gunInfoClass = mem.ReadPointer(entity.address, offsets.weaponInfoClass, offsets.gunInfoClass);
                int animationSpeed = mem.ReadInt(gunInfoClass, offsets.animationSpeed);
                int knockback = mem.ReadInt(gunInfoClass, offsets.knockback);
                int recoil = mem.ReadInt(gunInfoClass, offsets.recoil);

                Console.Clear();
                /*Console.WriteLine($"\nwindowLoc: {menu.windowLocation} | windowSize: {menu.windowSize} | lineOrigin: {menu.lineOrigin} | windowCenter: {menu.windowCenter}");
                Console.WriteLine($"{client + offsets.viewMatrix:X} | \nwindowLoc: {menu.windowLocation} | windowSize: {menu.windowSize} | lineOrigin: {menu.lineOrigin} | windowCenter: {menu.windowCenter}");*/
                //Console.WriteLine($"{localPlayer.name} | Team: {localPlayer.teamName} [{localPlayer.team}] -> hPos: {localPlayer.headPos} | pPos: {localPlayer.playerPos} | Pitch: {localPlayer.pitch} | Yaw: {localPlayer.yaw} | Health: {localPlayer.health} | TeamID: {localPlayer.team} | {localPlayer.weaponName} [{localPlayer.weaponID}] Anim: {animationSpeed} [{localPlayer.weaponInfo[0]}] Knockback: {knockback} [{localPlayer.weaponInfo[1]}] Recoil: {recoil} [{localPlayer.weaponInfo[2]}]");
                Console.WriteLine($"Gamemode: {GetGamemode()}");
                Console.WriteLine("\nEnemies\n");
                foreach (var ent in enemies)
                {
                    Console.WriteLine($"{ent.name} -> {ent.health} -> {ent.weaponName} | Distance: {CalculateDistance(localPlayer, ent)} | HeadPos: {ent.headPos} |\n Angles To: {CalculateAngles(localPlayer, ent)} | W2S: {entity.absScreenPosition}");
                }
                Console.WriteLine("\nFriendlies\n");
                foreach (var ent in friendlies)
                {

                    Console.WriteLine($"{ent.name} -> {ent.health} -> {ent.weaponName} | Distance: {CalculateDistance(localPlayer, ent)} | HeadPos: {ent.headPos} |\n Angles To: {CalculateAngles(localPlayer, ent)} | W2S: {entity.absScreenPosition}");
                }
            }

        }

        public Vector3 GetChestPos(Entity entity)
        {
            // Calculate the vector from playerPos to headPos
            Vector3 direction = entity.headPos - entity.playerPos;
            // Assume the chest is about 0.5 to 0.6 of the way up from the playerPos
            return entity.playerPos + direction * 0.55f;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9_.!*(){}£$@#]+", "", RegexOptions.Compiled);
        }

        string GetMap()
        {
            string mapBytes = encoding.GetString(mem.ReadBytes(client, offsets.mapName, 16));
            mapName = mapBytes;

            return mapName;
        }

        void JumpHack()
        {
            mem.WriteInt(localPlayer.address, offsets.Grounded, 1);
        }

        void ReloadEntities()
        {
            GetMap(); // update current map

            friendlies.Clear();
            enemies.Clear();

            localPlayer.address = mem.ReadPointer(client, offsets.localPlayer);
            UpdateEntity(localPlayer);
            UpdateEntities();

            if (aimbot)
            {
                if (distanceAimbot)
                {
                    enemies = enemies.OrderBy(o => o.magnitude).ToList();
                }

                if (fovAimbot)
                {
                    enemies = enemies.OrderBy(o => o.pixelDistance).ToList();
                }

            }
        }

        void UpdateEntities()
        {
            for (int i = 0; i < 16; i++)
            {
                Entity entity = new Entity();
                entity.address = mem.ReadPointer(client, offsets.entityList, i * 0x4);

                if (entity.address == IntPtr.Zero)
                    continue;

                UpdateEntity(entity);

                if (entity.isDead == 1)
                    continue;

                if (GetGamemode() == "TDM" || GetGamemode() == "TOSOK" || GetGamemode() == "Team Survivor" || GetGamemode() == "Unknown" || GetGamemode() == "CTF")
                {
                    if (entity.team == localPlayer.team && entity.health > 0)
                        friendlies.Add(entity);

                    if (entity.team != localPlayer.team && entity.health > 0)
                        enemies.Add(entity);
                }
                else
                {
                    if (entity.health > 0)
                    {
                        enemies.Add(entity);
                    }
                }


            }
        }

        public void ESP()
        {
            drawList = ImGui.GetWindowDrawList();

            if (esp)
            {
                try
                {
                    foreach (var entity in friendlies.ToList())
                    {
                        if (IsPixelInsideScreen(entity.originScreenPosition) &&  entity.isDead == 0)
                        {
                            DrawVisuals(entity, teamColor, teamLineBottom, teamLineTop, teamLineCrosshair, teamBox, teamBox3D, teamDotFeet, teamDotChest, teamDotHead, teamHealth, teamName);
                        }
                    }

                    foreach (var entity in enemies.ToList())
                    {
                        if (IsPixelInsideScreen(entity.originScreenPosition) && entity.isDead == 0)
                        {
                            DrawVisuals(entity, enemyColor, enemyLineBottom, enemyLineTop, enemyLineCrosshair, enemyBox, enemyBox3D, enemyDotFeet, enemyDotChest, enemyDotHead, enemyHealth, enemyName);
                        }
                    }
                }
                catch { }
            }

            if (aimbot && fovAimbot)
            {
                drawList.AddCircle(new Vector2(menu.windowCenter.X, menu.windowCenter.Y + fovPos), fovAimSize, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)));
            }

            drawList.AddText(new Vector2(menu.windowLocation.X + menu.windowSize.X - 100f, menu.windowLocation.Y + 40f), ImGui.ColorConvertFloat4ToU32(white), "Menial AC");

            if (teleportPosition != Vector3.Zero)
            {
                if (encoding.GetString(mem.ReadBytes(client, offsets.mapName, 16)) != teleportPositionMapName)
                {
                    teleportPosition = Vector3.Zero;
                }
                Vector2 drawPos = Vector2.Add(World2Screen(ReadMatrix(), teleportPosition, (int)menu.windowSize.X, (int)menu.windowSize.Y), menu.windowLocation);
                drawList.AddCircle(drawPos, 3f, ImGui.ColorConvertFloat4ToU32(green));
            }
        }

        void DrawVisuals(Entity entity, Vector4 color, bool lineBottom, bool lineTop, bool lineCrosshair, bool box, bool box_3d, bool feetDot, bool chestDot, bool headDot, bool health, bool name)
        {

            if (IsPixelInsideScreen(entity.originScreenPosition))
            {
                if (lineBottom)
                {
                    drawList.AddLine(menu.lineOrigin, entity.originScreenPosition, ImGui.ColorConvertFloat4ToU32(color), 2);
                }
                if (lineTop)
                {
                    drawList.AddLine(new Vector2(menu.lineOrigin.X, menu.lineOrigin.Y / 4), entity.originScreenPosition, ImGui.ColorConvertFloat4ToU32(color), 2);
                }
                if (lineCrosshair)
                {
                    drawList.AddLine(new Vector2(menu.windowCenter.X, menu.windowCenter.Y + 6f), entity.originScreenPosition, ImGui.ColorConvertFloat4ToU32(color), 2);
                }
                if (box)
                {
                    Vector2 width = new Vector2((entity.originScreenPosition.Y - entity.absScreenPosition.Y) / 4, 2f); // /3 looks better
                    drawList.AddRect(Vector2.Subtract(entity.absScreenPosition, width), Vector2.Add(entity.originScreenPosition, width), ImGui.ColorConvertFloat4ToU32(color), 0);
                }
                if (box_3d)
                {
                    // Define the dimensions of the player's bounding box
                    float boxWidth = 1f; // Adjust as needed - width
                    float boxHeight = 1.5f; // Adjust as needed (roughly the height of a player) - depth
                    float boxDepth = 2.5f; // Adjust as needed  - height
                    float thickness = 2f;

                    ViewMatrix currentViewMatrix = ReadMatrix();

                    // Define the 8 corners of the bounding box in local space
                    Vector3[] corners = new Vector3[]
                    {
                         new Vector3(-boxWidth, 0, -boxDepth), // Bottom-back-left
                         new Vector3(boxWidth, 0, -boxDepth), // Bottom-back-right
                         new Vector3(-boxWidth, 0, boxDepth), // Bottom-front-left
                         new Vector3(boxWidth, 0, boxDepth), // Bottom-front-right
                         new Vector3(-boxWidth, boxHeight, -boxDepth), // Top-back-left
                         new Vector3(boxWidth, boxHeight, -boxDepth), // Top-back-right
                         new Vector3(-boxWidth, boxHeight, boxDepth), // Top-front-left
                         new Vector3(boxWidth, boxHeight, boxDepth), // Top-front-right
                    };

                    // Calculate the 3D positions of the corners in world space
                    Vector3[] worldCorners = new Vector3[8];
                    for (int i = 0; i < 8; i++)
                    {
                        worldCorners[i] = new Vector3(entity.chestPos.X, entity.chestPos.Y - 0.75f, entity.chestPos.Z) + corners[i];
                    }

                    // Project the 3D positions to 2D screen coordinates
                    Vector2[] screenCorners = new Vector2[8];
                    for (int i = 0; i < 8; i++)
                    {
                        screenCorners[i] = Vector2.Add(World2Screen(currentViewMatrix, worldCorners[i], (int)menu.windowSize.X, (int)menu.windowSize.Y), menu.windowLocation);
                    }

                    // Draw the edges of the box
                    DrawLine(screenCorners[0], screenCorners[1], color, thickness); // Bottom-back
                    DrawLine(screenCorners[1], screenCorners[3], color, thickness); // Bottom-right
                    DrawLine(screenCorners[3], screenCorners[2], color, thickness); // Bottom-front
                    DrawLine(screenCorners[2], screenCorners[0], color, thickness); // Bottom-left

                    DrawLine(screenCorners[4], screenCorners[5], color, thickness); // Top-back
                    DrawLine(screenCorners[5], screenCorners[7], color, thickness); // Top-right
                    DrawLine(screenCorners[7], screenCorners[6], color, thickness); // Top-front
                    DrawLine(screenCorners[6], screenCorners[4], color, thickness); // Top-left

                    DrawLine(screenCorners[0], screenCorners[4], color, thickness); // Back-left
                    DrawLine(screenCorners[1], screenCorners[5], color, thickness); // Back-right
                    DrawLine(screenCorners[2], screenCorners[6], color, thickness); // Front-left
                    DrawLine(screenCorners[3], screenCorners[7], color, thickness); // Front-right

                }

                if (feetDot)
                {
                    drawList.AddCircleFilled(entity.originScreenPosition, 3, ImGui.ColorConvertFloat4ToU32(color));
                }
                if (chestDot)
                {
                    drawList.AddCircleFilled(entity.chestScreenPosition, 3, ImGui.ColorConvertFloat4ToU32(color));
                }

                if (headDot)
                {
                    drawList.AddCircleFilled(new Vector2(entity.absScreenPosition.X, entity.absScreenPosition.Y + 3f), 5, ImGui.ColorConvertFloat4ToU32(color));
                    
                }
                if (health)
                {
                    Vector4 barColor, textColor;

                    float entityHeight = entity.originScreenPosition.Y - entity.absScreenPosition.Y;

                    float boxLeft = entity.absScreenPosition.X - entityHeight / 3;
                    float boxRight = entity.originScreenPosition.X + entityHeight / 3;

                    float barPercentWidth = 0.05f;
                    float barPixelWidth = barPercentWidth + (boxRight - boxLeft) / 6;

                    float barHeight = entityHeight * (entity.health / 100f);

                    Vector2 barTop = new Vector2(boxLeft - barPixelWidth, entity.originScreenPosition.Y - barHeight);
                    Vector2 barBottom = new Vector2(boxLeft, entity.originScreenPosition.Y);


                    barColor = new Vector4(0, 1, 0, 0.6f);
                    textColor = new Vector4(1, 0, 0, 0.7f);

                    drawList.AddRectFilled(barTop, barBottom, ImGui.ColorConvertFloat4ToU32(barColor));
                    drawList.AddText(new Vector2(barTop.X - 5f, barTop.Y - 10f), ImGui.ColorConvertFloat4ToU32(textColor), entity.health.ToString()); // new Vector2(entity.originScreenPosition.X, entity.originScreenPosition.Y + 12f)
                }
                if (name)
                {
                    if (entity.name.Length > 0 && entity.name.Length < 8)
                        nameOffset = 10f;
                    if (entity.name.Length >= 8 && entity.name.Length < 12)
                        nameOffset = 30f;
                    if (entity.name.Length >= 12 && entity.name.Length < 20)
                        nameOffset = 50f;
                    drawList.AddText(new Vector2(entity.originScreenPosition.X - nameOffset, entity.originScreenPosition.Y + 8f), ImGui.ColorConvertFloat4ToU32(color), entity.name);
                }
            }
        }

        public void DrawLine(Vector2 start, Vector2 end, Vector4 color, float thickness)
        {
            // Implement your line drawing method here
            // This typically involves interfacing with your graphics/overlay library
            drawList.AddLine(start, end, ImGui.ColorConvertFloat4ToU32(color), thickness);
        }


        bool IsPixelInsideScreen(Vector2 pixel)
        {
            return pixel.X > menu.windowLocation.X && pixel.X < menu.windowLocation.X + menu.windowSize.X && pixel.Y > menu.windowLocation.Y && pixel.Y < menu.windowSize.Y + menu.windowLocation.Y;
        }

        public void Aimbot()
        {
            if (aimbot)
            {
                if (GetAsyncKeyState(Mouse5) < 0)
                {
                    if (enemies.Count > 0)
                    {
                        Entity targetEntity = null;
                        Vector2 targetAngles = Vector2.Zero;

                        if (distanceAimbot)
                        {
                            foreach (var entity in enemies)
                            {
                                targetAngles = CalculateAngles(localPlayer, entity);
                                targetEntity = entity;
                                break;
                            }
                        }
                        else if (fovAimbot)
                        {
                            foreach (var entity in enemies)
                            {
                                if (entity.pixelDistance < fovAimSize || entity.pixelDistanceChest < fovAimSize)
                                {
                                    targetAngles = CalculateAngles(localPlayer, entity);
                                    targetEntity = entity;
                                    break;
                                }
                            }
                        }

                        if (targetEntity != null)
                        {

                            AimAt(localPlayer, targetAngles, smoothFactor);
                        }
                    }
                }
            }
        }


        public Vector2 Lerp(Vector2 start, Vector2 end, float t)
        {
            t = Math.Clamp(t, 0f, 1f); // Ensure t is within the range [0, 1]
            return start + t * (end - start);
        }


        public void AimAt(Entity ent, Vector2 targetAngles, float smoothFactor)
        {
            Vector2 currentAngles = new Vector2(ent.yaw, ent.pitch);
            currentAngles = Lerp(currentAngles, targetAngles, smoothFactor);

            if (aimAtHead)
            {
                mem.WriteFloat(ent.address, offsets.Yaw, currentAngles.X);
                mem.WriteFloat(ent.address, offsets.Pitch, currentAngles.Y);
            }
            if (aimAtChest)
            {
                mem.WriteFloat(ent.address, offsets.Yaw, currentAngles.X);
                mem.WriteFloat(ent.address, offsets.Pitch, currentAngles.Y - 0.2f);
            }

        }

        public Vector2 CalculateAngles(Entity localPlayer, Entity destEnt)
        {
            float x, y, hyp, deltaX, deltaY, deltaZ;

            deltaX = destEnt.headPos.X - localPlayer.headPos.X;
            deltaY = destEnt.headPos.Y - localPlayer.headPos.Y;

            x = (float)(Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90;


            deltaZ = destEnt.headPos.Z - localPlayer.headPos.Z; // add chest pos

            float dist = CalculateDistance(localPlayer, destEnt);

            hyp = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            y = (float)(Math.Atan2(deltaZ, hyp) * 180 / Math.PI);

            //y = (float)(Math.Atan2(deltaZ, dist) + 180 / Math.PI);

            return new Vector2(x, y);
        }

        public static float CalculateDistance(Entity localPlayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.playerPos.X - localPlayer.playerPos.X, 2) 
                + Math.Pow(destEnt.playerPos.Y - localPlayer.playerPos.Y, 2));
        }

        public static float CalculateMagnitude(Entity localPlayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.playerPos.X - localPlayer.playerPos.X, 2) +
                Math.Pow(destEnt.playerPos.Y - localPlayer.playerPos.Y, 2) +
                Math.Pow(destEnt.playerPos.Z - localPlayer.playerPos.Z, 2)
                );
        }

        Vector2 WorldToScreen(ViewMatrix matrix, Vector3 pos, int width, int height)
        {
            var screenCoordinates = new Vector2();

            // calculate screenW
            float screenW = (matrix.m14 * pos.X) + (matrix.m24 * pos.Y) + (matrix.m34 * pos.Z) + matrix.m44;

            if (screenW > 0.001f)
            {
                //calculate screenX and Y
                float screenX = (matrix.m11 * pos.X) + (matrix.m21 * pos.Y) + (matrix.m31 * pos.Z) + matrix.m41;
                float screenY = (matrix.m12 * pos.X) + (matrix.m22 * pos.Y) + (matrix.m32 * pos.Z) + matrix.m34;
                // calculate camera center
                float camX = width / 2;
                float camY = height / 2;

                // perform perspective divison and transformation
                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                // return x and y
                screenCoordinates.X = X;
                screenCoordinates.Y = Y;
                return screenCoordinates;
            }
            else
            {
                return new Vector2(-99, -99);
            }
        } // Doesn't work

        public Vector2 World2Screen(ViewMatrix matrix, Vector3 worldPos, int width, int height)
        {

            //multiply vector against matrix
            float screenX = (matrix.m11 * worldPos.X) + (matrix.m21 * worldPos.Y) + (matrix.m31 * worldPos.Z) + matrix.m41;
            float screenY = (matrix.m12 * worldPos.X) + (matrix.m22 * worldPos.Y) + (matrix.m32 * worldPos.Z) + matrix.m42;
            float screenW = (matrix.m14 * worldPos.X) + (matrix.m24 * worldPos.Y) + (matrix.m34 * worldPos.Z) + matrix.m44;

            Vector2 screenPos = new Vector2();

            //camera position (eye level/middle of screen)
            float camX = width / 2f;
            float camY = height / 2f;

            //convert to homogeneous position
            float x = camX + (camX * screenX / screenW);
            float y = camY - (camY * screenY / screenW);
            screenPos = new Vector2(x, y);

            //check if object is behind camera / off screen (not visible)
            //w = z where z is relative to the camera 
            if (screenW > 0.001f)
            {
                return screenPos;
            }
            else
            {
                return new Vector2(-99, -99);
            }
        }

        ViewMatrix ReadMatrix()
        {
            var viewMatrix = new ViewMatrix();
            //var matrixPointer = mem.ReadPointer(client, offsets.viewMatrix);

            var matrix = mem.ReadMatrix(client + offsets.viewMatrix);

            viewMatrix.m11 = matrix[0];
            viewMatrix.m12 = matrix[1];
            viewMatrix.m13 = matrix[2];
            viewMatrix.m14 = matrix[3];

            viewMatrix.m21 = matrix[4];
            viewMatrix.m22 = matrix[5];
            viewMatrix.m23 = matrix[6];
            viewMatrix.m24 = matrix[7];

            viewMatrix.m31 = matrix[8];
            viewMatrix.m32 = matrix[9];
            viewMatrix.m33 = matrix[10];
            viewMatrix.m34 = matrix[11];

            viewMatrix.m41 = matrix[12];
            viewMatrix.m42 = matrix[13];
            viewMatrix.m43 = matrix[14];
            viewMatrix.m44 = matrix[15];

            return viewMatrix;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Menial AC\n");

            Program program = new Program();
            program.Start().Wait();
            
            Thread MainLogicThread = new Thread(program.MainLogic) { IsBackground = true };
            Thread MenuThread = new Thread(program.Init_Menu) { IsBackground = true };
            Thread HotkeyThread = new Thread(program.Init_Hotkeys) { IsBackground = true };

            MainLogicThread.Start();
            MenuThread.Start();
            HotkeyThread.Start();

        }
    }

}
