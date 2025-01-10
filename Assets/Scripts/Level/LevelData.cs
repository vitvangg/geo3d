using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Player;
using GD3D.UI;

namespace GD3D.Level
{
    public class LevelData : MonoBehaviour
    {
        //-- Instance
        public static LevelData Instance;

        /// <summary>
        /// Returns true if we are in the level editor.
        /// </summary>
        public static bool InLevelEditor;
        /// <summary>
        /// Returns true if we are both in the level editor and are playtesting the level.
        /// </summary>
        public static bool InPlayMode;

        [Header("Data")]
        public string LevelName;

        public float NormalPercent;
        public float PracticePercent;

        //-- Instance references
        private PlayerMain _player;

        private SaveFile.LevelSaveData levelData => SaveData.CurrentLevelData;

        private string _oldPercentString;

        private void Awake()
        {
            // Set the instance
            Instance = this;
        }

        private void Start()
        {
            // Get instances
            _player = PlayerMain.Instance;

            // Set percent from save data 1 frame later
            // We do thsi one frame later so that the save data won't be null since it's set in start
            Helpers.TimerEndOfFrame(this, () =>
            {
                NormalPercent = levelData.NormalPercent;
                PracticePercent = levelData.PracticePercent;
            });

            // Subscribe to events
            if (_player != null)
            {
                _player.OnDeath += OnDeath;
                _player.OnDeath += SaveLevelData;
            }
        }

        private void Update()
        {
            
        }

        private void OnDeath()
        {
            float percent = ProgressBar.Percent;

            // Set new percent if the player got a new record
            if (percent > NormalPercent)
            {
                NormalPercent = percent;

                // Only make the new best text appear if the visually shown percent has been changed
                if (_oldPercentString != ProgressBar.PercentString)
                {
                    // Make the player spawn script do the NEW BEST popup animation
                    _player.Spawn.ShowNewBest();
                }
            }
            // Set practice percent in practice mode
            else if (percent > PracticePercent)
            {
                PracticePercent = percent;
            }

            _oldPercentString = ProgressBar.PercentString;
        }

        /// <summary>
        /// Will set the <see cref="SaveData.CurrentLevelData"/> percent to the percent on this object for both normal and practice mode.
        /// </summary>
        private void SaveLevelData()
        {
            // Set the level data values
            levelData.NormalPercent = NormalPercent;
            levelData.PracticePercent = PracticePercent;
        }
    }
}
