using UnityEngine;
using PLAYERTWO.PlatformerProject;

namespace Hubble.DialogueSystem
{
    /// <summary>
    /// Dialogue blocker tailored for PLAYER TWO Platformer Project.
    /// - Forces player into Idle state
    /// - Zeros velocities so they don't keep running
    /// - Optionally freezes the PlayerCamera orbit
    ///
    /// Put this on the Player root and assign it to DialogueManager.
    /// </summary>
    [AddComponentMenu("Hubble/Dialogue/Player Two Dialogue Blocker")]
    [RequireComponent(typeof(Player))]
    public class PlayerTwoDialogueBlocker : MonoBehaviour, IDialogueInputBlocker
    {
        [Tooltip("Optional. If set, will toggle this camera's freeze flag during dialogue.")]
        public PlayerCamera playerCamera;

        [Tooltip("If true, forces Idle state at dialogue start.")]
        public bool forceIdle = true;

        [Tooltip("If true, zeros out lateral and vertical velocity at dialogue start.")]
        public bool zeroVelocity = true;

        [Tooltip("If true, disables PlayerInputManager component during dialogue.")]
        public bool disableInputs = true;

        private Player _player;
        private PlayerInputManager _inputs;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _inputs = GetComponent<PlayerInputManager>();

            if (playerCamera == null)
                playerCamera = GetComponentInChildren<PlayerCamera>(true);
        }

        public void SetDialogueInputBlocked(bool blocked)
        {
            if (_player == null)
                return;

            if (blocked)
            {
                if (disableInputs && _inputs != null)
                    _inputs.enabled = false;

                if (zeroVelocity)
                {
                    _player.velocity = Vector3.zero;
                    _player.lateralVelocity = Vector3.zero;
                    _player.verticalVelocity = Vector3.zero;
                }

                // Prevent "keep running" effect by forcing idle.
                if (forceIdle)
                {
                    // Safe: Player project already uses this transition.
                    _player.states.Change<IdlePlayerState>();
                }

                if (playerCamera != null)
                    playerCamera.freeze = true;
            }
            else
            {
                if (disableInputs && _inputs != null)
                    _inputs.enabled = true;

                if (playerCamera != null)
                    playerCamera.freeze = false;
            }
        }
    }
}
