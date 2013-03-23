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
        public static float PlayerCollisionRadius = 16.0f;
        public static int PlayerInitialCandles = 3;
        public static float PlayerLanternRadius = 256.0f; //16*sqrt(2)

        public static float EnemyCollisionRadius = 16.0f;

        public static float CandleInitialRadius = 100.0f;
        public static float CandleFlickerPeriod = 0.2f;
        public static float CandleFlickerAmount = 5.0f;
    }
}
