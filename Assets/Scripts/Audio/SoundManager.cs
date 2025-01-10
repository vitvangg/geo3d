using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using GD3D.ObjectPooling;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GD3D.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        [Header("Sounds")]
        [SerializeField] internal Sound[] sounds;

        [Header("Other Stuff")]
        [SerializeField] private int maxSounds = 50;
        private ObjectPool<SoundObject> pool;

        [Space]
        [SerializeField] private AudioMixerGroup soundEffectMixer;


        private void Awake()
        {
            // Check if there already exists an instance of this object
            if (Instance == null)
            {
                // If not, then set this object to be the instance
                Instance = this;

                // Also set this object to dont destroy on load
                transform.SetParent(null); // We set parent to null so unity won't scream at me with a warning
                DontDestroyOnLoad(gameObject);

                GameObject soundGameObject = new GameObject("Sound", typeof(AudioSource), typeof(SoundObject));

                SoundObject sound = soundGameObject.GetComponent<SoundObject>();

                // Create sound pool
                pool = new ObjectPool<SoundObject>(sound, maxSounds, 
                (obj) =>
                {
                    DontDestroyOnLoad(obj.gameObject);
                });

                // Destroy the original sound game object seing as we no longer need it
                Destroy(soundGameObject);
            }
            else
            {
                // Destroy this object because a sound manager already exists
                Destroy(gameObject);
            }
        }
        public static void PlaySoundClip(AudioClip clip, float volume, float pitch, AudioMixerGroup outputGroup, Vector3 position, bool is3D, Vector2 range, bool playInBothEars = false)
        {
            // Get sound object
            ObjectPool<SoundObject> pool = Instance.pool;

            // Don't spawn any sound if the pool is empty (max sounds have been reached)
            if (pool.IsEmpty())
            {
                return;
            }

            // Spawn a sound from the pool
            SoundObject sound = pool.SpawnFromPool();

            // Get the AudioSource on the sound and set it to be the right sound options
            AudioSource audio = sound.GetSource;
            audio.playOnAwake = false;
            audio.clip = clip;
            audio.volume = volume;
            audio.pitch = pitch;
            audio.outputAudioMixerGroup = outputGroup;
            audio.spatialBlend = is3D ? 1 : 0;
            audio.spread = playInBothEars ? 0 : 180;
            audio.minDistance = range.x;
            audio.maxDistance = range.y;

            // Play the sound
            sound.Play();
        }

        public static bool PlaySound(string name, Vector3 position, float pitch, bool is3D, Vector2 range, bool playInBothEars, bool isGroupedSound)
        {
            // If the name == null or empty then just don't play anything
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            // Get the sound manager and store it in a variable
            SoundManager soundManager = Instance;

            // If the SoundManager doesn't exist then return
            if (soundManager == null)
            {
                return false;
            }

            // Create a new temporary volume variable.
            float volume = 0;

            // Find right audio clip
            AudioClip clip = null;


                foreach (Sound s in soundManager.sounds)
                {
                    // Check if the names are the same. If it is then the right sound has been found!
                    if (s.name == name)
                    {
                        // Get the clip
                        clip = s.clip;
                        volume = s.soundVolume;
                        pitch += s.pitchSlider - 1;
                        break;
                    }
                }

            // If the clip is still null, that means there is no clip with a name that matches
            if (clip == null)
            {
                return false;
            }

            // Play the sound
            PlaySoundClip(clip, volume, pitch, soundManager.soundEffectMixer, position, is3D, range, playInBothEars);

            // The sound was successfully played!
            return true;
        }


        public static bool PlaySound(string name, bool isGroupedSound = false)
        {
            return PlaySound(name, Vector3.zero, Random.Range(0.8f, 1.2f), false, Vector2.zero, false, isGroupedSound);
        }

        public static bool PlaySound(string name, float pitch, bool isGroupedSound = false)
        {
            return PlaySound(name, Vector3.zero, pitch, false, Vector2.zero, false, isGroupedSound);
        }
        public static bool PlaySound(string name, Vector3 position, Vector2 range, bool playInBothEars = false, bool isGroupedSound = false)
        {
            return PlaySound(name, position, Random.Range(0.8f, 1.2f), true, range, playInBothEars, isGroupedSound);
        }

        public static bool PlaySound(string name, Vector3 position, float pitch, Vector2 range, bool playInBothEars = false, bool isGroupedSound = false)
        {
            return PlaySound(name, position, pitch, true, range, playInBothEars, isGroupedSound);
        }


        public void PlayUISound(string name)
        {
            PlaySound(name);
        }
    }
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0, 2)]
        public float soundVolume = 1;
        [Range(0, 2)]
        public float pitchSlider = 1;
    }

}