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
        
        public static float EnemySpeed = 80.0f;
        
        public static float EnemyCollisionRadius = 12.0f;
        
        public static bool IsScreenWrapping = false;

        public static float CameraZoom = 1.0f;

        public static float BlinkingPeriod = 1.0f;
        public static float BlinkingDuration = 0.1f;
        public static float BlinkingRadius = 50.0f;

        public static int CrystalInitialCount = 8;
        public static float CrystalCollectionRadius = 24.0f;
        public static float CrystalCollectionTime = 3.0f;
        public static int CrystalHarvestRequirement = 3;

        public static Matrix CameraZoomMatrix
        {
            get { return Matrix.CreateScale(CameraZoom); }
        }
    }
}