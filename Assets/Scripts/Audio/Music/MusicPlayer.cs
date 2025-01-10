using GD3D.Player;
using System;
using UnityEngine;
using PathCreation;
using GD3D.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Audio;
#endif

namespace GD3D.Audio
{
    /// <summary>
    /// Will play the level music
    /// </summary>
    [ExecuteAlways]
    public class MusicPlayer : MonoBehaviour
    {
        //-- Static Variables
        public static MusicPlayer Instance;

        [Header("Important Stuff")]
        [SerializeField] private AudioClip song;
        [SerializeField] private AudioSource practiceMusicSource;

        [Header("BPM")]
        [SerializeField] private float bpm = 160;
        [SerializeField] private float bpmStartOffset = 0;

#if UNITY_EDITOR
        [SerializeField] private bool showBpmLines;
#endif

        public static readonly float[] MAIN_LEVEL_BPMS = new float[]
        {
            160, // Stereo Madness
            142, // Back on Track
            163, // Polargeist
            145, // Dry Out
            141, // Base After Base
            170, // Cant Let Go
            143, // Time Machine
            175, // Jumper
            130, // xStep
            140, // Cycles
            130, // Theory of Everything
            170, // Electroman Adventures
            128, // Clubstep
            127, // Electrodynamix
            140, // Clutterfunk
            81,  // Hexagon Force
            132, // Theory of Everything 2
            135, // Blast Processing
            135, // Deadlocked
            148, // Geometrical Dominator
            112, // Fingerdash
        };

        //-- Other References
        private AudioSource _source;
        private PlayerMain _player;
        private PlayerMovement _playerMovement;

        private PathCreator _pathCreator;
        private VertexPath Path => _pathCreator.path;

        [HideInInspector] public float EndDistance;


        private void Awake()
        {
            // Set instance
            Instance = this;
        }

        private void Start()
        {
            // Get references
            _source = GetComponent<AudioSource>();
            _player = PlayerMain.Instance;

            if (_player != null && _player.Movement != null)
            {
                _playerMovement = _player.Movement;
            }

            _pathCreator = FindObjectOfType<PathCreator>();

            // Setup the audio source
            _source.clip = song;


            // Subscribe to events
            _player.OnDeath += OnDeath;
            _player.OnRespawn += OnRespawn;
            _source.Play();

            // Cache the instance just for this moment
            PauseMenu pauseMenu = PauseMenu.Instance;
        }


        public static void TogglePracticeSong(bool enable)
        {
            if (enable)
            {
                Instance.practiceMusicSource.Play();
                Instance.Stop();
            }
            else
            {
                Instance.practiceMusicSource.Stop();
                Instance._source.Play();
            }
        }

        public void PlayAtDistance(float distance)
        {
            float playerDist = Path.GetClosestDistanceAlongPath(_player.transform.position);
            float time = 0;

            if (distance < playerDist)
            {
                distance = playerDist;
            }

            int length = Mathf.CeilToInt(distance * 10);
            float currentDistance = playerDist;

            for (int i = 0; i < length; i++)
            {
                float speed = _playerMovement.GetSpeedAtDistance(currentDistance);

                float addedTime = (float)((distance - playerDist) / speed) / (float)length;
                time += addedTime;

                currentDistance += speed * addedTime;
            }

            time = Mathf.Clamp(time, 0, _source.clip.length);


            _source.time = time;
            _source.Play();
        }

        public void Stop()
        {


            _source.Stop();
        }

        private void OnDeath()
        {
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Only play the music if we are not in practice mode
            if (true)
            {
                _source.Play();
            }
        }

        public void UpdateEndDistance()
        {
            // Oops
            if (_player == null || _playerMovement == null || _source == null)
            {
                return;
            }

            float playerDist = Path.GetClosestDistanceAlongPath(_player.transform.position);
            float distance = playerDist;

            float time = _source.clip.length;

            int length = Mathf.CeilToInt((Path.length - playerDist) * 10);

            for (int i = 0; i < length; i++)
            {
                float speed = _playerMovement.GetSpeedAtDistance(distance);

                distance += (float)(speed * time) / (float)length;
            }

            EndDistance = distance;
        }

        public float GetDistanceFromMusicTime(float musicTime)
        {
            float playerDist = Path.GetClosestDistanceAlongPath(_player.transform.position);

            float distance = Helpers.Map(0, _source.clip.length, playerDist, EndDistance, musicTime);

            return distance;
        }


        

        internal void OnEditorChange()
        {
            // Correct missing components in case we are missing them in the editor
            if (_player == null)
            {
                _player = FindObjectOfType<PlayerMain>();
            }

            if (_playerMovement == null)
            {
                _playerMovement = FindObjectOfType<PlayerMovement>();
            }

            // Setup the audio source
            _source.clip = song;

            // Update end distance if the song is not null
            if (song != null)
            {
                UpdateEndDistance();
            }
        }
    }
    }
