using System;
using System.Collections.Generic;
using Lumen.Light_System;
using Lumen.Particle_System;
using Lumen.Props;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lumen.Entities
{
    class Player : Entity, IKeyboardCapable, IControllerCapable
    {
        public PlayerIndex ControllerIndex { get; set; }

        public int PlayerSpriteIndex;

        public bool IsInteractingWithProp = false;

        public BlinkingLight AttachedLight = null;
        public Crystal CollectionTarget;
        public float CollectingTime = -1;
        public int CrystalCount = 0;
        public List<OrbitingParticle> Orbs;
        
        public bool IsCollecting
        {
            get { return CollectingTime >= 0.0f; } 
        }

        public Player(string textureKey, Vector2 position) : base(textureKey, position)
        {
            Health = GameVariables.PlayerStartingHealth;

            Orbs = new List<OrbitingParticle>();

            for (int i = 0; i < Health; i++) {
                var atu = i*(MathHelper.TwoPi/Health);

                Orbs.Add(new OrbitingParticle(TextureManager.GetTexture(textureKey), new Rectangle(0,0,32,32), TextureManager.GetOrigin(textureKey), this, GameVariables.PlayerOrbsDistance, GameVariables.PlayerOrbsPeriod, atu)
                          {
                              Alpha = 1.0f,
                              Scale = 0.5f,
                              Angle = atu,
                              Color = Color.White
                          });
            }

            CollectingTime = 0.0f;
        }

        public void ResetOrbs()
        {
            foreach (var orb in Orbs)
                orb.IsVisible = true;
        }

        public override void Update(float dt)
        {
#if DEBUG
            if (ControllerIndex <= PlayerIndex.Two)
            {
                ProcessKeyDownAction(dt);
                ProcessKeyUpAction(dt);
                ProcessKeyPressAction(dt);
            }
#endif
            ProcessControllerInput(dt);
            foreach(var o in Orbs)
                o.Update(dt);
        }

        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(ControllerIndex).IsConnected)
                return;
            
            var changeLeft = InputManager.GamepadLeft(ControllerIndex);

            var speedToUse = GameVariables.PlayerSpeed;

            if(!IsCollecting)
                AdjustVelocity(changeLeft.X*speedToUse*dt, -changeLeft.Y*speedToUse*dt);

            if (Velocity != Vector2.Zero)
                Angle = (float)Math.Atan2(Velocity.Y, Velocity.X);

            AttachedLight.IsVisible = Velocity != Vector2.Zero;

            IsInteractingWithProp = InputManager.GamepadButtonPressed(ControllerIndex, Buttons.A);
        }

        public void ProcessKeyDownAction(float dt)
        {
            var speedToUse = GameVariables.PlayerSpeed;

            if (!IsCollecting)
            {
                if(InputManager.KeyDown(Keys.Left))
                    AdjustVelocity(-speedToUse*dt, 0);
                if(InputManager.KeyDown(Keys.Right))
                    AdjustVelocity(speedToUse*dt, 0);
                if(InputManager.KeyDown(Keys.Up))
                    AdjustVelocity(0, -speedToUse*dt);
                if(InputManager.KeyDown(Keys.Down))
                    AdjustVelocity(0, speedToUse*dt);
            }

            if(InputManager.KeyDown(Keys.Z))
                IsInteractingWithProp = true;
        }

        public void ProcessKeyUpAction(float dt)
        {
            if(!GamePad.GetState(ControllerIndex).IsConnected && InputManager.KeyUp(Keys.Space))
                ResetCollecting();

            if(InputManager.KeyUp(Keys.Z))
                IsInteractingWithProp = false;
        }

        public void ProcessKeyPressAction(float dt)
        {
            if (InputManager.KeyPressed(Keys.Space))
                Collect();
        }

        public void Collect()
        {
            CollectingTime = 0.0f;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            foreach(var o in Orbs)
                o.Draw(sb);
        }

        public void ResetCollecting()
        {
            if(CollectionTarget != null)
                CollectionTarget.DecrementCollectorCount(this);

            CollectionTarget = null;
            CollectingTime = 0.0f;
        }

        public void TakeDamage(int n)
        {
            Health -= n;

            int count = 0;
            foreach(var orb in Orbs) {
                if (count >= n) break;
                if(orb.IsVisible) {
                    count++;
                    orb.IsVisible = false;
                    LightSpawner.Instance.AddStaticLight(orb.Position, 1.0f, 12.0f, 2.0f);
                }
            }

            ParticleSystemManager.Instance.FireParticleSystem("player_hit", Position.X, Position.Y);
        }

        public void IncrementCrystalCount()
        {
            CrystalCount++;
            AttachedLight.IncreaseFrequency(1.5f);
        }
    }
}
