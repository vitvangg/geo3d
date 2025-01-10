using GD3D.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.Easing;

namespace GD3D.GDCamera
{
    /// <summary>
    /// Controls how the camera should behave and move about during the game
    /// </summary>
    public class CameraBehaviour : MonoBehaviour
    {
        //-- Constants
        public const float FOV_MIN = 1;
        public const float FOV_MAX = 179;

        //-- Instance
        public static CameraBehaviour Instance;

        [Header("Camera Settings")]
        [SerializeField] private Vector3 extraStartOffset = new Vector3(0, -1, 0);

        [SerializeField] private Vector3 offset = new Vector3(6, 3.5f, -10);
        private Vector3 _startOffset;

        [SerializeField] private Vector3 rotation = new Vector3(15, 0, 0);

        [Space]
        public Transform Target;
        private Vector3 _position;
        private Vector3 _targetPosition;

        [Header("Y Limits")]
        [SerializeField] private float limitYMin;
        [SerializeField] private float limitYMax;

        [HideInInspector] public float YLockPos;

        [Header("Interpolation Values")]
        [SerializeField] private float yMaxDelta = .2f;
        [SerializeField] private float yLerpDelta = .3f;
        [SerializeField] private float slerpRotationValue;

        //-- References
        private Camera _cam;

        //-- Shake
        private float _shakeStrength;
        private float _shakeFrequency;
        private float _shakeFrequencyTimer;

        private float _shakeLength;
        private float _shakeLengthTimer;

        private Vector3 _shakeOffset;

        //-- Start values
        private Vector3 _startPos;
        private Quaternion _startRot;
        private float _startFov;

        private Transform _transform;
        private PlayerMain _player;

        //-- Easing IDs (Nullable)
        private long? _currentOffsetEaseId = null;
        private long? _currentRotationEaseId = null;
        private long? _currentFovEaseId = null;

        private void Awake()
        {
            // Set instance
            Instance = this;

            _transform = transform;

            // Set the start values for when we want to reset the values back to their original starting value
            _startOffset = offset;
        }

        private void Start()
        {
            // Get references
            _cam = Helpers.Camera;

            // Set the start pos and tp to that position immediately because otherwise the camera will be weird
            _startPos = (Target == null ? transform.position : Target.position) + offset + extraStartOffset;
            _transform.position = _startPos;
            _position = _startPos;
            _targetPosition = _startPos;

            // Also set start rotation and fov
            _startRot = _transform.rotation;
            _startFov = _cam.fieldOfView;

            // Get player
            _player = PlayerMain.Instance;

            // Subscribe to events
            if (_player != null)
            {
                _player.OnRespawn += (a, b) => StopAllEasings();
                _player.OnRespawn += OnRespawn;
            }

            EasingManager.Instance.OnEaseObjectRemove += OnEaseObjectRemove;
        }

        private void OnRespawn(bool inPracticeMode, Checkpoint checkpoint)
        {
            // Check if we are not in practice mode
            if (!inPracticeMode)
            {
                // Reset position
                _transform.position = _startPos;
                _targetPosition = _startPos;
                _position = _startPos;

                // Reset offset
                offset = _startOffset;

                // Reset rotation
                _transform.rotation = _startRot;
                rotation = _startRot.eulerAngles;

                // Reset fov
                _cam.fieldOfView = _startFov;
            }
            else
            {
                // Set values based on the checkpoint cam state
                CamState state = checkpoint.CamState;

                // Set position
                Vector3 pos = state.Position;

                _transform.position = pos;
                _targetPosition = pos;
                _position = pos;

                // Set offset
                offset = state.Offset;

                // Set rotation
                Quaternion rot = state.Rotation;
                _transform.rotation = rot;
                rotation = rot.eulerAngles;

                // Set shake values
                _shakeStrength = state.ShakeStrength;
                _shakeFrequency = state.ShakeFrequency;
                _shakeFrequencyTimer = state.ShakeFrequencyTimer;
                _shakeLength = state.ShakeLength;
                _shakeLengthTimer = state.ShakeLengthTimer;

                // Set other stuff
                _cam.fieldOfView = state.FOV;
                YLockPos = state.YLockPos;
            }
        }

        #region Saving CamState (For checkpoints)
        /// <summary>
        /// Creates a new <see cref="CamState"/> and inputs this cameras data into it.
        /// </summary>
        /// <returns>The newly created <see cref="CamState"/>.</returns>
        public CamState Save()
        {
            CamState state = new CamState();

            // Transform
            state.Position = _position;
            state.Offset = offset;
            state.Rotation = _transform.rotation;

            // Shake
            state.ShakeStrength = _shakeStrength;
            state.ShakeFrequency = _shakeFrequency;
            state.ShakeFrequencyTimer = _shakeFrequencyTimer;
            state.ShakeLength = _shakeLength;
            state.ShakeLengthTimer = _shakeLengthTimer;

            // Other
            state.FOV = _cam.fieldOfView;
            state.YLockPos = YLockPos;

            return state;
        }

        /// <summary>
        /// A struct that contains data for the camera during a certain state. <para/>
        /// Can be saved and loaded. Mainly used for <see cref="Checkpoint"/>.
        /// </summary>
        public struct CamState
        {
            //-- Transform
            public Vector3 Position;
            public Vector3 Offset;
            public Quaternion Rotation;

            //-- Shake
            public float ShakeStrength;
            public float ShakeFrequency;
            public float ShakeFrequencyTimer;
            public float ShakeLength;
            public float ShakeLengthTimer;

            //-- Other
            public float FOV;
            public float YLockPos;
        }
        #endregion

        private void Update()
        {
            UpdateShake();
        }

        /// <summary>
        /// Called in <see cref="Update"/> every frame. This is just here for cleaner code (hopefully).
        /// </summary>
        private void UpdateShake()
        {
            // Check if the timer is 0 or less
            if (_shakeLengthTimer <= 0)
            {
                _shakeOffset = Vector3.zero;
                return;
            }

            // Decrease timer
            _shakeLengthTimer -= Time.deltaTime;

            // Check if the frequency timer is 0 or less
            if (_shakeFrequencyTimer <= 0)
            {
                // Shake the camera
                float shakePower = Helpers.Map(0, _shakeLength, 0, 1, _shakeLengthTimer);
                _shakeOffset = Random.insideUnitSphere * _shakeStrength * shakePower;

                // Reset timer
                _shakeFrequencyTimer = 1f / _shakeFrequency;
            }
            // If not, decrease the timer
            else
            {
                _shakeFrequencyTimer -= Time.deltaTime;
            }
        }

        private void LateUpdate()
        {
            // Calculate the new position
            if (Target != null)
            {
                _targetPosition.x = Target.position.x;
                _targetPosition.z = Target.position.z;
            }

            _position.x = _targetPosition.x;
            _position.z = _targetPosition.z;

            // Set position
            _transform.position = _position + offset + _shakeOffset;
        }

        private void FixedUpdate()
        {
            if (Target == null)
            {
                return;
            }

            // If the borders are active, lock the Y position to the YLockPos (which is set to be the center between the borders)
            // Otherwise just set it to the target position
            float targetYPos = BorderManager.BordersActive ? YLockPos : Target.position.y;

            // Check if the targetYPos is out of the Y limits (or the borders are active)
            if ((targetYPos > _position.y + limitYMax || targetYPos < _position.y + limitYMin) || BorderManager.BordersActive)
            {
                // If so, move the targetY towards the targetYPos
                float maxDelta = yMaxDelta;

                _targetPosition.y = Mathf.MoveTowards(_targetPosition.y, targetYPos, maxDelta);
            }

            // Lerp towards targetY and targetZ
            _position.y = Mathf.Lerp(_position.y, _targetPosition.y, yLerpDelta);
        }

        #region Easing
        /// <summary>
        /// Called when a <see cref="EaseObject"/> is removed.
        /// </summary>
        private void OnEaseObjectRemove(long id)
        {
            // Set the IDs to null if one of them was removed
            // Sorry for using if else here but I don't see how you can improve this aside from making a whole list system or something like that
            if (_currentOffsetEaseId.HasValue && id == _currentOffsetEaseId.Value)
            {
                _currentOffsetEaseId = null;
            }
            else if (_currentRotationEaseId.HasValue && id == _currentRotationEaseId.Value)
            {
                _currentRotationEaseId = null;
            }
            else if (_currentFovEaseId.HasValue && id == _currentFovEaseId.Value)
            {
                _currentFovEaseId = null;
            }
        }

        /// <summary>
        /// Stops all the current active camera easings
        /// </summary>
        private void StopAllEasings()
        {
            // Remove each ease using try remove
            EasingManager.TryRemoveEaseObject(_currentOffsetEaseId);
            EasingManager.TryRemoveEaseObject(_currentRotationEaseId);
            EasingManager.TryRemoveEaseObject(_currentFovEaseId);
        }

        /// <summary>
        /// Will set the given easing <paramref name="obj"/> to change the offset to the given <paramref name="target"/>. <para/>
        /// Will remove and replace the old easing if one already exists.
        /// </summary>
        public void EaseOffset(Vector3 target, EaseObject obj)
        {
            // Remove the current ease using try remove
            EasingManager.TryRemoveEaseObject(_currentOffsetEaseId);

            // Get the current offset and use it as the start value
            Vector3 startValue = offset;

            // Set the easing on update method to change offset over time
            obj.OnUpdate = (obj) =>
            {
                Vector3 newOffset = obj.EaseVector(startValue, target);

                offset = newOffset;
            };

            // Set the ID
            _currentOffsetEaseId = obj.ID;
        }

        /// <summary>
        /// Will set the given easing <paramref name="obj"/> to change the rotation to the given <paramref name="target"/>. <para/>
        /// Will remove and replace the old easing if one already exists.
        /// </summary>
        public void EaseRotation(Vector3 target, EaseObject obj)
        {
            // Remove the current ease using try remove
            EasingManager.TryRemoveEaseObject(_currentRotationEaseId);

            // Get the current rotation and use it as the start value
            Vector3 startValue = rotation;

            // Set the easing on update method to change rotation over time
            obj.OnUpdate = (obj) =>
            {
                Vector3 newRotation = obj.EaseVector(startValue, target);

                rotation = newRotation;
            };

            // Set the ID
            _currentRotationEaseId = obj.ID;
        }


        /// <summary>
        /// Will set the given easing <paramref name="obj"/> to change the FOV to the given <paramref name="target"/>. <para/>
        /// Will remove and replace the old easing if one already exists. <para/>
        /// <paramref name="target"/> is also limited between the <see cref="FOV_MIN"/> and <see cref="FOV_MAX"/>.
        /// </summary>
        public void EaseFov(float target, EaseObject obj)
        {
            // Clamp the target between the min and max fov
            target = Mathf.Clamp(target, FOV_MIN, FOV_MAX);

            // Remove the current ease using try remove
            EasingManager.TryRemoveEaseObject(_currentFovEaseId);

            // Get the current field of view and use it as the start value
            float startValue = _cam.fieldOfView;

            // Set the easing on update method to change the field of view over time
            obj.OnUpdate = (obj) =>
            {
                _cam.fieldOfView = obj.GetValue(startValue, target);
            };

            // Set the ID
            _currentFovEaseId = obj.ID;
        }
        #endregion

        /// <summary>
        /// Shakes the camera
        /// </summary>
        /// <param name="strength">The strength of the shake</param>
        /// <param name="frequency">How often the camera will shake. This is in shakes per second</param>
        /// <param name="length">How long the shake will last</param>
        public void Shake(float strength, float frequency, float length)
        {
            _shakeStrength = strength;
            _shakeFrequency = frequency;
            _shakeFrequencyTimer = 0;

            _shakeLength = length;
            _shakeLengthTimer = length;
        }

#if UNITY_EDITOR
        // Draw gizmos only in the editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            Vector3 position = transform.position;

            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Gizmos.matrix = matrix;

            position.y += limitYMin;
            Gizmos.DrawLine(position - new Vector3(10, 0, 0), position + new Vector3(10, 0, 0));

            position.y = transform.position.y + limitYMax;
            Gizmos.DrawLine(position - new Vector3(10, 0, 0), position + new Vector3(10, 0, 0));
        }
#endif
    }
}
