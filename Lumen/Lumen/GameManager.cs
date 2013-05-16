using System;
using System.Collections.Generic;
using System.Linq;
using Lumen.Entities;
using Lumen.Light_System;
using Lumen.Particle_System;
using Lumen.Props;
using Lumen.State_Management;
using Lumen.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lumen
{
    internal enum GameState
    {
        StillGoing,
        PlayersWin,
        EnemyWins
    }

    internal class GameManager
    {
        public readonly Dictionary<Player, List<Light>> DeadPlayers = new Dictionary<Player, List<Light>>();
        private readonly Vector2 _gameResolution;
        public GameState State = GameState.StillGoing;
        private GameState _lastPlayerState;
        private List<PlayerIndex> _playerOrder;
        private float _playerBonusSpeed, _guardianBonusSpeed;

        public GameManager(Vector2 gameResolution, IEnumerable<PlayerIndex> playerOrder)
        {
            Players = new List<Player>();
            Props = new List<Prop>();
            PropsToBeAdded = new List<Prop>();
            RoundNumber = 7;
            _playerOrder = playerOrder.ToList();
            FadeoutTimer = -1.0f;
            _gameResolution = gameResolution;
        }

        public List<Player> Players { get; private set; }

        public Guardian Guardian { get; private set; }
        public List<Prop> Props { get; private set; }
        private List<Prop> PropsToBeAdded { get; set; }

        private int CrystalsCollected { get; set; }

        public int CrystalsRemaining
        {
            get { return GameVariables.Crystals(RoundNumber, CrystalsCollected) - GameVariables.FinalCrystalBuffer; }
        }

        public int RoundNumber { get; private set; }
        public float FadeoutTimer { get; private set; }

        public bool IsFadingOut
        {
            get { return FadeoutTimer >= 0.0f; }
        }

        private bool IsWinnerDecided
        {
            get { return RoundNumber < 5 || RoundNumber > 9; }
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (State == GameState.StillGoing) {
                //iterate through all entities and update them (their deltas will be set at the end of update)
                foreach (var player in Players) {
                    player.Update(dt);

                    HandleCrystalCollection(player, dt);
                }

                foreach (var deadPlayer in DeadPlayers) {
                    deadPlayer.Key.Update(dt);
                }

                Guardian.Update(dt);

                HandlePropsToBeRemoved();
                HandlePropsToBeAdded(dt);

                //SetPlayerCrystalVibration(Guardian);
                ResolveOutOfBoundsCollision(Guardian);
                Guardian.OrbitRing.Update(dt);

                foreach (var prop in Props) {
                    prop.Update(dt);

                    var crystal = prop as Crystal;

                    if (crystal != null) {
                        if (!crystal.IsSomeoneCollectingThis) {
                            crystal.ResetCollectionTimeLeft(Players.Count);
                        }

                        if (crystal.Collector != null) {
                            IncreasePlayerCrystalCount(crystal.Collector);
                            crystal.Collector = null;
                        }
                    }
                }

                Guardian.ResetVelocity();

                for (var i = 0; i < Players.Count; i++) {
                    var player = Players[i];

                    ResolveOutOfBoundsCollision(player);
                    player.OrbitRing.Update(dt);
                    ResolveCollisionAgainstEnemy(player);

                    player.ResetVelocity();

                    if (player.Health <= 0) {
                        KillPlayer(player);
                        i--;
                    }
                }

                if (Guardian != null) {
                    if (Guardian.IsChargingUp) {
                        GameVariables.ScreenShakeAmount = GameVariables.MaxScreenShake*
                                                          (Guardian.ChargingAttackRadius/
                                                           GameVariables.EnemyAttackMaxRadius);
                        GamePad.SetVibration(Guardian.ControllerIndex,
                                             (Guardian.ChargingAttackRadius/GameVariables.EnemyAttackMaxRadius),
                                             (Guardian.InternalAttackRadius/GameVariables.EnemyAttackMaxRadius));
                    }
                    else if (Guardian.IsAttacking) {
                        GameVariables.ScreenShakeAmount = GameVariables.MaxScreenShake*
                                                          (Guardian.ChargingAttackRadius/
                                                           GameVariables.EnemyAttackMaxRadius);
                        GamePad.SetVibration(Guardian.ControllerIndex, 0, 0);
                    }
                    else {
                        GameVariables.ScreenShakeAmount = 0;
                        GamePad.SetVibration(Guardian.ControllerIndex, 0, 0);
                    }
                }

                HandlePropCollision();

                ParticleSystemManager.Instance.Update(dt, _gameResolution);
                LightSpawner.Instance.Update(dt);
            }
            else if (State == GameState.PlayersWin) {
                FadeoutTimer -= dt;

                ResetVibration();

                if (!IsFadingOut) {
                    ResetFadeoutTimer();
                    RoundNumber++;
                    StartNextRound();
                }
            }
            else if (State == GameState.EnemyWins) {
                FadeoutTimer -= dt;

                ResetVibration();

                if (!IsFadingOut) {
                    ResetFadeoutTimer();
                    RoundNumber--;
                    StartNextRound();
                }
            }
        }

        private void ResetFadeoutTimer()
        {
            FadeoutTimer = -1.0f;
        }

        private void ResetVibration()
        {
            Players.ForEach(p => GamePad.SetVibration(p.ControllerIndex, 0, 0));
            foreach (var p in DeadPlayers.Keys) {
                GamePad.SetVibration(p.ControllerIndex, 0, 0);
            }
            if (Guardian != null) {
                GamePad.SetVibration(Guardian.ControllerIndex, 0, 0);
            }
        }

        private void StartNextRound()
        {
            if (IsWinnerDecided) {
                MediaPlayer.Stop();
                StateManager.Instance.PushState(new GameOverState(State));
                return;
            }

            if (State == GameState.PlayersWin) {
                if (_lastPlayerState == GameState.EnemyWins && RoundNumber-1 != 7) {
                    //reset back to 7
                    ResetBackToInitialRound(State);
                }

                _lastPlayerState = GameState.PlayersWin;
            }
            else if (State == GameState.EnemyWins) {
                if (_lastPlayerState == GameState.PlayersWin && RoundNumber+1 != 7) {
                    //reset back to 7
                    ResetBackToInitialRound(State);
                }

                _lastPlayerState = GameState.EnemyWins;
            }

            State = GameState.StillGoing;

            foreach (var kvp in DeadPlayers) {
                Players.Add(kvp.Key);
                Props.AddRange(kvp.Value);
            }

            DeadPlayers.Clear();

            foreach (var player in Players) {
                player.CrystalCount = 0;
                CrystalsCollected = 0;
                player.Health = GameVariables.PlayerStartingHealth;
                player.Position = new Vector2(64, _gameResolution.Y/2 - 96 + 32 + 32*player.PlayerSpriteIndex);
                player.Speed = GameVariables.PlayerSpeed + _playerBonusSpeed;
                player.ResetOrbs();
                player.ResetRecentlyHitTimer();
                player.ResetBlinkingTimer();
                player.AttachedLight.IsVisible = false;
                player.AttachedLight.AbruptlyTurnOff();
                player.AttachedBlinkingLight.IsVisible = false;
                player.AttachedBlinkingLight.ResetFrequency();
            }

            SoundManager.GetSoundInstance("player_light").Stop();

            LightSpawner.Instance.Reset();

            Guardian.Position = new Vector2(_gameResolution.X - 64, _gameResolution.Y/2);
            Guardian.EnergyRemaining = GameVariables.EnemyAttackMaxRadius;
            Guardian.Speed = GameVariables.EnemySpeed + _guardianBonusSpeed;
            Guardian.SpeedWhileCharging = GameVariables.EnemySpeedWhileCharging + _guardianBonusSpeed;
            Guardian.ResetAllAttackData();

            Props.RemoveAll(p => p is Crystal);

            for (var i = 0; i < GameVariables.CrystalsToSpawn(RoundNumber); i++) {
                SpawnCrystalUniformly();
            }

            ParticleSystemManager.Instance.StopFiringAllSystems();
            ParticleSystemManager.Instance.KillAllParticles();

            StateManager.Instance.PushState(new NextRoundState(CrystalsRemaining, _playerOrder));
        }

        private void ResetBackToInitialRound(GameState state)
        {
            if(state == GameState.PlayersWin)
            {
                _playerBonusSpeed += GameVariables.PlayerComebackBonusSpeed;
            }
            else if(state == GameState.EnemyWins)
            {
                _guardianBonusSpeed += GameVariables.EnemyComebackBonusSpeed;
            }

            RoundNumber = 7;
        }

        public void SpawnCrystalUniformly()
        {
            var rectToTry = new Rectangle(150, 0, (int) _gameResolution.X - 16 - 150, (int) _gameResolution.Y - 100);
            var pt = GameDriver.GetPointWithinRect(rectToTry);

            if (!Props.Any(p => p is Crystal)) {
                AddCrystal(pt);
            }
            else {
                var crystalLengths =
                    Props.Where(p => p is Crystal).Select(v => (pt - v.Position).LengthSquared()).ToList();
                var closestCrystalDistance = crystalLengths.Min();

                var bestAttempt = Vector2.Zero;

                if (closestCrystalDistance >=
                    GameVariables.CrystalMinimumSpawnDistanceBetween*GameVariables.CrystalMinimumSpawnDistanceBetween) {
                    AddCrystal(pt);
                }
                else {
                    int i;
                    for (i = 0; i < GameVariables.CrystalSpawningMaxAttempts; i++) {
                        pt = GameDriver.GetPointWithinRect(rectToTry);

                        crystalLengths =
                            Props.Where(p => p is Crystal).Select(v => (pt - v.Position).LengthSquared()).ToList();

                        if (crystalLengths.Min() > closestCrystalDistance) {
                            //if this new point results in a better fit then all the other ones, then we will use this if we exhaust the search
                            closestCrystalDistance = crystalLengths.Min();
                            bestAttempt = pt;
                        }

                        if (closestCrystalDistance >=
                            GameVariables.CrystalMinimumSpawnDistanceBetween*
                            GameVariables.CrystalMinimumSpawnDistanceBetween) {
                            AddCrystal(pt);
                            break;
                        }
                    }

                    if (i == GameVariables.CrystalSpawningMaxAttempts) {
                        AddCrystal(bestAttempt);
                    }
                }
            }
        }

        private void ResolveOutOfBoundsCollision(Entity entity)
        {
            var screenBounds = new Rectangle(0, 0, (int) _gameResolution.X, (int) _gameResolution.Y);


            if (screenBounds.Contains((int) (entity.Position.X + entity.Velocity.X),
                                      (int) (entity.Position.Y))) {
                entity.ApplyVelocity(true, false);
            }

            if (screenBounds.Contains((int) (entity.Position.X),
                                      (int) (entity.Position.Y + entity.Velocity.Y))) {
                entity.ApplyVelocity(false, true);
            }
        }

        private void ResolveCollisionAgainstEnemy(Player player)
        {
            //check player against the attack
            if (Guardian.IsAttacking) {
                if (Collider.CirclesCollide(Guardian.Position, Guardian.LightRadius, player.Position,
                                            GameVariables.PlayerCollisionRadius)) {
                    if (!Guardian.PlayersHitThisAttack.Contains(player)) {
                        Guardian.PlayersHitThisAttack.Add(player);

                        SoundManager.GetSound("player_hit").Play();

                        player.TakeDamage(1);
                    }
                }
            }
        }

        private void HandlePropsToBeAdded(float dt)
        {
            for (var i = 0; i < PropsToBeAdded.Count; i++) {
                var prop = PropsToBeAdded[i];
                prop.Lifetime += dt;

                if (prop.Lifetime > 0) {
                    prop.Lifetime = 0.0f;
                    Props.Add(prop);
                    PropsToBeAdded.RemoveAt(i);
                    i--;
                }
            }
        }

        private void HandlePropsToBeRemoved()
        {
            Props.RemoveAll(p => p.IsToBeRemoved);
        }

        private void HandleCrystalCollection(Player player, float dt)
        {
            if (player.CollectionTarget == null) {
                Crystal colTar = null;

                foreach (var prop in Props) {
                    if (prop.PropType == PropTypeEnum.Crystal) {
                        if (Collider.IsPlayerWithinRadius(player, prop.Position,
                                                          GameVariables.CrystalCollectionRadius)) {
                            colTar = (Crystal) prop;
                            SoundManager.GetSound("crystal_hit").Play();
                            break;
                        }
                    }
                }

                if (colTar == null || colTar.IsToBeRemoved) {
                    player.ResetCollecting();
                }
                else {
                    player.CollectionTarget = colTar;
                    player.CollectionTarget.IncrementCollectorCount(player);
                }
            }
            else {
                //if player is out of range of crystal OR crystal is gone OR crystal is mined out
                if (!Collider.IsPlayerWithinRadius(player, player.CollectionTarget.Position,
                                                   GameVariables.CrystalCollectionRadius) ||
                    player.CollectionTarget.IsToBeRemoved || player.CollectionTarget.Health <= 0) {
                    player.ResetCollecting();
                }
            }
        }

        private void IncreasePlayerCrystalCount(Player player)
        {
            CrystalsCollected++;
            player.IncrementCrystalCount();

            SoundManager.GetSound("crystal_get").Play();

            RoundOver();
        }

        private void RoundOver()
        {
            if (CrystalsRemaining == 0) {
                State = GameState.PlayersWin;
                BeginFadeout();

                foreach (var player in Players) {
                    GamePad.SetVibration(player.ControllerIndex, 0, 0);
                }
                GamePad.SetVibration(Guardian.ControllerIndex, 0, 0);
            }
        }

        private void HandlePropCollision()
        {
            var collidingProps = new List<Prop>();
            var interactingProps = new List<Prop>();

            foreach (var player in Players) {
                collidingProps.Clear();
                interactingProps.Clear();

                foreach (var prop in Props.Where(p => !(p is BlinkingLight))) {
                    if (Collider.Collides(player, prop)) {
                        if (player.IsInteractingWithProp) {
                            if (prop.CanInteract) {
                                interactingProps.Add(prop);
                                prop.OnInteract(player);
                            }
                        }

                        collidingProps.Add(prop);
                        prop.OnCollide(player);
                    }
                }
            }
        }

        public void DrawScene(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null, null, GameVariables.CameraZoomMatrix);

            DrawBackground(sb);

            ParticleSystemManager.Instance.Draw(sb);

            foreach (var prop in Props.OrderByDescending(p => p.PropType)) {
                prop.Draw(sb);
            }

            foreach (var player in Players) {
                player.Draw(sb);
            }

            Guardian.Draw(sb);

            sb.End();
        }

        private void DrawBackground(SpriteBatch sb)
        {
            //sb MUST have been Begin'd

            sb.Draw(TextureManager.GetTexture("background"),
                    new Rectangle(0, 0, (int) _gameResolution.X, (int) _gameResolution.Y), Color.White);
        }

        public void AddPlayer(Player player, PlayerIndex playerIndex)
        {
            if (AddPlayer(player)) {
                player.ControllerIndex = playerIndex;
            }
        }

        public void AddEnemy(Guardian e, PlayerIndex playerIndex)
        {
            if (AddEnemy(e)) {
                e.ControllerIndex = playerIndex;
            }
        }

        private bool AddPlayer(Player p)
        {
            if (Players.Contains(p)) {
                return false;
            }

            var light = new DurationLimitedFadingLight("blank", p, GameVariables.PlayerLightRadius);
            var blinkingLight = new BlinkingLight("blank", p, GameVariables.BlinkingRadius);
            Props.Add(light);
            Props.Add(blinkingLight);
            p.AttachedLight = light;
            p.AttachedBlinkingLight = blinkingLight;
            Players.Add(p);

            return true;
        }

        private bool AddEnemy(Guardian e)
        {
            if (Guardian != null) {
                return false;
            }

            Props.Add(new Light("blank", GameVariables.EnemyBlinkingRadius, e.Position, e));
            Guardian = e;

            return true;
        }

        private void AddCrystal(Vector2 position)
        {
            Props.Add(new Crystal(position));
        }

        private List<Light> RemovePlayersLight(Player player)
        {
            var lights =
                Props.FindAll(p => p is Light && ((Light) p).EntityAttachedTo == player).Cast<Light>().ToList();

            foreach (var prop in lights) {
                Props.Remove(prop);
            }

            return lights;
        }

        private void BeginFadeout()
        {
            FadeoutTimer = GameVariables.RoundOverFadeOutDuration;
        }

        public void KillPlayer(Player player)
        {
            if (Players.Count == 1) {
                State = GameState.EnemyWins;
                BeginFadeout();
            }

            if (player.CollectionTarget != null) {
                player.ResetCollecting();
            }

            GamePad.SetVibration(player.ControllerIndex, 0, 0);

            //find the light that belongs to this Guardian and eliminate it
            var lights = RemovePlayersLight(player);

            Players.Remove(player);
            DeadPlayers.Add(player, lights);

            SoundManager.GetSound("death_sound").Play();
        }

        public void ResetCompletely()
        {
            State = GameState.StillGoing;

            LightSpawner.Instance.Reset();
            Players.Clear();
            DeadPlayers.Clear();
            Props.Clear();
            PropsToBeAdded.Clear();

            Guardian = null;
        }

        public IEnumerable<ILightProvider> GetLights()
        {
            var lights = new List<ILightProvider>(50);

            foreach (var prop in Props) {
                var asLight = prop as ILightProvider;

                if (asLight != null) {
                    lights.Add(asLight);
                }
            }

            if (Guardian != null) {
                lights.Add(Guardian);
                lights.AddRange(Guardian.OrbitRing.GetLights());
            }

            lights.AddRange(LightSpawner.Instance.GetLights());
            return lights;
        }
    }
}