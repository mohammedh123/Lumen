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
    enum GameState
    {
        StillGoing,
        PlayersWin,
        EnemyWins
    }

    class GameManager
    {
        public List<Player> Players { get; set; }
        public List<Prop>   Props   { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Prop> PropsToBeAdded { get; set; }

        public GameState State = GameState.StillGoing;

        public Player Enemy;

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
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var song = SoundManager.GetSong("main_bgm");

            if(MediaPlayer.State == MediaState.Stopped)
                MediaPlayer.Play(song);

            //iterate through all entities and update them (their deltas will be set at the end of update)
            foreach(var player in Players)
                player.Update(dt);
            
            foreach(var prop in Props)
                prop.Update(dt);

            foreach(var kvp in PropsToBeAdded)
            {
                kvp.Lifetime += dt;
            }

            var kvpsToBeRemoved = PropsToBeAdded.Where(kvp => kvp.Lifetime > 0).ToList();

            foreach(var kvp in kvpsToBeRemoved)
            {
                kvp.Lifetime = 0.0f;
                Props.Add(kvp);
                PropsToBeAdded.Remove(kvp);
            }

            //now that all the deltas are collected, actually move the entities whilst checking for blocking collisions
            //blocking collisions are collisions that will actually stop/impede the entities movement

            //check player vs block collision and 'move' the block if its moveable at half speed
            var isEnemyMoving = false;
            var closestPlayerDistSq = 99999999.0f;

            for (int i = 0; i < Players.Count; i++) {
                var player = Players[i];

                if(player != Enemy) {
                    if(Collider.IsPlayerWithinRadius(player, Enemy.Position, GameVariables.EnemyKillRadius)) {
                        player.TimeWithinEnemyProximity += dt;
                    }
                    else {
                        player.ResetProximityTime();
                    }
                }

                foreach (var block in Blocks) {
                    if (Collider.Collides(player, block, true)) //see if player will collide with block if he moves
                    {
                        //TODO: THIS
                        break;
                    }
                }
                
                 var hasCollided = false;
                
                for (int j = 0; j < Players.Count; j++) {
                    if (i == j)
                        continue;

                    var otherPlayer = Players[j];

                    if(player.IsEnemy)
                    {
                        var distSq = (otherPlayer.Position - player.Position).LengthSquared();

                        if (distSq < closestPlayerDistSq)
                            closestPlayerDistSq = distSq;
                    }

                    if(player.IsAttacking) {
                        if(!player.CollidedPlayersThisAttack.Contains(otherPlayer) && Collider.AttackCollidesWith(player, otherPlayer)) {
                            player.CollidedPlayersThisAttack.Add(otherPlayer);
                            if (player.Weapon == PlayerWeaponType.Torch) {
                                otherPlayer.Health -= 2;
                                otherPlayer.SetOnFire();
                            }
                            else if (player.Weapon == PlayerWeaponType.Sword)
                                otherPlayer.Health -= 3;
                        }
                    }

                    var playerNewX = player.Position.X + player.Velocity.X;
                    
                    var playerNewRect = new Rectangle((int) (playerNewX - GameVariables.PlayerCollisionRadius),
                                                      (int) (player.Position.Y - GameVariables.PlayerCollisionRadius),
                                                      (int) (GameVariables.PlayerCollisionRadius*2),
                                                      (int) (GameVariables.PlayerCollisionRadius*2));

                    var otherOldRect =
                        new Rectangle((int) (otherPlayer.Position.X - GameVariables.PlayerCollisionRadius),
                                      (int) (otherPlayer.Position.Y - GameVariables.PlayerCollisionRadius),
                                      (int) (GameVariables.PlayerCollisionRadius*2),
                                      (int) (GameVariables.PlayerCollisionRadius*2));


                    if (playerNewRect.Intersects(otherOldRect)) {
                        if (player.Velocity.X > 0) {
                            player.Velocity =
                                new Vector2(
                                    (otherPlayer.Position.X - player.Position.X - 2*GameVariables.PlayerCollisionRadius), player.Velocity.Y);
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                               otherPlayer.Velocity.Y);
                        }
                        else if (player.Velocity.X < 0) {

                            player.Velocity =
                                new Vector2(
                                    (otherPlayer.Position.X - player.Position.X + 2*GameVariables.PlayerCollisionRadius), player.Velocity.Y);
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                               otherPlayer.Velocity.Y);
                        }

                        hasCollided = !otherPlayer.IsEnemy;
                        if (otherPlayer.IsBurning)
                            player.SetOnFire();
                        if(player.IsBurning)
                            otherPlayer.SetOnFire();

                        otherPlayer.HasCollidedWithPlayerThisFrame = true;
                    }

                    var playerNewY = player.Position.Y + player.Velocity.Y;

                    playerNewRect = new Rectangle((int) (player.Position.X - GameVariables.PlayerCollisionRadius),
                                                  (int) (playerNewY - GameVariables.PlayerCollisionRadius),
                                                  (int) (GameVariables.PlayerCollisionRadius*2),
                                                  (int) (GameVariables.PlayerCollisionRadius*2));

                    if (playerNewRect.Intersects(otherOldRect)) {
                        if (player.Velocity.Y > 0) {

                            player.Velocity = new Vector2(player.Velocity.X,
                                                          (otherPlayer.Position.Y - player.Position.Y -
                                                           2*GameVariables.PlayerCollisionRadius));
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                               otherPlayer.Velocity.Y);
                        }
                        else if (player.Velocity.Y < 0) {

                            player.Velocity = new Vector2(player.Velocity.X,
                                                          (otherPlayer.Position.Y - player.Position.Y +
                                                           2*GameVariables.PlayerCollisionRadius));
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                               otherPlayer.Velocity.Y);
                        }
                        hasCollided = !otherPlayer.IsEnemy;
                        if(otherPlayer.IsBurning)
                            player.SetOnFire();
                        if (player.IsBurning)
                            otherPlayer.SetOnFire();
                        otherPlayer.HasCollidedWithPlayerThisFrame = true;
                    }
                }

                var screenBounds = new Rectangle(0,0,(int)_gameResolution.X, (int)_gameResolution.Y);

                if (GameVariables.IsScreenWrapping)
                {
                    if (player.IsEnemy && player.Velocity != Vector2.Zero)
                        isEnemyMoving = true;

                    player.ApplyVelocity();
                    player.WrapPositionAround();
                }
                else {

                    if (screenBounds.Contains((int) (player.Position.X + player.Velocity.X),
                                              (int) (player.Position.Y + player.Velocity.Y))) {
                        player.ApplyVelocity();
                    }
                }

                player.ResetVelocity();

                if(hasCollided && !player.HasCollidedWithPlayerThisFrame) {
                    //AddCandle(player);
                    player.HasCollidedWithPlayerThisFrame = true;
                }
                else if(!hasCollided) {
                    player.HasCollidedWithPlayerThisFrame = false;
                }
                
                if(player.HasSpentTooMuchTimeNearEnemy || player.Health <= 0) {
                    KillPlayer(player);
                }
            }

            var ratio = closestPlayerDistSq/(GameVariables.EnemyKillRadius*GameVariables.EnemyKillRadius);

            if(ratio <= 1)
            {
                GamePad.SetVibration(Enemy.PlayerNum, ratio, ratio);
            }
            else
            {
                GamePad.SetVibration(Enemy.PlayerNum, 0, 0);
            }

            if (isEnemyMoving)
                SoundManager.GetSoundInstance("footstep").Play();

            //all entities have proper state updated now, now check for the following types of interactions
            // Player vs Prop
            // Enemy vs Player

            HandlePropCollision();
        }

        private void HandlePropCollision()
        {
            var collidingProps = new List<Prop>();
            var interactingProps = new List<Prop>();

            foreach (var player in Players)
            {
                collidingProps.Clear();
                interactingProps.Clear();

                foreach (var prop in Props.Where(p => !(p is AttachedCandle)))
                {
                    if (Collider.Collides(player, prop))
                    {
                        if (player.IsInteractingWithProp)
                        {
                            if (prop.CanInteract)
                            {
                                interactingProps.Add(prop);
                                prop.OnInteract(player);
                            }
                        }

                        collidingProps.Add(prop);
                        prop.OnCollide(player);
                    }
                }

                var coinsToBeRemoved = Props.Where(p => p.PropType == PropTypeEnum.Coin && p.IsToBeRemoved).ToList();
                Props.RemoveAll(p => p.IsToBeRemoved);

                if (GameVariables.CoinCanRespawn) {
                    foreach (var coin in coinsToBeRemoved) {
                        coin.IsToBeRemoved = false;
                        coin.Lifetime = -1*
                                        (float)
                                        (GameDriver.RandomGen.NextDouble() *
                                         (GameVariables.CoinRespawnRateMax - GameVariables.CoinRespawnRateMin) +
                                         GameVariables.CoinRespawnRateMin);
                        PropsToBeAdded.Add(coin);
                    }
                }

                // interacting + no prop -> place candle

                if (player.IsInteractingWithProp && !interactingProps.Any())
                {
                    //- place candle, if player has any candles left, place candle, decrement, otherwise nothing

                    AddCandle(player);
                }
            }
        }

        public void DrawScene(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null,null,GameVariables.CameraZoomMatrix);

            DrawBackground(sb);
            
            foreach (var prop in Props.OrderByDescending(p => p.PropType))
                prop.Draw(sb);

            foreach (var player in Players)
                player.Draw(sb);

            foreach (var block in Blocks)
                block.Draw(sb);

            sb.End();
        }

        public void DrawBackground(SpriteBatch sb)
        {
            //sb MUST have been Begin'd

            sb.Draw(TextureManager.GetTexture("background"), new Rectangle(0, 0, (int)_gameResolution.X, (int)_gameResolution.Y), Color.White);
        }

        public void AddPlayer(Player player, PlayerIndex playerIndex)
        {
            AddPlayer(player);
            Players.Last().PlayerNum = playerIndex;
        }


        public void AddPlayer(Player p)
        {
            if (Players.Contains(p)) return;

            //Props.Add(new AttachedCandle("candle", p) { Radius = GameVariables.PlayerLanternRadius });
            Players.Add(p);
        }

        private void AddCandle(Player player)
        {
            if (player.NumCandlesLeft > 0) {
                player.NumCandlesLeft--;
                var candle = new Candle("candle", player.Position, player, GameVariables.CandleLifetime);

                Props.Add(candle);
            }
        }

        public void AddCoin(Vector2 position)
        {
            Props.Add(new Coin(position));
        }

        public void AddBlock(Vector2 topLeftCorner, int size)
        {
            Blocks.Add(new Block(topLeftCorner, size));
        }

        public void MarkPlayerAsEnemy(Player player)
        {
            player.CanPickUpCoins = false;
            player.IsEnemy = true;
            player.Color = Color.Red;

            Enemy = player;
            var idx = Props.FindLastIndex(p => p is AttachedCandle && ((AttachedCandle)p).Owner == player);

            if(idx >= 0)
                Props.RemoveAt(idx);
        }

        public void KillPlayer(Player player)
        {
            if(player.IsEnemy)
                State = GameState.PlayersWin;
            else if(Players.Count(p => !p.IsEnemy) == 1)
                State = GameState.EnemyWins;

            Players.Remove(player);

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
