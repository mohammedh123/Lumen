using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumen.Entities;
using Lumen.Props;
using Microsoft.Xna.Framework;

namespace Lumen.Light_System
{
    class LightSpawner
    {
        private struct LightData
        {
            public float IntensityDecay;
            public Entity EntityAttachedTo;
        };

        private readonly Dictionary<ILightProvider, LightData> _managedLights = new Dictionary<ILightProvider, LightData>();

        #region Singleton Data

        private LightSpawner()
        {
        }

        private LightSpawner(LightSpawner ls)
        {
        }

        private static LightSpawner _instance = null;

        public static LightSpawner Instance
        {
            get { return _instance ?? (_instance = new LightSpawner()); }
        }

        #endregion

        public IEnumerable<ILightProvider> GetLights()
        {
            return _managedLights.Keys;
        }

        public void AddStaticLight(Vector2 position, float intensity, float radius, float intensityDecay)
        {
            var l = CreateLight(position, intensity, radius);

            _managedLights.Add(l, new LightData { IntensityDecay = intensityDecay});
        }

        public void AddAttachedLight(Entity e, float intensity, float radius, float intensityDecay)
        {
            var l = CreateLight(e.Position, intensity, radius);

            _managedLights.Add(l, new LightData { IntensityDecay = intensityDecay, EntityAttachedTo = e});
        }

        private static ILightProvider CreateLight(Vector2 position, float intensity, float radius)
        {
            return new BasicLight
                   {
                            LightIntensity = intensity,
                            LightRadius = radius,
                            LightColor = Color.White,
                            Position = position,
                            IsVisible = true
                        };
        }

        public void Update(float dt)
        {
            var lightsToRemove = new List<ILightProvider>(_managedLights.Count);

            foreach(var kvp in _managedLights) {
                kvp.Key.LightIntensity -= kvp.Value.IntensityDecay*dt;

                if (kvp.Value.EntityAttachedTo != null)
                    kvp.Key.Position = kvp.Value.EntityAttachedTo.Position;

                if(kvp.Key.LightIntensity <= 0)
                    lightsToRemove.Add(kvp.Key);
            }

            foreach (var light in lightsToRemove) _managedLights.Remove(light);
        }

        public void Reset()
        {
            _managedLights.Clear();
        }
    }
}
