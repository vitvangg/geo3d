using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using GD3D.Level;

namespace GD3D.Objects
{
    /// <summary>
    /// A trigger is an invisble object that will make something happen at a specific point during the level, like changing the background color gradually.
    /// </summary>
    [RequireComponent(typeof(AttachToPath))]
    public abstract class Trigger : LevelObject
    {
        //-- ID
        protected ObjectIDHandler idHandler;
        [HideInInspector] public long ID;

        private bool _isActivated = false;

        [Header("General Trigger Options")]
        [LevelSave] [SerializeField] private bool isTouchTriggered;
        private SpriteRenderer _sprite;

        private bool _playerHasPassed;

        public bool IsActivated => _isActivated;

        //-- References
        protected AttachToPath _attachToPath;
        protected PlayerMain _player;

        //-- Properties
        public float Distance => _attachToPath.Distance;
        protected bool CanTrigger => !_isActivated && !_player.IsDead && CustomTriggerCondition;

        protected override void Start()
        {
            base.Start();

            // Get references
            _attachToPath = GetComponent<AttachToPath>();
            _player = PlayerMain.Instance;

            // Get sprite
            _sprite = GetComponentInChildren<SpriteRenderer>();

            if (InLevelEditor)
            {
                // Make the sprite appear in the level editor
                _sprite.gameObject.SetActive(true);
            }

            // Subsribe to events
            _player.OnRespawn += OnRespawn;

            // Get the ID handler and generate ID
            idHandler = ObjectIDHandler.Instance;

            ID = idHandler.GetID(this);
        }

        /// <summary>
        /// Override this to do stuff when the player dies
        /// </summary>
        protected virtual void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            /*
            // Execute this one frame later using this epic timer thingy I wrote :D
            Helpers.TimerEndOfFrame(this, () =>
            {
            }
            );
            */
            _isActivated = idHandler.IsActivated(this);
            _playerHasPassed = false;
        }

        protected override void Update()
        {
            base.Update();

            // Return if this trigger is touch triggered (cuz then we check in OnCollisionEnter())
            if (isTouchTriggered || _playerHasPassed)
            {
                return;
            }

            // Check if the player has gone past the trigger
            if (_player.Movement.TravelAmount > Distance)
            {
                // Make sure this is set to true so the trigger won't trigger again
                _playerHasPassed = true;

                // Trigger the trigger (if we can trigger the trigger)
                if (CanTrigger)
                {
                    OnTriggered();
                    ActivateID();
                }
            }
        }

        protected virtual void OnCollisionEnter(Collision col)
        {
            // Return if this trigger is NOT touch triggered (cuz then we check in Update())
            // Also return if this trigger cant trigger
            if (!isTouchTriggered || !CanTrigger)
            {
                return;
            }

            // Player touched trigger so trigger the trigger
            if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                OnTriggered();
                ActivateID();
            }
        }

        private void ActivateID()
        {
            idHandler.ActivateID(this);
            _isActivated = true;
        }

        /// <summary>
        /// Implement this to determine what happens when this trigger is triggered
        /// </summary>
        protected abstract void OnTriggered();

        /// <summary>
        /// Override this to determine a custom trigger condition that has to be met in order for the player to trigger this trigger. <para/>
        /// So this must return true in order for the trigger to be triggered.
        /// </summary>
        protected virtual bool CustomTriggerCondition => true;

#if UNITY_EDITOR
        // Draw a trigger line in the editor
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2705882f, 0.8784314f, 1, 0.5f);

            Vector3 pos1 = transform.position;
            pos1.y = -100;

            Vector3 pos2 = transform.position;
            pos2.y = 100;

            Gizmos.DrawLine(pos1, pos2);
        }

        /// <summary>
        /// Will draw a gizmo duration line that curves along the current path creator.
        /// </summary>
        protected void DrawDurationLine(float time, int linesPerTile = 3)
        {
            Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 0.5f);
            
            float totalDistance = time * PlayerMovement.NORMAL_SPEED;

            // Get then path creator
            PathCreator pathCreator = FindObjectOfType<PathCreator>();

            if (pathCreator == null)
            {
                return;
            }

            // Get the path from the path creator
            VertexPath path = pathCreator.path;

            // Get the attach to path component if it's null
            if (_attachToPath == null)
            {
                _attachToPath = GetComponent<AttachToPath>();
            }

            // Return if the component is still null
            if (_attachToPath == null)
            {
                return;
            }

            // Draw the lines
            float startDistance = _attachToPath.Distance;

            float addedDistance = 0;

            Vector3 oldPos = transform.position;

            // Calculate amount of lines
            int amount = Mathf.CeilToInt((float)totalDistance * (float)linesPerTile);

            for (int i = 0; i < amount; i++)
            {
                // Get position at the current distance
                Vector3 posAtDistance = path.GetPointAtDistance(startDistance + addedDistance);
                posAtDistance.y = oldPos.y;

                // Draw the line
                Gizmos.DrawLine(oldPos, posAtDistance);

                // Set old pos
                oldPos = posAtDistance;

                // Add distance
                addedDistance += (float)totalDistance / (float)amount;
            }

        }
#endif
    }
}
