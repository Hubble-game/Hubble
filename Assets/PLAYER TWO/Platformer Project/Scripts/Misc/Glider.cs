using UnityEngine;
using System.Collections;

namespace PLAYERTWO.PlatformerProject
{
	[AddComponentMenu("PLAYER TWO/Platformer Project/Misc/Glider")]
	public class Glider : MonoBehaviour
	{
		public Player player;
		public TrailRenderer[] trails;
		public float scaleDuration = 0.7f;

		[Header("Scale Settings")]
		[Tooltip("If enabled, use the manual overrideScale instead of the transform scale from the editor.")]
		public bool useOverrideScale = false;
		[Tooltip("Manual scale to use if useOverrideScale is true. Leave at (0,0,0) to use the inspector value as-is.")]
		public Vector3 overrideScale = Vector3.one;

		// Captured target scale from the editor (or override)
		protected Vector3 m_targetScale;

		[Header("Audio Settings")]
		public AudioClip openAudio;
		public AudioClip closeAudio;

		protected AudioSource m_audio;

		protected virtual void InitializePlayer()
		{
			if (!player)
				player = GetComponentInParent<Player>();
		}

		protected virtual void InitializeAudio()
		{
			if (!TryGetComponent(out m_audio))
				m_audio = gameObject.AddComponent<AudioSource>();
		}

		protected virtual void InitializeCallbacks()
		{
			player.playerEvents.OnGlidingStart.AddListener(ShowGlider);
			player.playerEvents.OnGlidingStop.AddListener(HideGlider);
		}

		protected virtual void InitializeGlider()
		{
			// Capture the scale set in the editor as the target scale (unless overridden)
			if (useOverrideScale && overrideScale != Vector3.zero)
			{
				m_targetScale = overrideScale;
			}
			else
			{
				m_targetScale = transform.localScale;
			}

			SetTrailsEmitting(false);
			// Start hidden
			transform.localScale = Vector3.zero;
		}

		protected virtual void ShowGlider()
		{
			StopAllCoroutines();
			StartCoroutine(ScaleGliderRoutine(Vector3.zero, m_targetScale));
			SetTrailsEmitting(true);
			if (openAudio != null && m_audio != null)
			{
				m_audio.PlayOneShot(openAudio);
			}
		}

		protected virtual void HideGlider()
		{
			StopAllCoroutines();
			StartCoroutine(ScaleGliderRoutine(m_targetScale, Vector3.zero));
			SetTrailsEmitting(false);
			if (closeAudio != null && m_audio != null)
			{
				m_audio.PlayOneShot(closeAudio);
			}
		}

		protected virtual void SetTrailsEmitting(bool value)
		{
			if (trails == null) return;

			foreach (var trail in trails)
			{
				trail.emitting = value;
			}
		}

		protected IEnumerator ScaleGliderRoutine(Vector3 from, Vector3 to)
		{
			var time = 0f;

			transform.localScale = from;

			while (time < scaleDuration)
			{
				var scale = Vector3.Lerp(from, to, time / scaleDuration);
				transform.localScale = scale;
				time += Time.deltaTime;
				yield return null;
			}

			transform.localScale = to;
		}

		protected virtual void Start()
		{
			InitializePlayer();
			InitializeAudio();
			InitializeCallbacks();
			InitializeGlider();
		}
	}
}
