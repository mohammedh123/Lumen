﻿using System;
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
        public static readonly Vector2 PlayerOrigin = new Vector2(12,12);

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

            throw new ArgumentException(String.Format("A texture with the name {0} has not been added to the texture details map yet.", name), "name");
        }

        public static Vector2 GetOrigin(string name)
        {
            TextureDetails td;
            if (_textureDetails.TryGetValue(name, out td))
                return td.Origin;

            throw new ArgumentException(String.Format("A texture origin with the name {0} has not been added to the texture details map yet.", name), "name");
        }

        public static SpriteFont GetFont(string name)
        {
            SpriteFont f;
            if (_fontDetails.TryGetValue(name, out f))
                return f;
            
            throw new ArgumentException(String.Format("A font with the name {0} has not been added to the font map yet.", name), "name");
        }

        private static void LoadTextureInformation(ContentManager contentManager)
        {
            _textureDetails.Add("blank", new TextureDetails(contentManager.Load<Texture2D>("blank"), Vector2.Zero));
            _textureDetails.Add("drunkard", new TextureDetails(contentManager.Load<Texture2D>("drunkard"), new Vector2(12, 12)));
            _textureDetails.Add("player",   new TextureDetails(contentManager.Load<Texture2D>("player"), new Vector2(12,12)));
            _textureDetails.Add("enemy", new TextureDetails(contentManager.Load<Texture2D>("enemy"), new Vector2(12, 12)));
            _textureDetails.Add("wax", new TextureDetails(contentManager.Load<Texture2D>("wax"), new Vector2(12, 12)));
            _textureDetails.Add("candle", new TextureDetails(contentManager.Load<Texture2D>("candle"), new Vector2(12, 12)));
            _textureDetails.Add("coin", new TextureDetails(contentManager.Load<Texture2D>("coin"), new Vector2(12, 12)));
            _textureDetails.Add("block", new TextureDetails(contentManager.Load<Texture2D>("block"), new Vector2(12, 12)));
            _textureDetails.Add("background", new TextureDetails(contentManager.Load<Texture2D>("background"), new Vector2(512, 384)));

            _fontDetails.Add("debug", contentManager.Load<SpriteFont>("debug_font"));
            _fontDetails.Add("large_font", contentManager.Load<SpriteFont>("reallyLargeFont"));
        }
    }
}
