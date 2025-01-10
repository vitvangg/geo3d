using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GD3D.Player
{
    /// <summary>
    /// Handles player's win behavior, including resetting position to the start.
    /// </summary>
    public class PlayerWinTest : PlayerScript
    {
        // Start values
        [HideInInspector] public Vector3 StartPos;
        [HideInInspector] public Vector3 StartScale;
        [HideInInspector] public Quaternion StartRotation;

        // Event handler for win
        public event System.Action OnWin;

        public override void Start()
        {
            base.Start();

            // Store initial position, scale, and rotation
            StartPos = transform.position;
            StartScale = transform.localScale;
            StartRotation = transform.rotation;

            Debug.Log("StartPos: " + StartPos);
            Debug.Log("StartScale: " + StartScale);
            Debug.Log("StartRotation: " + StartRotation);
        }

        /// <summary>
        /// Handles the win event by resetting the player to the start position after a delay.
        /// </summary>
        public void InvokeWinEvent()
        {
            Debug.Log("Player has won!");

            // Trigger win event (for UI or other systems)
            OnWin?.Invoke();

            // Wait 1 second before resetting state
            StartCoroutine(WinResetDelay());
        }

        /// <summary>
        /// Coroutine to handle delay before resetting the player to the start state.
        /// </summary>
        private IEnumerator WinResetDelay()
        {
            Debug.Log("Waiting for reset...");
            yield return new WaitForSeconds(1f); // Delay for 1 second

            ResetToStartState(); // Reset player to start position
        }

        /// <summary>
        /// Resets the player's position, scale, and rotation to the start state.
        /// </summary>
        public void ResetToStartState()
        {
            Debug.Log("Resetting to start state...");
            transform.position = StartPos;
            transform.localScale = StartScale;
            transform.rotation = StartRotation;

            Debug.Log("Position: " + transform.position);
            Debug.Log("Scale: " + transform.localScale);
            Debug.Log("Rotation: " + transform.rotation);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Win"))
            {
                InvokeWinEvent(); // Trigger win logic
            }
        }

    }
}
