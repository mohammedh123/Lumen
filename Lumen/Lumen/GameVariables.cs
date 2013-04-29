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
        public static float PlayerCollisionRadius = 10.0f;
        public const int PlayerStartingHealth = 3; //CHANGIN THIS REQUIRES A CHANGE OF ART+CODE
        public static float PlayerOrbsDistance = 20.0f;
        public static float PlayerOrbsPeriod = 0.5f;
        public static float PlayerLightDuration = 1.0f;
        public static float PlayerLightRadius = 150.0f;
        
        public static float EnemySpeed = 80.0f;
        public static float EnemyCollisionRadius = 24.0f;
        public static float EnemyAttackCooldown = 3.0f;
        public static float EnemyAttackMaxRadius = 512.0f;
        public static float EnemyAttackRadiusGrowth = 200.0f;
        public static float EnemyAttackRadiusAcceleration = 100.0f;
        public static float EnemyLightRadiusWhileCharging = 64.0f;
        public static float EnemyAttackTotalDuration = 1.00f;
        public static float EnemyEnergyRegeneration = 20.0f;
        public static float EnemySpeedWhileCharging = 30.0f;

        public static float PlayerVibrationDetectionRadius = 100.0f;
        public static bool IsScreenWrapping = false;

        public static float CameraZoom = 1.0f;

        public static float BlinkingPeriod = 1.0f;
        public static float BlinkingDuration = 0.1f;
        public static float BlinkingRadius = 50.0f;
        public static float EnemyBlinkingRadius = 50.0f;
        public static float BlinkingFadeInDuration = 0.1f;
        public static float BlinkingFadeOutDuration = 0.1f;
        public static float BlinkingMaxFrequency = 16.0f;

        public static int CrystalInitialCount = 3;
        public static float CrystalCollectionRadius = 24.0f;
        public static float CrystalCollectionTime = 3.0f;
        public static int CrystalHarvestRequirement = 3;
        public static float CrystalGlowRadius = 120.0f;
        public static float CrystalMaxVibration = 0.8f;

        public static Matrix CameraZoomMatrix
        {
            get { return Matrix.CreateScale(CameraZoom); }
        }
        
        public static int CrystalsToSpawn(int roundNum)
        {
            return (CrystalRoundGoal(roundNum) + FinalCrystalBuffer);
        }

        public static int CrystalRoundGoal(int roundNum)
        {
            return (roundNum - 1) + RoundOneGoal;
        }

        public static int Crystals(int roundNum, int collectedCrystalCount)
        {
            return CrystalsToSpawn(roundNum) - collectedCrystalCount;
        }

        public static int RoundOneGoal = 3;
        public static int FinalCrystalBuffer = 2;

        public static float ScreenShakeAmount = 0.0f;
        public const float MaxScreenShake = 0.0f;

        public static float UIScale = 0.25f;
    }
}