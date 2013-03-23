using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen
{
    class GameManager
    {
        protected List<Player> Players { get; set; }
        public List<Enemy>  Enemies { get; set; }
        public List<Prop>   Props   { get; set; }

        private readonly Vector2 _gameResolution;

        public GameManager(Vector2 gameResolution)
        {
            Players = new List<Player>();
            Enemies = new List<Enemy>();
            Props = new List<Prop>();

            _gameResolution = gameResolution;
        }

        public void AddPlayer(Player p)
        {
            if (Players.Contains(p)) return;

            Props.Add(new AttachedCandle("candle", p) { Radius = GameVariables.PlayerLanternRadius });
            Players.Add(p);
        }

        public void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //iterate through all entities and update them
            foreach(var player in Players)
                player.Update(dt);

            foreach(var enemy in Enemies)
                enemy.Update(dt);

            foreach(var prop in Props)
                prop.Update(dt);

            //all entities have proper state updated now, now check for the following types of interactions
            // Player vs Prop
            // Enemy vs Player

            foreach(var player in Players) {
                Prop interactingProp = null, collidingProp = null;

                foreach (var prop in Props.Where(p => !(p is AttachedCandle)))
                {
                    if (Collider.Collides(player, prop)) {
                        if (player.IsInteractingWithProp) {
                               interactingProp = prop;
                            prop.OnInteract(player);
                        }

                        collidingProp = prop;

                        break;
                    }
                }
                
                // cases:
                // interacting + candle ->
                //      if not players candle, pick up, increase player candle count
                //      else do nothing
                // interacting/not interacting + wax -> increase radius...or something
                // interacting + no prop -> place candle

                if(!player.IsInteractingWithProp && collidingProp != null) {
                    if(collidingProp.PropType == PropTypeEnum.Wax) {
                        //increase candle radius...TODO
                    }
                }
                else if (player.IsInteractingWithProp && interactingProp != null) 
                {
                    if(interactingProp.PropType == PropTypeEnum.Wax) {
                        //for now, do nothing. maybe we do something later...
                    }
                    else if(interactingProp.PropType == PropTypeEnum.Candle) {
                        var candleProp = interactingProp as Candle;

                        //pick up candle, increase player's candle count
                        player.NumCandlesLeft++;

                        Props.Remove(candleProp);
                    }
                }
                else if (player.IsInteractingWithProp && collidingProp == null && interactingProp == null) {
                    //- place candle, if player has any candles left, place candle, decrement, otherwise nothing

                    if (player.NumCandlesLeft > 0) {
                        player.NumCandlesLeft--;
                        Props.Add(new Candle("candle", player.Position, player));
                    }
                }
            }
        }

        public void DrawScene(SpriteBatch sb)
        {
            sb.Begin();

            DrawBackground(sb);

            foreach (var player in Players)
                player.Draw(sb);

            foreach (var enemy in Enemies)
                enemy.Draw(sb);

            foreach (var prop in Props)
                prop.Draw(sb);

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
    }
}
