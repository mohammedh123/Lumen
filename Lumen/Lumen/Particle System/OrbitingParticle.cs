using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Light_System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen.Particle_System
{
    class OrbitingParticle : Particle, ILightProvider
    {
        private Texture2D _texture;
        private Rectangle _textureRect;
        private Vector2 _textureOrigin;
        private float _initialAngle;

        public Entity CenterEntity { get; set; }
        public float DistanceFromCenter { get; set; }
        public float OrbitPeriod { get; set; } //1 means 1 complete rotation in 1 second

        public Color LightColor { get; set; }
        public float LightRadius { get; set; }
        public float LightIntensity { get; set; }

        public bool IsVisible { get; set; }


        public OrbitingParticle(Texture2D tex, Rectangle texRect, Vector2 texOrigin, Entity e, float distFromCenter, float orbitPeriod, float startingAngle)
        {
            LightColor = Color.White;
            LightRadius = 8.0f;
            LightIntensity = 1.0f;

            _initialAngle = Angle = startingAngle;
            _texture = tex;
            _textureRect = texRect;
            _textureOrigin = texOrigin;

            IsVisible = true;
            CenterEntity = e;
            DistanceFromCenter = distFromCenter;
            OrbitPeriod = orbitPeriod;
            Lifetime = 0.0f;
        }

        public void Update(float dt)
        {
            Lifetime += dt;

            Angle += (dt*OrbitPeriod)*MathHelper.TwoPi;
            if (CenterEntity != null) {
                Position = CenterEntity.Position +
                           new Vector2((float) Math.Cos(Angle), (float) Math.Sin(Angle))*
                           DistanceFromCenter;
            }
            else {
                IsVisible = false;
            }
        }

        public void Draw(SpriteBatch sb)
        {
            if(IsVisible)
                sb.Draw(_texture,Position,_textureRect,Color, Angle,_textureOrigin,Scale,SpriteEffects.None, 0);
        }
    }
}
