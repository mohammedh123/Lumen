﻿using Microsoft.Xna.Framework;

namespace Lumen
{
    public class GameVariables
    {
        public const int PlayerStartingHealth = 3; //CHANGIN THIS REQUIRES A CHANGE OF ART+CODE
        public const float MaxScreenShake = 0.3f;
        public const float PlayerSpeed = 300; //pixels per second
        public const float PlayerCollisionRadius = 16.0f;
        public const float PlayerOrbsDistance = 15.0f;
        public const float PlayerOrbsPeriod = 0.6f;
        public const float PlayerLightDuration = 0.5f;
        public const float PlayerLightRadius = 120.0f;
        public const float PlayerLightFadeInDuration = 0.125f;
        public const float PlayerLightFadeOutDuration = 0.5f;
        public const float PlayerHitVibrationDuration = 0.25f;
        public const float PlayerHitBlinkingDuration = 3.0f;

        public const float PlayerComebackBonusSpeed = 10.0f;
        public const float EnemyComebackBonusSpeed = 20.0f;

        public static float OnePlayerCollectionRate = 1.25f; //meaning when there is  1 player  left alive, he finishes collecting in 1.0 seconds
        public static float TwoPlayersCollectionRate = 2.0f; //meaning when there are 2 players left alive, he finishes collecting in 2.0 seconds
        public static float ThreePlayersCollectionRate = 3.0f; //meaning when there are 3 players left alive, he finishes collecting in 3.0 seconds

        public static float GetCollectionRateScale(int roundNum)
        {
            switch (roundNum)
            {
                case 5:
                    return 0.85f;
                case 6:
                    return 0.95f;
                case 7:
                    return 1.00f;
                case 8:
                    return 1.05f;
                case 9:
                    return 1.15f;
                default:
                    return 1.0f;
            }
        }

        public const float EnemySpeed = 330.0f;
        public const float EnemyCollisionRadius = 24.0f;
        public const float EnemyAttackCooldown = 0.5f;
        public const float EnemyAttackMaxRadius = 100.0f;
        public const float EnemyAttackRadiusGrowth = 160.0f;
        public const float EnemyAttackRadiusAcceleration = 60.0f;
        public const float EnemyLightRadiusWhileCharging = 20.0f;
        public const float EnemyAttackTotalDuration = 0.067f;
        public const float EnemyEnergyRegeneration = 2000.0f;
        public const float EnemySpeedWhileCharging = 275.0f;

        public const bool IsScreenWrapping = false;

        public static float CameraZoom = 1.0f;

        public const float BlinkingPeriod = 0.5f;
        public const float BlinkingDuration = 0.625f;
        public const float BlinkingRadius = 24.0f;
        public const float EnemyBlinkingRadius = 4.0f;
        public const float BlinkingFadeInDuration = 0.25f;
        public const float BlinkingFadeOutDuration = 0.1f;
        public const float BlinkingMaxFrequency = 16.0f;

        public const float CrystalCollectionRadius = 24.0f;
        public const float CrystalCollectionTime = 3.0f;
        public const int CrystalHarvestRequirement = 1;
        public const float CrystalGlowRadius = 120.0f;
        public const float CrystalFadeInDuration = 0.3f;
        public const float CrystalFadeOutDuration = 0.3f;


        public const float CrystalMinimumSpawnDistanceBetween = 180.0f;
        public const int CrystalSpawningMaxAttempts = 100;

        public const int FinalCrystalBuffer = 2;

        public const float RoundOverFadeOutDuration = 0.5f;

        public static float ScreenShakeAmount = 0.0f;

        public const float UIScale = 0.25f;

        private const int RoundOneGoal = 1;

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
    }
}