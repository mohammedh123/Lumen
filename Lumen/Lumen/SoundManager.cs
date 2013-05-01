using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Lumen
{
    public class SoundManager
    {
        private static Dictionary<string, SoundEffect> _soundDetails;
        private static Dictionary<string, SoundEffectInstance> _soundInstances;
        private static Dictionary<string, Song> _songDetails; 

        public static void LoadContent(ContentManager contentManager)
        {
            _soundDetails = new Dictionary<string, SoundEffect>();
            _soundInstances = new Dictionary<string, SoundEffectInstance>();
            _songDetails = new Dictionary<string, Song>();

            LoadSoundEffectInformation(contentManager);
        }

        public static SoundEffect GetSound(string name)
        {
            SoundEffect soundEffect;
            if (_soundDetails.TryGetValue(name, out soundEffect))
                return soundEffect;

            throw new ArgumentException(String.Format("A sound effect with the name {0} has not been added to the sound effect map yet.", name), "name");
        }

        public static Song GetSong(string name)
        {
            Song song;
            if (_songDetails.TryGetValue(name, out song))
                return song;

            throw new ArgumentException(String.Format("A song with the name {0} has not been added to the sound effect map yet.", name), "name");
        }

        public static SoundEffectInstance GetSoundInstance(string name)
        {
            SoundEffectInstance soundEffect;
            if (_soundInstances.TryGetValue(name, out soundEffect))
                return soundEffect;

            throw new ArgumentException(String.Format("A sound effect instance with the name {0} has not been added to the sound effect instances map yet.", name), "name");
        }

        private static void LoadSoundEffectInformation(ContentManager contentManager)
        {
            var footstepSound = contentManager.Load<SoundEffect>("Sounds/footstep");
            _soundDetails.Add("footstep", footstepSound);
            _soundInstances.Add("footstep", footstepSound.CreateInstance());

            var mainSong = contentManager.Load<Song>("Sounds/main_bgm");
            _songDetails.Add("main_bgm", mainSong);

            var deathSound = contentManager.Load<SoundEffect>("Sounds/death_sound");
            _soundDetails.Add("death_sound", deathSound);
            _soundInstances.Add("death_sound", deathSound.CreateInstance());

            var crystalGetSound = contentManager.Load<SoundEffect>("Sounds/crystal_get");
            _soundDetails.Add("crystal_get", crystalGetSound);
            _soundInstances.Add("crystal_get", crystalGetSound.CreateInstance());

            var playerHitSound = contentManager.Load<SoundEffect>("Sounds/player_hit");
            _soundDetails.Add("player_hit", playerHitSound);
            _soundInstances.Add("player_hit", playerHitSound.CreateInstance());

            var guardianChargeSound = contentManager.Load<SoundEffect>("Sounds/guardian_charge");
            _soundDetails.Add("guardian_charge", guardianChargeSound);
            _soundInstances.Add("guardian_charge", guardianChargeSound.CreateInstance());

            var guardianReleaseSound = contentManager.Load<SoundEffect>("Sounds/guardian_release");
            _soundDetails.Add("guardian_release", guardianReleaseSound);
            _soundInstances.Add("guardian_release", guardianReleaseSound.CreateInstance());

            var playerLightSound = contentManager.Load<SoundEffect>("Sounds/player_light");
            _soundDetails.Add("player_light", playerLightSound);
            _soundInstances.Add("player_light", playerLightSound.CreateInstance());

            var crystalChargeSound = contentManager.Load<SoundEffect>("Sounds/crystal_charge");
            _soundDetails.Add("crystal_charge", crystalChargeSound);
            _soundInstances.Add("crystal_charge", crystalChargeSound.CreateInstance());

            var crystalHitSound = contentManager.Load<SoundEffect>("Sounds/crystal_hit");
            _soundDetails.Add("crystal_hit", crystalHitSound);
            _soundInstances.Add("crystal_hit", crystalHitSound.CreateInstance());
        }
    }
}
