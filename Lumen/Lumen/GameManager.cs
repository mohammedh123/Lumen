using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Props;
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
        public List<Player> Players { get; set; }
        private Dictionary<Player,BlinkingLight> _deadPlayers = new Dictionary<Player, BlinkingLight>(); 
        public Guardian Guardian { get; set; }
        public List<Prop> Props { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Prop> PropsToBeAdded { get; set; }

        public int CrystalsCollected { get; private set; }

        public int CrystalsRemaining
        {
            get { return GameVariables.Crystals(RoundNumber, CrystalsCollected) - GameVariables.FinalCrystalBuffer; }
        }

        public GameState State = GameState.StillGoing;

        private readonly Vector2 _gameResolution;

        public int RoundNumber { get; private set; }

        public float TimeTillNextRound { get; private set; }

        public GameManager(Vector2 gameResolution)
        {
            Players = new List<Player>();
            Props = new List<Prop>();
            Blocks = new List<Block>();
            PropsToBeAdded = new List<Prop>();
            RoundNumber = 1;

            _gameResolution = gameResolution;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var song = SoundManager.GetSong("main_bgm");

            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(song);

            if (State == GameState.StillGoing) {
                //iterate through all entities and update them (their deltas will be set at the end of update)
                foreach (var player in Players) {
                    player.Update(dt);

                    SetPlayerCrystalVibration(player);

                    HandleCrystalCollection(player, dt);
                }

                Guardian.Update(dt);

                foreach (var prop in Props)
                    prop.Update(dt);

                HandlePropsToBeRemoved();
                HandlePropsToBeAdded(dt);

                var isEnemyMoving = false;

                ResolveOutOfBoundsCollision(Guardian);
                Guardian.ResetVelocity();

                for (int i = 0; i < Players.Count; i++) {
                    var player = Players[i];

                    ResolveCollisionAgainstPlayers(player, GameVariables.PlayerCollisionRadius);
                    ResolveOutOfBoundsCollision(player);

                    ResolveCollisionAgainstEnemy(player, GameVariables.PlayerCollisionRadius);

                    player.ResetVelocity();

                    if (player.Health <= 0) {
                        KillPlayer(player);
                        i--;
                    }
                }

                if (isEnemyMoving)
                    SoundManager.GetSoundInstance("footstep").Play();
                if (Guardian != null) {
                    if (Guardian.IsChargingUp)
                        GameVariables.ScreenShakeAmount = GameVariables.MaxScreenShake*
                                                          (Guardian.FinalRadiusOfAttack/
                                                           GameVariables.EnemyAttackMaxRadius);
                    else if (Guardian.IsAttacking)
                        GameVariables.ScreenShakeAmount = GameVariables.MaxScreenShake*
                                                          (Guardian.FinalRadiusOfAttack/
                                                           GameVariables.EnemyAttackMaxRadius);
                    else
                        GameVariables.ScreenShakeAmount = 0;
                }

                HandlePropCollision();
            }
            else if(State == GameState.PlayersWin) {
                TimeTillNextRound -= dt;

                if(TimeTillNextRound <= 0.0f) {
                    TimeTillNextRound = 0.0f;
                    StartNextRound();
                }
            }
            else if(State == GameState.EnemyWins) {
                HandleRestartInput();
            }
        }

        private void HandleRestartInput()
        {
            if(InputManager.KeyDown(Keys.R)) {
                RoundNumber--;
                StartNextRound();
            }
            else if(InputManager.KeyDown(Keys.T)) {
                //restart
                RoundNumber = 0;
                StartNextRound();
            }
        }

        private void StartNextRound()
        {
            RoundNumber++;
            State = GameState.StillGoing;

            foreach(var kvp in _deadPlayers) {
                Players.Add(kvp.Key);
                Props.Add(kvp.Value);
            }

            _deadPlayers.Clear();

            foreach(var player in Players) {
                player.CrystalCount = 0;
                CrystalsCollected = 0;
                player.Position = new Vector2(64, _gameResolution.Y/2 - 96 + 32*player.PlayerSpriteIndex);
            }

            Props.RemoveAll(p => p is Crystal);

            for (var i = 0; i < GameVariables.CrystalsToSpawn(RoundNumber); i++)
            {
                AddCrystal(new Vector2(GameDriver.RandomGen.Next(16, (int)_gameResolution.X), GameDriver.RandomGen.Next(16, (int)_gameResolution.Y)));
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

        private void ResolveCollisionAgainstPlayers(Entity ent, float entCollisionRadius)
        {
            for (int j = 0; j < Players.Count; j++)
            {
                var otherPlayer = Players[j];

                if (otherPlayer == ent)
                {
                    continue;
                }

                var entNewX = ent.Position.X + ent.Velocity.X;

                var entNewRect = new Rectangle((int)(entNewX - entCollisionRadius),
                                                  (int)(ent.Position.Y - entCollisionRadius),
                                                  (int)(entCollisionRadius * 2),
                                                  (int)(entCollisionRadius * 2));

                var otherOldRect =
                    new Rectangle((int)(otherPlayer.Position.X - GameVariables.PlayerCollisionRadius),
                                  (int)(otherPlayer.Position.Y - GameVariables.PlayerCollisionRadius),
                                  (int)(GameVariables.PlayerCollisionRadius * 2),
                                  (int)(GameVariables.PlayerCollisionRadius * 2));


                if (entNewRect.Intersects(otherOldRect))
                {
                    if (ent.Velocity.X > 0)
                    {
                        ent.Velocity =
                            new Vector2(
                                (otherPlayer.Position.X - ent.Position.X - 2 * entCollisionRadius),
                                ent.Velocity.Y);
                        otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                           otherPlayer.Velocity.Y);
                    }
                    else if (ent.Velocity.X < 0)
                    {
                        ent.Velocity =
                            new Vector2(
                                (otherPlayer.Position.X - ent.Position.X + 2 * GameVariables.PlayerCollisionRadius),
                                ent.Velocity.Y);
                        otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                           otherPlayer.Velocity.Y);
                    }
                }

                var playerNewY = ent.Position.Y + ent.Velocity.Y;

                entNewRect = new Rectangle((int)(ent.Position.X - entCollisionRadius),
                                              (int)(playerNewY - entCollisionRadius),
                                              (int)(entCollisionRadius * 2),
                                              (int)(entCollisionRadius * 2));

                if (entNewRect.Intersects(otherOldRect))
                {
                    if (ent.Velocity.Y > 0)
                    {
                        ent.Velocity = new Vector2(ent.Velocity.X,
                                                      (otherPlayer.Position.Y - ent.Position.Y -
                                                       2 * entCollisionRadius));
                        otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                           otherPlayer.Velocity.Y);
                    }
                    else if (ent.Velocity.Y < 0)
                    {
                        ent.Velocity = new Vector2(ent.Velocity.X,
                                                      (otherPlayer.Position.Y - ent.Position.Y +
                                                       2 * GameVariables.PlayerCollisionRadius));
                        otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                           otherPlayer.Velocity.Y);
                    }
                }
            }
        }

        private void ResolveCollisionAgainstEnemy(Player player, float entCollisionRadius)
        {
            var entRect = new Rectangle((int)(player.Position.X - entCollisionRadius),
                                                (int)(player.Position.Y - entCollisionRadius),
                                                (int)(entCollisionRadius * 2),
                                                (int)(entCollisionRadius * 2));

            var enemyRect =
                new Rectangle((int)(Guardian.Position.X - GameVariables.PlayerCollisionRadius),
                                (int)(Guardian.Position.Y - GameVariables.PlayerCollisionRadius),
                                (int)(GameVariables.PlayerCollisionRadius * 2),
                                (int)(GameVariables.PlayerCollisionRadius * 2));

            var collided = entRect.Intersects(enemyRect);

            if(collided) {
                KillPlayer(player);
            }

            //now check player against the attack
            if(Guardian.IsAttacking) {
                if(Collider.CirclesCollide(Guardian.Position, Guardian.LightRadius, player.Position, GameVariables.PlayerCollisionRadius)) {
                    if (!Guardian.PlayersHitThisAttack.Contains(player)) {
                        Guardian.PlayersHitThisAttack.Add(player);

                        if(Guardian.IsFullyCharged) {
                            player.Health -= 2;
                        }
                        else {
                            player.Health -= 1;
                        }
                    }
                }
            }
        }

        private void HandlePropsToBeAdded(float dt)
        {
            for (int i = 0; i < PropsToBeAdded.Count; i++) {
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
            if (player.IsCollecting) {
                if (player.CollectionTarget == null) {
                    Crystal colTar = null;

                    foreach (var prop in Props) {
                        if (prop.PropType == PropTypeEnum.Crystal) {
                            if (Collider.IsPlayerWithinRadius(player, prop.Position,
                                                              GameVariables.CrystalCollectionRadius)) {
                                colTar = (Crystal) prop;
                                break;
                            }
                        }
                    }

                    if (colTar == null || colTar.IsToBeRemoved) {
                        player.ResetCollecting();
                    }
                    else {
                        player.CollectionTarget = colTar;
                        player.CollectionTarget.IncrementCollectorCount();
                    }
                }
                else {
                    player.CollectingTime += dt;

                    if (player.CollectingTime >= GameVariables.CrystalCollectionTime) {
                        player.CollectionTarget.Health--;

                        if (player.CollectionTarget.Health <= 0) {
                            player.CollectionTarget.IsToBeRemoved = true;
                        }

                        player.ResetCollecting();

                        IncreasePlayerCrystalCount(player);
                    }
                }
            }
        }

        private void IncreasePlayerCrystalCount(Player player)
        {
            CrystalsCollected++;
            player.CrystalCount++;
            SoundManager.GetSound("crystal_get").Play();

            RoundOver();
        }

        private void RoundOver()
        {
            if (CrystalsRemaining == 0) {
                State = GameState.PlayersWin;
                TimeTillNextRound = 5.0f;

                foreach (var player in Players)
                    GamePad.SetVibration(player.ControllerIndex, 0, 0);
                GamePad.SetVibration(Guardian.PlayerNum, 0, 0);
            }
        }

        private void SetPlayerCrystalVibration(Player player)
        {
            float closestDist = 2000*2000;

            foreach (var prop in Props) {
                if (prop.PropType == PropTypeEnum.Crystal) {
                    var dist = (prop.Position - player.Position).LengthSquared();

                    if (dist < closestDist) {
                        closestDist = dist;
                    }
                }
            }

            var vibRat = closestDist / (GameVariables.PlayerVibrationDetectionRadius * GameVariables.PlayerVibrationDetectionRadius);

            if (GamePad.GetState(player.ControllerIndex).IsConnected) {
                GamePad.SetVibration(player.ControllerIndex, Math.Max(0, 0.5f*(1 - vibRat)), Math.Max(0, 0.5f*(1 - vibRat)));
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

            foreach (var prop in Props.OrderByDescending(p => p.PropType))
                prop.Draw(sb);

            foreach (var player in Players)
                player.Draw(sb);

            Guardian.Draw(sb);

            foreach (var block in Blocks)
                block.Draw(sb);

            sb.End();
        }

        public void DrawBackground(SpriteBatch sb)
        {
            //sb MUST have been Begin'd

            sb.Draw(TextureManager.GetTexture("background"),
                    new Rectangle(0, 0, (int) _gameResolution.X, (int) _gameResolution.Y), Color.White);
        }

        public void AddPlayer(Player player, PlayerIndex playerIndex)
        {
            if(AddPlayer(player))
                player.ControllerIndex = playerIndex;
        }

        public void AddEnemy(Guardian e, PlayerIndex playerIndex)
        {
            if (AddEnemy(e))
                e.PlayerNum = playerIndex;
        }

        public bool AddPlayer(Player p)
        {
            if (Players.Contains(p)) return false;

            Props.Add(new BlinkingLight("blank", p, GameVariables.BlinkingRadius));
            Players.Add(p);

            return true;
        }

        public bool AddEnemy(Guardian e)
        {
            if (Guardian != null) return false;

            Props.Add(new BlinkingLight("blank", e, GameVariables.EnemyBlinkingRadius));
            Guardian = e;

            return true;
        }

        public void AddBlock(Vector2 topLeftCorner, int size)
        {
            Blocks.Add(new Block(topLeftCorner, size));
        }

        public void AddCrystal(Vector2 position)
        {
            Props.Add(new Crystal(position));
        }
        
        private BlinkingLight RemovePlayersBlinkingLight(Player player)
        {
            var idx = Props.FindLastIndex(p => p is BlinkingLight && ((BlinkingLight) p).Owner == player);
            var prop = Props[idx];

            if (idx >= 0) {
                Props.RemoveAt(idx);
            }

            return prop as BlinkingLight;
        }

        public void KillPlayer(Player player)
        {
            if (Players.Count == 1)
                State = GameState.EnemyWins;

            GamePad.SetVibration(player.ControllerIndex, 0, 0);

            //find the lantern that belongs to this Guardian and eliminate it
            var light = RemovePlayersBlinkingLight(player);

            Players.Remove(player);
            _deadPlayers.Add(player, light);

            SoundManager.GetSound("death_sound").Play();
        }

        public void KillEnemy(Guardian guardian)
        {
            State = GameState.PlayersWin;

            GamePad.SetVibration(guardian.PlayerNum, 0, 0);
            Guardian = null;

            SoundManager.GetSound("death_sound").Play();
        }
        
        public void ResetCompletely()
        {
            State = GameState.StillGoing;

            Players.Clear();
            _deadPlayers.Clear();
            Props.Clear();
            Blocks.Clear();
            PropsToBeAdded.Clear();

            Guardian = null;
        }

        public IEnumerable<ILightProvider> GetLights()
        {
            var lights = new List<ILightProvider>();

            foreach(var prop in Props) {
                var asLight = prop as ILightProvider;

                if(asLight != null) {
                    lights.Add(asLight);
                }
            }

            if(Guardian != null)
                lights.Add(Guardian);

            return lights;
        }
    }
}
