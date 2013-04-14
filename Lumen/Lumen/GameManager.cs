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
        public Enemy Enemy { get; set; }
        public List<Prop> Props { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Prop> PropsToBeAdded { get; set; }

        public GameState State = GameState.StillGoing;

        private readonly Vector2 _gameResolution;

        public GameManager(Vector2 gameResolution)
        {
            Players = new List<Player>();
            Props = new List<Prop>();
            Blocks = new List<Block>();
            PropsToBeAdded = new List<Prop>();

            _gameResolution = gameResolution;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var song = SoundManager.GetSong("main_bgm");

            if (MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(song);

            //iterate through all entities and update them (their deltas will be set at the end of update)
            foreach (var player in Players) {
                player.Update(dt);

                SetPlayerCrystalVibration(player);

                HandleCrystalCollection(player, dt);
            }

            Enemy.Update(dt);

            foreach (var prop in Props)
                prop.Update(dt);

            HandlePropsToBeAdded(dt);

            var isEnemyMoving = false;

            foreach (var player in Players) {
                ResolveCollisionAgainstPlayers(player, GameVariables.PlayerCollisionRadius);
                ResolveOutOfBoundsCollision(player);

                ResolveCollisionAgainstEnemy(player, GameVariables.PlayerCollisionRadius);

                player.ResetVelocity();

                if (player.Health <= 0) {
                    KillPlayer(player);
                }
            }

            if (isEnemyMoving)
                SoundManager.GetSoundInstance("footstep").Play();
            
            HandlePropCollision();
        }

        private void ResolveOutOfBoundsCollision(Player player)
        {
            var screenBounds = new Rectangle(0, 0, (int) _gameResolution.X, (int) _gameResolution.Y);

            if (screenBounds.Contains((int) (player.Position.X + player.Velocity.X),
                                      (int) (player.Position.Y))) {
                player.ApplyVelocity(true, false);
            }

            if (screenBounds.Contains((int) (player.Position.X),
                                      (int) (player.Position.Y + player.Velocity.Y))) {
                player.ApplyVelocity(false, true);
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
                new Rectangle((int)(Enemy.Position.X - GameVariables.PlayerCollisionRadius),
                                (int)(Enemy.Position.Y - GameVariables.PlayerCollisionRadius),
                                (int)(GameVariables.PlayerCollisionRadius * 2),
                                (int)(GameVariables.PlayerCollisionRadius * 2));

            var collided = entRect.Intersects(enemyRect);

            if(collided) {
                KillPlayer(player);
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

                    if (colTar == null) {
                        player.ResetCollecting();
                    }
                    else {
                        player.CollectionTarget = colTar;
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

                        player.CrystalCount++;
                    }
                }
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

            var vibRat = closestDist/(100.0f*100.0f);

            if (GamePad.GetState(player.PlayerNum).IsConnected) {
                GamePad.SetVibration(player.PlayerNum, Math.Max(0, 0.5f*(1 - vibRat)), Math.Max(0, 0.5f*(1 - vibRat)));
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

            Enemy.Draw(sb);

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
                player.PlayerNum = playerIndex;
        }

        public void AddEnemy(Enemy e, PlayerIndex playerIndex)
        {
            if (AddEnemy(e))
                e.PlayerNum = playerIndex;
        }

        public bool AddPlayer(Player p)
        {
            if (Players.Contains(p)) return false;

            Props.Add(new BlinkingLight("candle", p));
            Players.Add(p);

            return true;
        }

        public bool AddEnemy(Enemy e)
        {
            if (Enemy != null) return false;

            Props.Add(new BlinkingLight("candle", e));
            Enemy = e;

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
        
        private void RemovePlayersBlinkingLight(Player player)
        {
            var idx = Props.FindLastIndex(p => p is BlinkingLight && ((BlinkingLight) p).Owner == player);

            if (idx >= 0) {
                Props.RemoveAt(idx);
            }
        }

        public void KillPlayer(Player player)
        {
            if (Players.Count == 1)
                State = GameState.EnemyWins;

            GamePad.SetVibration(player.PlayerNum, 0, 0);

            //find the lantern that belongs to this enemy and eliminate it
            RemovePlayersBlinkingLight(player);

            Players.Remove(player);

            SoundManager.GetSound("death_sound").Play();
        }

        public void KillEnemy(Enemy enemy)
        {
            State = GameState.PlayersWin;

            GamePad.SetVibration(enemy.PlayerNum, 0, 0);
            Enemy = null;

            SoundManager.GetSound("death_sound").Play();
        }

        public void Reset()
        {
            Players.Clear();
            Props.Clear();
            Blocks.Clear();
            PropsToBeAdded.Clear();

            Enemy = null;
        }
    }
}
