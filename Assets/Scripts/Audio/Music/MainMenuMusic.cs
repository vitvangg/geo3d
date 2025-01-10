using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Audio
{
    /// <summary>
    /// The music that plays in the main menu.
    /// </summary>
    public class MainMenuMusic : MonoBehaviour
    {
        public static MainMenuMusic Instance;

        private AudioSource _source;

        private void Awake()
        {
            // Check if instance is null
            if (Instance == null)
            {
                Instance = this;

                // Dont destroy on load
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _source = GetComponent<AudioSource>();

            _source.Play();
        }
        public static void StopInstance()
        {
            // Destroy the instance
            Destroy(Instance.gameObject);

            Instance = null;
        }
    }
}
