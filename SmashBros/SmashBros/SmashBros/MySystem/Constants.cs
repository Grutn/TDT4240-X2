using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmashBros.Model;
using Microsoft.Xna.Framework;

namespace SmashBros.MySystem
{
    public static class Constants
    {
        public const bool DebugMode = true;
        public const bool StartGameplay = !true;
        public const bool Music = !true;
        public const bool ZoomMin = !true;
        public const int WindowWidth = 1280;
        public const int WindowHeight = 720;
        public const bool FullScreen = false;

        public const int FPS = 20;
        public const float MaxZoom = 1.9f;
        public const float MinZoom = 0.5f;
        public const int ThumbWith = 210;
        public const int ThumbHeight = 210;
        public const float MaxCursorSpeed = 10f;
        public const float GamePlayGravity = 20f;


        private CharacterStats Wolverine()
        {
            CharacterStats c = new CharacterStats();
            c.animations = "Characters/SpidermanAnimation";
            c.thumbnail = "Characters/WolverineThumb";
            c.image = "Characters/WolverinePose";
            c.sound_selected = "Sound/Spiderman/selected";
            c.sound_jump = "Sound/Spiderman/jump";
            c.sound_kill = "Sound/Wolwerine/kill";
            c.sound_punch = "Sound/Spiderman/punch";

            c.ani_noneStart = 0;
            c.ani_noneEnd = 0;
            c.ani_runStart = 13;
            c.ani_jumpStart = 14;
            c.ani_jumpEnd = 19;
            c.ani_fallStart = 22;
            c.ani_landStart = 22;
            c.ani_landEnd = 25;
            c.ani_takeHitStart = 26;
            c.ani_takeHitEnd = 27;
            c.ani_brake = 26;

            c.size = new Vector2(40, 120);

            c.a = new MoveStats(10, 500, 150, 300, 28, 34, 0, new Vector2(10, 0), new Vector2(20, 15), new Vector2(50, -5), new Vector2(10, 10));
            c.aLR = new ChargeMove(30, 500, 100, 250, 45, 52, 0, new Vector2(20, 20), new Vector2(40, 40), new Vector2(45, -20), new Vector2(10, 10), 50, 250, 40, 42, 42, 45);
            c.x = new BodyMove(10, 500, 275, 360, 53, 71, 0, new Vector2(-3, 20), new Vector2(0, -60), new Vector2(0, -60), new Vector2(10, 10), new Vector2(0, -5.0f), 100, 300);
            c.xLR = new BodyMove(20, 1000, 500, 850, 72, 86, 0, new Vector2(20, -1), new Vector2(50, 0), new Vector2(50, 0), new Vector2(10, 30), new Vector2(10.0f, -1.0f), 220, 700);
            return c;
        }
    }
}
