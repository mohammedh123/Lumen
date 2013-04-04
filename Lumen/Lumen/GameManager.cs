using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Lumen.Entities;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

namespace Lumen
{
    enum GameState
    {
        Victory,
        Lost,
        Playing
    }

    class GameManager
    {
        public List<Player> Players { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Drunkard> Drunkards { get; set; }
        public List<IProp> Props { get; set; }
        public List<Block> Blocks { get; set; }
        public List<Rectangle> GoalAreas { get; set; }
        public List<IProp> PropsToBeAdded { get; set; }

        public World World;

        public GameState GameState = GameState.Playing;

        private readonly Vector2 _gameResolution;

        public GameManager(Vector2 gameResolution)
        {
            Players = new List<Player>();
            Enemies = new List<Enemy>();
            Drunkards = new List<Drunkard>();
            Props = new List<IProp>();
            Blocks = new List<Block>();
            PropsToBeAdded = new List<IProp>();
            GoalAreas = new List<Rectangle>();

            var worldAABB = new AABB
                            {
                                lowerBound = new Vector2(-gameResolution.X, -gameResolution.Y),
                                upperBound = new Vector2(gameResolution.X, gameResolution.Y)
                            };
            World = new World(Vector2.Zero, true);

            _gameResolution = gameResolution;
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            World.Step(1.0f/60.0f, 10, 1);

            //iterate through all entities and update them (their deltas will be set at the end of update)
            foreach (var player in Players)
                player.Update(dt);

            foreach (var enemy in Enemies)
                enemy.Update(dt);

            GameState = GameState.Playing;
            foreach (var drunkard in Drunkards) {
                drunkard.Update(dt);

                if (GameState == GameState.Playing) {
                    foreach (var goal in GoalAreas.Where(goal => Collider.Collides(drunkard, goal))) {
                        GameState = GameState.Victory;
                        break;
                    }

                    var anyShine = false;
                    foreach (var candle in Props.Where(p => p.PropType == PropTypeEnum.Candle).Cast<Candle>()) {
                        if (Collider.CircleRect(candle.Position.X, candle.Position.Y, candle.Radius*0.8f,
                                                new Rectangle(
                                                    (int) (drunkard.Position.X - GameVariables.DrunkardCollisionRadius),
                                                    (int) (drunkard.Position.Y - GameVariables.DrunkardCollisionRadius),
                                                    2*(int) GameVariables.DrunkardCollisionRadius,
                                                    2*(int) GameVariables.DrunkardCollisionRadius))) {
                            anyShine = true;
                            break;
                        }
                    }

                    if (!anyShine) {
                        GameState = GameState.Lost;
                        break;
                    }
                }
            }

            foreach(var prop in Props)
                prop.Update(dt);


            foreach (var prop in PropsToBeAdded)
            {
                prop.Lifetime += dt;
            }

            var propsToBeRemoved = PropsToBeAdded.Where(kvp => kvp.Lifetime > 0).ToList();

            foreach (var prop in propsToBeRemoved)
            {
                prop.Lifetime = 0.0f;

                Props.Add(prop);

                propsToBeRemoved.Remove(prop);
            }

            for (int i = 0; i < Players.Count; i++) {
                var player = Players[i];

                //player.Body.ApplyImpulse(new Vec2(player.Velocity.X / GameVariables.PixelsInOneMeter, player.Velocity.Y / GameVariables.PixelsInOneMeter), new Vec2());
                player.Body.SetLinearVelocity(player.Velocity/GameVariables.PixelsInOneMeter);
                player.Velocity = Vector2.Zero;
            }

            foreach(var drunkard in Drunkards) {
                drunkard.Body.SetLinearVelocity(drunkard.Velocity);
                drunkard.Velocity = Vector2.Zero;
            }


            HandlePropCollision();
        }

        private void HandlePropCollision()
        {
            var collidingProps = new List<IProp>();
            var interactingProps = new List<IProp>();
            var random = new Random();

            foreach (var player in Players)
            {
                collidingProps.Clear();
                interactingProps.Clear();

                foreach (var prop in Props)
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
                    coin.Lifetime = -1*(float) (random.NextDouble()*(GameVariables.CoinRespawnRateMax-GameVariables.CoinRespawnRateMin) + GameVariables.CoinRespawnRateMin);
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

            foreach (var enemy in Enemies)
                enemy.Draw(sb);

            foreach (var prop in Props.OrderByDescending(p => p.PropType))
                prop.Draw(sb);

            foreach (var drunkard in Drunkards)
                drunkard.Draw(sb);

            foreach (var player in Players)
                player.Draw(sb);

            foreach (var block in Blocks)
                block.Draw(sb);

            foreach(var area in GoalAreas) {
                sb.Draw(TextureManager.GetTexture("blank"), area, Color.Red*0.5f);
            }

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

            Props.Add(new AttachedCandle("candle", p) { Radius = GameVariables.PlayerLanternRadius, Position = p.Position});
            Players.Add(p);
        }

        public void AddCoin(Vector2 position)
        {
            Props.Add(new Coin(position));
        }

        public void AddBlock(Vector2 topLeftCorner, int size)
        {
            Blocks.Add(new Block(topLeftCorner, size, World));
        }

        public void AddGoalArea(Vector2 topLeftCorner, int size)
        {
            GoalAreas.Add(new Rectangle((int)topLeftCorner.X, (int)topLeftCorner.Y, size, size));
        }

        public void AddDrunkard(Vector2 position)
        {
            Drunkards.Add(new Drunkard(position, World));
        }

        public void Clear()
        {
            Players.Clear();
            Props.Clear();
            Enemies.Clear();
            PropsToBeAdded.Clear();
            Blocks.Clear();
            GoalAreas.Clear();
            Drunkards.Clear();
        }
    }
}
