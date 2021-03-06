﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SmashBros.MySystem
{
    public static class Constants
    {
        public const bool DebugMode = !true;
        public const bool StartGameplay = !true;
        public const bool Music = true;
        public const bool ZoomMin = !true;
        public const int WindowWidth = 1280;
        public const int WindowHeight = 720;
        public const bool FullScreen = true;

        public const int FPS = 14;
        public const float MaxZoom = 1.9f;
        public const float MinZoom = 0.5f;
        public const int ThumbWith = 210;
        public const int ThumbHeight = 210;
        public const float MaxCursorSpeed = 10f;
        public const float GamePlayGravity = 20f;

        //GamePlay
        public const int CountDownTime = 4;

        //PowerUpStuff
        public const int PowerUpBeforeDisapear = 10;
        public const int PowerUpMinTimeBeforeNew = 20;
        public const int PowerUpMaxTimeBeforeNew = 35;

    }
}
