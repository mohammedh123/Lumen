using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lumen
{
    public class GameVariables
    {
        public static float PlayerSpeed = 100; //pixels per second
        public static float PlayerCollisionRadius = 12.0f;
        public static int   PlayerInitialCandles = 3;
        public static float PlayerPushSpeed = 50;
        public static float PlayerLanternRadius = 256.0f;

        public static float EnemyCollisionRadius = 12.0f;

        public static float CandleInitialRadius = 100.0f;
        public static float CandleFlickerPeriod = 0.2f;
        public static float CandleFlickerAmount = 5.0f;
        public static float CandleLifetime = 5.0f;

        public static bool CoinCanRespawn = false;
        public static float CoinRespawnRateMin = 6.0f;
        public static float CoinRespawnRateMax = 6.0f;
        public static int CoinInitialCount = 3;

        public static float CameraZoom = 1.0f;

        public static Matrix CameraZoomMatrix
        {
            get { return Matrix.CreateScale(CameraZoom); }
        }
    }
}