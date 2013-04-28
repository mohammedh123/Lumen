using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    public class TextureManager
    {
        public static readonly Vector2 PlayerOrigin = new Vector2(16,16);

        private struct TextureDetails
        {
            public Texture2D Texture;
            public Vector2 Origin;

            public TextureDetails(Texture2D texture, Vector2 origin)
            {
                Texture = texture;
                Origin = origin;
            }
        };

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
            if (_textureDetails.TryGetValue(name, out td))
                return td.Texture;

            throw new ArgumentException(String.Format("A texture with the name \"{0}\" has not been added to the texture details map yet.", name), "name");
        }

        public static Vector2 GetOrigin(string name)
        {
            TextureDetails td;
            if (_textureDetails.TryGetValue(name, out td))
                return td.Origin;

            throw new ArgumentException(String.Format("A texture origin with the name \"{0}\" has not been added to the texture details map yet.", name), "name");
        }

        public static SpriteFont GetFont(string name)
        {
            SpriteFont f;
            if (_fontDetails.TryGetValue(name, out f))
                return f;

            throw new ArgumentException(String.Format("A font with the name \"{0}\" has not been added to the font map yet.", name), "name");
        }

        private static void LoadTextureInformation(ContentManager contentManager)
        {
            _textureDetails.Add("blank", new TextureDetails(contentManager.Load<Texture2D>("blank"), new Vector2(12, 12)));
            _textureDetails.Add("player_portrait", new TextureDetails(contentManager.Load<Texture2D>("player_portrait"), new Vector2(64, 64)));
            _textureDetails.Add("player_dead_portrait", new TextureDetails(contentManager.Load<Texture2D>("player_dead"), new Vector2(64, 64)));
            _textureDetails.Add("player1_portrait", new TextureDetails(contentManager.Load<Texture2D>("player1"), new Vector2(64, 64)));
            _textureDetails.Add("player2_portrait", new TextureDetails(contentManager.Load<Texture2D>("player2"), new Vector2(64, 64)));
            _textureDetails.Add("player3_portrait", new TextureDetails(contentManager.Load<Texture2D>("player3"), new Vector2(64, 64)));
            _textureDetails.Add("player1", new TextureDetails(contentManager.Load<Texture2D>("player1_mini"), new Vector2(16, 16)));
            _textureDetails.Add("player2", new TextureDetails(contentManager.Load<Texture2D>("player2_mini"), new Vector2(16, 16)));
            _textureDetails.Add("player3", new TextureDetails(contentManager.Load<Texture2D>("player3_mini"), new Vector2(16, 16)));
            _textureDetails.Add("guardian", new TextureDetails(contentManager.Load<Texture2D>("guardian"), new Vector2(32, 32)));
            _textureDetails.Add("player_orb", new TextureDetails(contentManager.Load<Texture2D>("player_orb"), new Vector2(8, 8)));
            _textureDetails.Add("crystal", new TextureDetails(contentManager.Load<Texture2D>("crystal"), new Vector2(16, 16)));
            _textureDetails.Add("health", new TextureDetails(contentManager.Load<Texture2D>("health"), new Vector2(16, 16)));
            _textureDetails.Add("hit_particle", new TextureDetails(contentManager.Load<Texture2D>("hit_particle"), new Vector2(1, 1)));
            _textureDetails.Add("background", new TextureDetails(contentManager.Load<Texture2D>("background"), new Vector2(512, 384)));

            _fontDetails.Add("debug", contentManager.Load<SpriteFont>("debug_font"));
            _fontDetails.Add("big", contentManager.Load<SpriteFont>("bigFont"));
        }
    }
}
