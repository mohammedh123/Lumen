using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen
{
    class GameManager
    {
        public List<Player> Players { get; set; }
        public List<Enemy>  Enemies { get; set; }
        public List<Prop>   Props   { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Prop> PropsToBeAdded { get; set; }

        private readonly Vector2 _gameResolution;

        public GameManager(Vector2 gameResolution)
        {
            Players = new List<Player>();
            Enemies = new List<Enemy>();
            Props = new List<Prop>();
            Blocks = new List<Block>();
            PropsToBeAdded = new List<Prop>();

            _gameResolution = gameResolution;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //iterate through all entities and update them (their deltas will be set at the end of update)
            foreach(var player in Players)
                player.Update(dt);

            foreach(var enemy in Enemies)
                enemy.Update(dt);

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
            for (int i = 0; i < Players.Count; i++) {
                var player = Players[i];

                foreach (var block in Blocks) {
                    if (Collider.Collides(player, block, true)) //see if player will collide with block if he moves
                    {
                        //TODO: THIS
                        break;
                    }
                }

                for (int j = 0; j < Players.Count; j++) {
                    if (i == j)
                        continue;

                    var otherPlayer = Players[j];

                    var playerNewX = player.Position.X + player.Velocity.X;

                    var playerOldRect = new Rectangle((int)(player.Position.X - GameVariables.PlayerCollisionRadius),
                                                      (int)(player.Position.Y - GameVariables.PlayerCollisionRadius),
                                                      (int)(GameVariables.PlayerCollisionRadius * 2),
                                                      (int)(GameVariables.PlayerCollisionRadius * 2));

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
                                    (otherPlayer.Position.X - player.Position.X - 2*GameVariables.PlayerCollisionRadius) +
                                    GameVariables.PlayerSpeed*0.5f*dt, player.Velocity.Y);
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X + GameVariables.PlayerSpeed*0.5f*dt,
                                                               otherPlayer.Velocity.Y);
                        }
                        else if (player.Velocity.X < 0) {

                            player.Velocity =
                                new Vector2(
                                    (otherPlayer.Position.X - player.Position.X + 2*GameVariables.PlayerCollisionRadius) -
                                    GameVariables.PlayerSpeed * 0.5f * dt, player.Velocity.Y);
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X - GameVariables.PlayerSpeed*0.5f*dt,
                                                               otherPlayer.Velocity.Y);
                        }
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
                                                           2*GameVariables.PlayerCollisionRadius) +
                                                          GameVariables.PlayerSpeed * 0.5f * dt);
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                               otherPlayer.Velocity.Y + GameVariables.PlayerSpeed * 0.5f * dt);
                        }
                        else if (player.Velocity.Y < 0) {

                            player.Velocity = new Vector2(player.Velocity.X,
                                                          (otherPlayer.Position.Y - player.Position.Y +
                                                           2*GameVariables.PlayerCollisionRadius) -
                                                          GameVariables.PlayerSpeed * 0.5f * dt);
                            otherPlayer.Velocity = new Vector2(otherPlayer.Velocity.X,
                                                               otherPlayer.Velocity.Y - GameVariables.PlayerSpeed * 0.5f * dt);
                        }
                    }
                }

                player.ApplyVelocity();
                player.ResetVelocity();
            }

            //all entities have proper state updated now, now check for the following types of interactions
            // Player vs Prop
            // Enemy vs Player

            HandlePropCollision();
        }

        private void HandlePropCollision()
        {
            var collidingProps = new List<Prop>();
            var interactingProps = new List<Prop>();
            var random = new Random();

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

                foreach(var coin in coinsToBeRemoved)
                {
                    coin.IsToBeRemoved = false;
                    coin.Lifetime = -1*(float) (random.NextDouble()*3.0f + 2.0f);
                    PropsToBeAdded.Add(coin);
                }

                // interacting + no prop -> place candle

                if (player.IsInteractingWithProp && !interactingProps.Any())
                {
                    //- place candle, if player has any candles left, place candle, decrement, otherwise nothing

                    if (player.NumCandlesLeft > 0)
                    {
                        player.NumCandlesLeft--;
                        Props.Add(new Candle("candle", player.Position, player));
                    }
                }
            }
        }

        public void DrawScene(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, null, null, null, null,null,GameVariables.CameraZoomMatrix);

            DrawBackground(sb);

            foreach (var player in Players)
                player.Draw(sb);

            foreach (var enemy in Enemies)
                enemy.Draw(sb);

            foreach (var prop in Props)
                prop.Draw(sb);

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

            Props.Add(new AttachedCandle("candle", p) { Radius = GameVariables.PlayerLanternRadius });
            Players.Add(p);
        }

        public void AddCoin(Vector2 position)
        {
            Props.Add(new Coin(position));
        }

        public void AddBlock(Vector2 topLeftCorner, int size)
        {
            Blocks.Add(new Block(topLeftCorner, size));
        }
    }
}
