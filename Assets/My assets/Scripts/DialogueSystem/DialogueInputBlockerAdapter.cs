using UnityEngine;

namespace Hubble.DialogueSystem
{
    /// <summary>
    /// Simple adapter that can disable a list of behaviours while dialogue is active.
    /// Use this if you don't want to edit your player controller scripts.
    /// </summary>
    [AddComponentMenu("Hubble/Dialogue/Dialogue Input Blocker Adapter")]
    public class DialogueInputBlockerAdapter : MonoBehaviour, IDialogueInputBlocker
    {
        [Tooltip("Behaviours to disable while dialogue is active (player movement, jump, attack, etc.).")]
        public Behaviour[] behavioursToDisable;

    [Header("Hard stop (optional)")]
    [Tooltip("If set, will zero its velocity when dialogue starts (useful to immediately stop movement).")]
    public Rigidbody targetRigidbody;

    [Tooltip("If true, sets Rigidbody.isKinematic while dialogue is active (strong freeze).")]
    public bool setKinematicDuringDialogue = false;

    private bool _wasKinematic;

        [Tooltip("Optional: also unlock cursor while dialogue is active.")]
        public bool unlockCursorDuringDialogue = false;

        private bool _wasCursorVisible;
        private CursorLockMode _wasLockMode;

        public void SetDialogueInputBlocked(bool blocked)
        {
            if (behavioursToDisable != null)
            {
                foreach (var b in behavioursToDisable)
                {
                    if (b == null)
                        continue;
                    b.enabled = !blocked;
                }
            }

            if (targetRigidbody != null)
            {
                if (blocked)
                {
                    targetRigidbody.velocity = Vector3.zero;
                    targetRigidbody.angularVelocity = Vector3.zero;

                    if (setKinematicDuringDialogue)
                    {
                        _wasKinematic = targetRigidbody.isKinematic;
                        targetRigidbody.isKinematic = true;
                    }
                }
                else
                {
                    if (setKinematicDuringDialogue)
                    {
                        targetRigidbody.isKinematic = _wasKinematic;
                    }
                }
            }

            if (!unlockCursorDuringDialogue)
                return;

            if (blocked)
            {
                _wasCursorVisible = Cursor.visible;
                _wasLockMode = Cursor.lockState;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = _wasCursorVisible;
                Cursor.lockState = _wasLockMode;
            }
        }
    }
}
