using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    class TextureManager
    {
        private static Dictionary<string, Texture2D> _textures;
 
        public static void LoadContent(ContentManager contentManager)
        {
            _textures = new Dictionary<string, Texture2D>();
        
            LoadImages();
        }
        
        public static Texture2D GetTexture(string name)
        {
            Texture2D tex;
            _textures.TryGetValue(name, out tex);

            if (tex != null)
                return tex;

            throw new ArgumentException(String.Format("A texture with the name {0} has not been added to the texture map yet.", name), "name");
        }

        private static void LoadImages()
        {
            //TODO: load images here
            throw new NotImplementedException();
        }
    }
}
