using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lumen
{
    class Block : PhysicsProp
    {
        public bool IsMoveable { get; set; }
        public Rectangle BoundingBox;

        public Block(Vector2 topLeftCorner, int size, World world, bool isMoveable = false) : base("block", new Vector2(topLeftCorner.X + size/2.0f, topLeftCorner.Y + size/2.0f), size, world, !isMoveable)
        {
            IsMoveable = isMoveable;
            BoundingBox = new Rectangle((int)topLeftCorner.X, (int)topLeftCorner.Y, size, size);

            var cv = topLeftCorner;
            if (!isMoveable) {
                var bodyDef = new BodyDef
                              {
                                  position = topLeftCorner/ GameVariables.PixelsInOneMeter,
                                  fixedRotation = true
                              };

                Body = world.CreateBody(bodyDef);

                var tlCorner = Vector2.Zero;
                var trCorner = Vector2.Zero;
                var blCorner = Vector2.Zero;
                var brCorner = Vector2.Zero;

                trCorner += new Vector2(size, 0);
                blCorner += new Vector2(0, size);
                brCorner += new Vector2(size, size);

                tlCorner /= GameVariables.PixelsInOneMeter;
                trCorner /= GameVariables.PixelsInOneMeter;
                blCorner /= GameVariables.PixelsInOneMeter;
                brCorner /= GameVariables.PixelsInOneMeter;

                var leftEdge = new EdgeShape();
                leftEdge.Set(tlCorner, blCorner);
                var topEdge = new EdgeShape();
                topEdge.Set(trCorner, tlCorner);
                var rightEdge = new EdgeShape();
                rightEdge.Set(brCorner, trCorner);
                var botEdge = new EdgeShape();
                botEdge.Set(blCorner, brCorner);

                var fix = Body.CreateFixture(leftEdge, 0.0f);
                fix.SetRestitution(0);
                fix.SetFriction(0);
                fix = Body.CreateFixture(topEdge, 0.0f);
                fix.SetRestitution(0);
                fix.SetFriction(0);
                fix = Body.CreateFixture(rightEdge, 0.0f);
                fix.SetRestitution(0);
                fix.SetFriction(0);
                fix = Body.CreateFixture(botEdge, 0.0f);
                fix.SetRestitution(0);
                fix.SetFriction(0);
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(Texture,BoundingBox, Color);
        }
    }
}
