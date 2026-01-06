using System;
using UnityEngine;

namespace Hubble.DialogueSystem
{
    [Serializable]
    public class DialogueLine
    {
        [TextArea(2, 6)]
        public string text;

        [Tooltip("Delay (seconds) AFTER this line finishes (before next line).")]
        [Min(0f)]
        public float delayAfter = 0f;

        [Tooltip("Characters per second for the typewriter effect. If <= 0, uses the DialogueSequence default.")]
        public float charactersPerSecond = -1f;

        [Header("Animation (optional)")]
        [Tooltip("If set, DialogueManager will call Animator.SetTrigger with this value at the start of this line.")]
        public string animatorTrigger;

        [Tooltip("If set, DialogueManager will call Animator.Play(stateName, layer, normalizedTime) at the start of this line.")]
        public string animatorStateName;

        [Tooltip("Animator layer for Animator.Play. Defaults to 0.")]
        public int animatorLayer = 0;

        [Tooltip("Normalized time for Animator.Play. Defaults to 0.")]
        [Range(0f, 1f)]
        public float animatorNormalizedTime = 0f;
    }
}
