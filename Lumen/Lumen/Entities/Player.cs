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

        public Light AttachedLight = null;
        public BlinkingLight AttachedBlinkingLight = null;
        public Crystal CollectionTarget;
        public float CollectingTime = -1;
        public int CrystalCount = 0;

        public OrbitingRing OrbitRing;

        private float _lightVisibilityTimer = -1.0f, _lightModulationSoundTimer = 0.0f;
        
        public bool IsLightOn
        {
            get { return _lightVisibilityTimer >= 0.0f; }
        }

        public bool IsCollecting
        {
            get { return CollectingTime >= 0.0f; } 
        }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        public Player(string textureKey, Vector2 position) : base(textureKey, position)
        {
            Health = GameVariables.PlayerStartingHealth;

            OrbitRing = new OrbitingRing(GameVariables.PlayerOrbsDistance, Health, 0.5f, GameVariables.PlayerOrbsPeriod,textureKey, new Rectangle(0,0,32,32), this);

            CollectingTime = 0.0f;
        }

        public void ResetOrbs()
        {
            OrbitRing.IsVisible = true;
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
            OrbitRing.Update(dt);

            if(IsLightOn)
            {
                _lightVisibilityTimer += dt;
                _lightModulationSoundTimer += dt;
                SoundManager.GetSound("player_light").Play(1.0f,0.5f*(float)Math.Sin(_lightModulationSoundTimer*3.0f),0);
            }
            else {
                _lightModulationSoundTimer = 0.0f;
            }
                
            if (AttachedLight != null) AttachedLight.IsVisible = IsLightOn;
            
            if(_lightVisibilityTimer >= GameVariables.PlayerLightDuration)
                TurnOffLight();
        }

        public void ProcessControllerInput(float dt)
        {
            if (!GamePad.GetState(ControllerIndex).IsConnected)
                return;
            
            var changeLeft = InputManager.GamepadLeft(ControllerIndex);

            var speedToUse = GameVariables.PlayerSpeed;

            AdjustVelocity(changeLeft.X*speedToUse*dt, -changeLeft.Y*speedToUse*dt);

            if (Velocity != Vector2.Zero)
                Angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
            
            AttachedBlinkingLight.IsVisible = Velocity != Vector2.Zero;
            
            if (InputManager.GamepadButtonDown(ControllerIndex, Buttons.A))
                TurnOnLight();

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

        public void TurnOnLight()
        {
            _lightVisibilityTimer = 0.0f;
        }

        public void TurnOffLight()
        {
            _lightVisibilityTimer = -1.0f;
            if (AttachedLight != null) AttachedLight.IsVisible = false;
        }

        public void Collect()
        {
            CollectingTime = 0.0f;
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            OrbitRing.Draw(sb);
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
            foreach(var orb in OrbitRing.Satellites) {
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
            AttachedBlinkingLight.IncreaseFrequency(1.5f);
        }
    }
}
