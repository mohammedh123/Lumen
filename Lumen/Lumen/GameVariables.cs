﻿using System;
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
        public static int   PlayerInitialCandles = 1;
        public static float PlayerPushSpeed = 50;
        public static float PlayerLanternRadius = 256.0f;

        public static float PlayerSwordAttackCooldown = 5.0f;
        public static float PlayerSwordAttackDuration = 0.5f;
        public static float PlayerTorchAttackCooldown = 5.0f;
        public static float PlayerTorchAttackDuration = 1.0f;
        public static float PlayerTorchAttackLightRadius = 100.0f;

        public static float EnemySpeed = 80.0f;
        public static float EnemyDashSpeed = 120.0f;
        public static float EnemyDashCooldown = 5.0f;
        public static float EnemyDashDuration = 2.0f;
        public static float EnemyKillRadius = 30.0f;
        public static float EnemyKillTimeRequirement = 2.0f;

        public static float ImmolatedLightRadius = 90.0f;
        public static float ImmolatedLightDuration = 2.0f;

        public static float EnemyCollisionRadius = 12.0f;

        public static float CandleInitialRadius = 200.0f;
        public static float CandleFlickerPeriod = 0.2f;
        public static float CandleFlickerAmount = 5.0f;
        public static float CandleMinFlicker = 25.0f;
        public static float CandleMaxFlicker = 40.0f;
        public static float CandleLifetime = 999.0f;

        public static bool CoinCanRespawn = false;
        public static float CoinRespawnRateMin = 6.0f;
        public static float CoinRespawnRateMax = 6.0f;
        public static int CoinInitialCount = 8;

        public static bool IsScreenWrapping = false;

        public static float CameraZoom = 1.0f;

        public static float BlinkingPeriod = 1.0f;
        public static float BlinkingDuration = 0.1f;
        public static float BlinkingRadius = 50.0f;

        public static float CrystalCollectionRadius = 24.0f;
        public static float CrystalCollectionTime = 3.0f;
        public static int CrystalHarvestRequirement = 3;

        public static Matrix CameraZoomMatrix
        {
            get { return Matrix.CreateScale(CameraZoom); }
        }
    }
}