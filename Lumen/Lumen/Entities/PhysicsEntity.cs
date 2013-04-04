using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace Lumen.Entities
{
    abstract class PhysicsEntity
    {
        public Vector2 Position
        {
            get { return new Vector2(Body.GetWorldCenter().X, Body.GetWorldCenter().Y)*GameVariables.PixelsInOneMeter; }
        }

        public Vector2 Velocity { get; set; }
        
        protected Texture2D Texture { get; set; }
        protected Vector2 TextureOrigin { get; set; }

        protected float Angle = 0f;
        public Color Color = Color.White;

        public Body Body = null;

        public bool IsVisible = true;

        protected PhysicsEntity(string textureKeyName, Vector2 position, float size, World world, bool handledByDerived = false)
        {
            Texture = TextureManager.GetTexture(textureKeyName);
            TextureOrigin = TextureManager.GetOrigin(textureKeyName);

            if (!handledByDerived) {
                var bodyDef = new BodyDef
                              {
                                  position = new Vector2(position.X/GameVariables.PixelsInOneMeter,
                                                         position.Y/GameVariables.PixelsInOneMeter),
                                  fixedRotation = true
                              };
                bodyDef.type = BodyType.Dynamic;

                Body = world.CreateBody(bodyDef);

                var dynBox = new PolygonShape();
                dynBox.SetAsBox(size / GameVariables.PixelsInOneMeter, size / GameVariables.PixelsInOneMeter, Vector2.Zero,
                                  0.0f);
          
                var shapeDef = new FixtureDef {density = 1, friction = 0, restitution = 0};

                Body.CreateFixture(dynBox, 1);
            }
        }

        public abstract void Update(float dt);
        
        public virtual void Draw(SpriteBatch sb)
        {
            if(IsVisible)
                sb.Draw(Texture, Position, null, Color, Angle,TextureOrigin,1.0f,SpriteEffects.None,0);
        }
    }
}
