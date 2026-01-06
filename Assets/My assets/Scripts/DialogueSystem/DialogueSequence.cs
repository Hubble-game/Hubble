using System.Collections.Generic;
using UnityEngine;

namespace Hubble.DialogueSystem
{
    [CreateAssetMenu(menuName = "Hubble/Dialogue/Monologue Sequence", fileName = "NewDialogueSequence")]
    public class DialogueSequence : ScriptableObject
    {
        [Header("Defaults")]
        [Tooltip("Shown in the name box (optional). Since it's a monologue you can keep it empty, or set to e.g. 'Narrateur'.")]
        public string speakerName;

        [Tooltip("Default typewriter speed in characters per second.")]
        [Min(0f)]
        public float defaultCharactersPerSecond = 40f;

        [Tooltip("Default delay (seconds) between lines if a line delayAfter is <= 0.")]
        [Min(0f)]
        public float defaultDelayBetweenLines = 0f;

        [Header("Lines")]
        public List<DialogueLine> lines = new();
    }
}
