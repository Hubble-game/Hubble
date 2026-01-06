using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hubble.DialogueSystem
{
    /// <summary>
    /// Plays dialogue sequences (monologue) through an existing UI.
    /// - Typewriter effect
    /// - Continue button enabled only when typing finished
    /// - Space shortcut (only works when typing finished)
    /// - Optional delay between lines
    /// - Optional input blocking through IDialogueInputBlocker
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private DialogueUI ui;

        [Tooltip("Optional: camera travelling/lock during dialogue (Cinemachine).")]
        [SerializeField] private DialogueCameraTravel cameraTravel;

        [Tooltip("Optional: Animator driven by dialogue lines (trigger/state per line).")]
        [SerializeField] private Animator dialogueAnimator;

        [Tooltip("Optional. Assign a component that implements IDialogueInputBlocker (player/controller adapter).")]
        [SerializeField] private MonoBehaviour inputBlockerBehaviour;

        [Header("Controls")]
        [Tooltip("Input Action used to continue (keyboard/gamepad). Recommended: a dedicated action bound to Space / A / Cross.")]
        [SerializeField] private InputActionReference continueAction;

        [Tooltip("If true, DialogueManager will enable the Continue action while dialogue is playing and restore it afterwards.")]
        [SerializeField] private bool manageContinueActionEnableState = true;

        [Tooltip("Logs when the Continue action fires (useful to debug why skip/continue doesn't work).")]
        [SerializeField] private bool debugContinue = false;

        [Tooltip("If true, pressing continue key while typing will instantly reveal the full line (common VN behavior).")]
        [SerializeField] private bool allowSkipTypewriter = true;

        private IDialogueInputBlocker _inputBlocker;

        private DialogueSequence _current;
        private int _lineIndex;

        private Coroutine _typeRoutine;
        private bool _isPlaying;
        private bool _isTyping;
        private bool _canContinue;
        private bool _requestedContinue;

    private bool _continueWasEnabledBeforeDialogue;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (inputBlockerBehaviour != null)
            {
                _inputBlocker = inputBlockerBehaviour as IDialogueInputBlocker;
                if (_inputBlocker == null)
                {
                    Debug.LogWarning($"[DialogueManager] Input Blocker Behaviour '{inputBlockerBehaviour.name}' doesn't implement IDialogueInputBlocker. " +
                                     "Assign a DialogueInputBlockerAdapter (or a custom blocker) to enable freezing.");
                }
            }

            // Fallback: if nothing assigned (or assigned component is wrong), try to find one in the scene.
            if (_inputBlocker == null)
            {
                var behaviours = FindObjectsOfType<MonoBehaviour>(true);
                foreach (var b in behaviours)
                {
                    if (b is IDialogueInputBlocker blocker)
                    {
                        _inputBlocker = blocker;
                        break;
                    }
                }
            }

            if (ui != null)
            {
                ui.SetVisible(false);
                ui.SetContinueInteractable(false);
                if (ui.dialogButton != null)
                    ui.dialogButton.onClick.AddListener(RequestContinue);
            }

            if (continueAction != null)
            {
                continueAction.action.performed += OnContinuePerformed;
            }
        }

        private void OnDestroy()
        {
            if (continueAction != null)
            {
                continueAction.action.performed -= OnContinuePerformed;
            }

            if (Instance == this)
                Instance = null;
        }

        private void OnEnable()
        {
            // Don't force-enable here: the action may belong to a shared InputActionAsset
            // used by gameplay. We only listen to it; the owner should manage enabling.
        }

        private void OnDisable()
        {
            // Don't force-disable here for the same reason.
        }

        private void OnContinuePerformed(InputAction.CallbackContext _)
        {
            if (debugContinue)
                Debug.Log("[DialogueManager] Continue performed");

            if (!_isPlaying)
                return;

            if (_isTyping && allowSkipTypewriter)
            {
                // Reveal instantly
                _requestedContinue = true;
            }
            else if (_canContinue)
            {
                RequestContinue();
            }
        }

        public bool IsDialogueActive => _isPlaying;

        public void Play(DialogueSequence sequence)
        {
            if (sequence == null)
            {
                Debug.LogWarning("[DialogueManager] Tried to play a null DialogueSequence.");
                return;
            }

            if (ui == null)
            {
                Debug.LogError("[DialogueManager] DialogueUI reference is missing.");
                return;
            }

            StopCurrent();

            _current = sequence;
            _lineIndex = 0;
            _isPlaying = true;

            ui.SetVisible(true);

            cameraTravel?.Begin();

            var name = sequence.speakerName;
            bool hasName = !string.IsNullOrWhiteSpace(name);
            ui.SetNameVisible(hasName);
            if (hasName)
                ui.SetName(name);

            _inputBlocker?.SetDialogueInputBlocked(true);

            EnsureContinueActionStateForDialogue(true);

            PlayCurrentLine();
        }

        public void StopCurrent()
        {
            if (_typeRoutine != null)
            {
                StopCoroutine(_typeRoutine);
                _typeRoutine = null;
            }

            _isPlaying = false;
            _isTyping = false;
            _canContinue = false;
            _requestedContinue = false;

            if (ui != null)
            {
                ui.SetContinueInteractable(false);
                ui.SetText(string.Empty);
                ui.SetVisible(false);
            }

            cameraTravel?.End();

            _inputBlocker?.SetDialogueInputBlocked(false);

            EnsureContinueActionStateForDialogue(false);

            _current = null;
        }

        private void EnsureContinueActionStateForDialogue(bool enable)
        {
            if (!manageContinueActionEnableState)
                return;

            if (continueAction == null || continueAction.action == null)
                return;

            if (enable)
            {
                _continueWasEnabledBeforeDialogue = continueAction.action.enabled;
                if (!continueAction.action.enabled)
                    continueAction.action.Enable();
            }
            else
            {
                // Restore previous state precisely, so we don't break gameplay actions.
                if (!_continueWasEnabledBeforeDialogue && continueAction.action.enabled)
                    continueAction.action.Disable();
            }
        }

        private void RequestContinue()
        {
            if (!_isPlaying || !_canContinue)
                return;

            _requestedContinue = true;
        }

        private void PlayCurrentLine()
        {
            if (_current == null)
                return;

            if (_current.lines == null || _current.lines.Count == 0)
            {
                StopCurrent();
                return;
            }

            if (_lineIndex >= _current.lines.Count)
            {
                StopCurrent();
                return;
            }

            var line = _current.lines[_lineIndex];
            _requestedContinue = false;

            TryApplyLineAnimation(line);

            if (_typeRoutine != null)
            {
                StopCoroutine(_typeRoutine);
                _typeRoutine = null;
            }

            _typeRoutine = StartCoroutine(TypeLineRoutine(line));
        }

        private void TryApplyLineAnimation(DialogueLine line)
        {
            if (dialogueAnimator == null || line == null)
                return;

            if (!string.IsNullOrWhiteSpace(line.animatorStateName))
                dialogueAnimator.Play(line.animatorStateName, line.animatorLayer, line.animatorNormalizedTime);

            if (!string.IsNullOrWhiteSpace(line.animatorTrigger))
            {
                // Reset then set avoids a "stuck" trigger if you reuse the same trigger multiple lines.
                dialogueAnimator.ResetTrigger(line.animatorTrigger);
                dialogueAnimator.SetTrigger(line.animatorTrigger);
            }
        }

        private IEnumerator TypeLineRoutine(DialogueLine line)
        {
            _isTyping = true;
            _canContinue = false;
            ui.SetContinueInteractable(false);

            string fullText = line?.text ?? string.Empty;
            ui.SetText(string.Empty);

            float cps = (line != null && line.charactersPerSecond > 0f)
                ? line.charactersPerSecond
                : Mathf.Max(1f, _current != null ? _current.defaultCharactersPerSecond : 40f);

            float secondsPerChar = 1f / cps;

            // Typewriter
            for (int i = 0; i < fullText.Length; i++)
            {
                if (_requestedContinue && allowSkipTypewriter)
                {
                    ui.SetText(fullText);
                    break;
                }

                ui.SetText(fullText.Substring(0, i + 1));
                yield return new WaitForSeconds(secondsPerChar);
            }

            _isTyping = false;
            _requestedContinue = false;
            _canContinue = true;
            ui.SetContinueInteractable(true);

            // Wait for user
            while (!_requestedContinue)
                yield return null;

            _requestedContinue = false;
            _canContinue = false;
            ui.SetContinueInteractable(false);

            // Optional delay
            float delay = 0f;
            if (line != null)
                delay = line.delayAfter > 0f ? line.delayAfter : (_current != null ? _current.defaultDelayBetweenLines : 0f);

            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            _lineIndex++;
            PlayCurrentLine();
        }
    }
}
