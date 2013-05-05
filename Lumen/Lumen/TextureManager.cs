using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    public class TextureManager
    {
        public static readonly Vector2 PlayerOrigin = new Vector2(16, 16);

        private static Dictionary<string, TextureDetails> _textureDetails;
        private static Dictionary<string, SpriteFont> _fontDetails;

        public static void LoadContent(ContentManager contentManager)
        {
            _textureDetails = new Dictionary<string, TextureDetails>();
            _fontDetails = new Dictionary<string, SpriteFont>();

            LoadTextureInformation(contentManager);
        }

        public static Texture2D GetTexture(string name)
        {
            TextureDetails td;
            if (_textureDetails.TryGetValue(name, out td)) {
                return td.Texture;
            }

            throw new ArgumentException(
                String.Format("A texture with the name \"{0}\" has not been added to the texture details map yet.", name),
                "name");
        }

        public static Vector2 GetOrigin(string name)
        {
            TextureDetails td;
            if (_textureDetails.TryGetValue(name, out td)) {
                return td.Origin;
            }

            throw new ArgumentException(
                String.Format(
                    "A texture origin with the name \"{0}\" has not been added to the texture details map yet.", name),
                "name");
        }

        public static SpriteFont GetFont(string name)
        {
            SpriteFont f;
            if (_fontDetails.TryGetValue(name, out f)) {
                return f;
            }

            throw new ArgumentException(
                String.Format("A font with the name \"{0}\" has not been added to the font map yet.", name), "name");
        }

        private static void LoadTextureInformation(ContentManager contentManager)
        {
            //main game state textures
            _textureDetails.Add("blank",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/blank"), new Vector2(12, 12)));
            _textureDetails.Add("player_portrait",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player_portrait"),
                                                   new Vector2(64, 64)));
            _textureDetails.Add("player_dead_portrait",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player_dead"),
                                                   new Vector2(64, 64)));
            _textureDetails.Add("player1_portrait",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player1"),
                                                   new Vector2(64, 64)));
            _textureDetails.Add("player2_portrait",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player2"),
                                                   new Vector2(64, 64)));
            _textureDetails.Add("player3_portrait",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player3"),
                                                   new Vector2(64, 64)));
            _textureDetails.Add("player1",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player1_mini"),
                                                   new Vector2(16, 16)));
            _textureDetails.Add("player2",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player2_mini"),
                                                   new Vector2(16, 16)));
            _textureDetails.Add("player3",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player3_mini"),
                                                   new Vector2(16, 16)));
            _textureDetails.Add("player4",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/white_player"),
                                                   new Vector2(16, 16)));
            _textureDetails.Add("guardian",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/guardian"),
                                                   new Vector2(32, 32)));
            _textureDetails.Add("player_orb",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/player_orb"),
                                                   new Vector2(8, 8)));
            _textureDetails.Add("crystal",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/crystal"),
                                                   new Vector2(16, 16)));
            _textureDetails.Add("health",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/health"),
                                                   new Vector2(16, 16)));
            _textureDetails.Add("hit_particle",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/hit_particle"),
                                                   new Vector2(1, 1)));
            _textureDetails.Add("background",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/background"),
                                                   new Vector2(512, 384)));

            _textureDetails.Add("white_player",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/white_player"),
                                                   new Vector2(16, 16)));

            //main menu state textures
            _textureDetails.Add("mainmenu_bg",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/mainmenu_bg"),
                                                   new Vector2(0, 0)));
            _textureDetails.Add("mainmenu_logo",
                                new TextureDetails(contentManager.Load<Texture2D>("Textures/mainmenu_logo"),
                                                   new Vector2(357, 117)));

            _fontDetails.Add("debug", contentManager.Load<SpriteFont>("Fonts/debug_font"));
            _fontDetails.Add("big", contentManager.Load<SpriteFont>("Fonts/bigFont"));
        }

        #region Nested type: TextureDetails

        private struct TextureDetails
        {
            public readonly Vector2 Origin;
            public readonly Texture2D Texture;

            public TextureDetails(Texture2D texture, Vector2 origin)
            {
                Texture = texture;
                Origin = origin;
            }
        };

        #endregion
    }
}