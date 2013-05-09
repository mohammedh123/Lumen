using Microsoft.Xna.Framework;

namespace Lumen
{
    public class GameVariables
    {
        public const int PlayerStartingHealth = 3; //CHANGIN THIS REQUIRES A CHANGE OF ART+CODE
        public const float MaxScreenShake = 0.3f;
        public static float PlayerSpeed = 300; //pixels per second
        public static float PlayerCollisionRadius = 16.0f;
        public static float PlayerOrbsDistance = 15.0f;
        public static float PlayerOrbsPeriod = 0.6f;
        public static float PlayerLightDuration = 0.5f;
        public static float PlayerLightRadius = 120.0f;
        public static float PlayerLightFadeInDuration = 0.125f;
        public static float PlayerLightFadeOutDuration = 0.125f;
        public static float PlayerHitVibrationDuration = 0.25f;
        public static float PlayerHitBlinkingDuration = 3.0f;

        public static float EnemySpeed = 300.0f;
        public static float EnemyCollisionRadius = 24.0f;
        public static float EnemyAttackCooldown = 1.0f;
        public static float EnemyAttackMaxRadius = 80.0f;
        public static float EnemyAttackRadiusGrowth = 160.0f;
        public static float EnemyAttackRadiusAcceleration = 40.0f;
        public static float EnemyLightRadiusWhileCharging = 20.0f;
        public static float EnemyAttackTotalDuration = 0.067f;
        public static float EnemyEnergyRegeneration = 2000.0f;
        public static float EnemySpeedWhileCharging = 250.0f;

        public static bool IsScreenWrapping = false;

        public static float CameraZoom = 1.0f;

        public static float BlinkingPeriod = 0.5f;
        public static float BlinkingDuration = 0.625f;
        public static float BlinkingRadius = 24.0f;
        public static float EnemyBlinkingRadius = 4.0f;
        public static float BlinkingFadeInDuration = 0.25f;
        public static float BlinkingFadeOutDuration = 0.1f;
        public static float BlinkingMaxFrequency = 16.0f;

        public static int CrystalInitialCount = 3;
        public static float CrystalCollectionRadius = 24.0f;
        public static float CrystalCollectionTime = 3.0f;
        public static int CrystalHarvestRequirement = 1;
        public static float CrystalGlowRadius = 120.0f;
        public static float CrystalFadeInDuration = 0.3f;
        public static float CrystalFadeOutDuration = 0.3f;

        public static float CrystalMinimumSpawnDistanceBetween = 180.0f;
        public static int CrystalSpawningMaxAttempts = 100;

        public static int RoundOneGoal = 1;
        public static int FinalCrystalBuffer = 2;

        public static float RoundOverFadeOutDuration = 0.5f;

        public static float ScreenShakeAmount = 0.0f;

        public static float UIScale = 0.25f;

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