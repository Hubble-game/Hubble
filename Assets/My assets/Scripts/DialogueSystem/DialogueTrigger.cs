using UnityEngine;

namespace Hubble.DialogueSystem
{
    /// <summary>
    /// Starts a dialogue sequence when the player enters a trigger.
    /// </summary>
    [AddComponentMenu("Hubble/Dialogue/Dialogue Trigger")]
    [RequireComponent(typeof(Collider))]
    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueSequence sequence;

        [Tooltip("If true, plays automatically when player enters collider.")]
        public bool playOnEnter = true;

        [Tooltip("If true, plays only once.")]
        public bool playOnce = true;

        [Tooltip("Tag of the player collider.")]
        public string playerTag = "Player";

        private bool _played;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!playOnEnter || _played)
                return;

            if (!other.CompareTag(playerTag))
                return;

            Play();
        }

        [ContextMenu("Play")]
        public void Play()
        {
            if (sequence == null)
            {
                Debug.LogWarning("[DialogueTrigger] No DialogueSequence assigned.");
                return;
            }

            if (DialogueManager.Instance == null)
            {
                Debug.LogError("[DialogueTrigger] No DialogueManager in scene.");
                return;
            }

            DialogueManager.Instance.Play(sequence);

            if (playOnce)
                _played = true;
        }
    }
}
