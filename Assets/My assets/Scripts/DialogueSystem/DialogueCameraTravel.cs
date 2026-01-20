using System.Collections;
using Cinemachine;
using UnityEngine;

namespace Hubble.DialogueSystem
{
    /// <summary>
    /// Optional camera travelling/lock during dialogue.
    ///
    /// Works by temporarily raising a dedicated CinemachineVirtualCamera priority,
    /// optionally lerping it to a target Transform.
    /// </summary>
    [AddComponentMenu("Hubble/Dialogue/Dialogue Camera Travel")]
    public class DialogueCameraTravel : MonoBehaviour
    {
        [Header("Cinemachine")]
        [Tooltip("A dedicated virtual camera used for dialogue shots.")]
        public CinemachineVirtualCamera dialogueVirtualCamera;

        [Tooltip("Priority while dialogue is active.")]
        public int activePriority = 50;

        [Header("Travel")]
        [Tooltip("Optional: where the dialogue camera should travel to.")]
        public Transform target;

        [Tooltip("Seconds to travel to target.")]
        [Min(0f)]
        public float travelDuration = 0.75f;

        [Tooltip("If true, keeps the camera fixed once arrived (no follow).")]
        public bool lockAfterTravel = true;

        private int _previousPriority;
        private Transform _previousFollow;
        private Transform _previousLookAt;
        private Coroutine _travelRoutine;

        public void Begin()
        {
            if (dialogueVirtualCamera == null)
                return;

            if (_travelRoutine != null)
            {
                StopCoroutine(_travelRoutine);
                _travelRoutine = null;
            }

            _previousPriority = dialogueVirtualCamera.Priority;
            _previousFollow = dialogueVirtualCamera.Follow;
            _previousLookAt = dialogueVirtualCamera.LookAt;

            dialogueVirtualCamera.Priority = activePriority;

            if (target != null)
            {
                _travelRoutine = StartCoroutine(TravelRoutine());
            }
        }

        public void End()
        {
            if (dialogueVirtualCamera == null)
                return;

            if (_travelRoutine != null)
            {
                StopCoroutine(_travelRoutine);
                _travelRoutine = null;
            }

            dialogueVirtualCamera.Priority = _previousPriority;
            dialogueVirtualCamera.Follow = _previousFollow;
            dialogueVirtualCamera.LookAt = _previousLookAt;
        }

        private IEnumerator TravelRoutine()
        {
            var camTransform = dialogueVirtualCamera.transform;
            Vector3 startPos = camTransform.position;
            Quaternion startRot = camTransform.rotation;

            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;

            float t = 0f;
            float duration = Mathf.Max(0.0001f, travelDuration);

            // While travelling, make sure we don't follow anything.
            dialogueVirtualCamera.Follow = null;
            dialogueVirtualCamera.LookAt = null;

            while (t < 1f)
            {
                t += Time.unscaledDeltaTime / duration;
                camTransform.position = Vector3.Lerp(startPos, endPos, t);
                camTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }

            if (lockAfterTravel)
            {
                dialogueVirtualCamera.Follow = null;
                dialogueVirtualCamera.LookAt = null;
            }
        }
    }
}
