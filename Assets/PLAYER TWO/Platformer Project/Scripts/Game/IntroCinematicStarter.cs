using UnityEngine;
using UnityEngine.Events;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/Platformer Project/Game/Intro Cinematic Starter")]
    public class IntroCinematicStarter : MonoBehaviour
    {
        [Tooltip("Event invoked when the scene load has finished. Assign here the Play method of your Timeline/Video or any method that starts the cinematic.")]
        public UnityEvent onStartCinematic;

        void Start()
        {
            // Subscribe to GameLoader.OnLoadFinish so the cinematic starts only after loading finishes
            if (GameLoader.instance != null)
            {
                GameLoader.instance.OnLoadFinish.AddListener(HandleLoadFinished);
            }
            else
            {
                // Fallback: if no GameLoader present, start cinematic shortly after Start
                Invoke(nameof(HandleLoadFinished), 0.1f);
            }
        }

        void OnDestroy()
        {
            if (GameLoader.instance != null)
                GameLoader.instance.OnLoadFinish.RemoveListener(HandleLoadFinished);
        }

        void HandleLoadFinished()
        {
            onStartCinematic?.Invoke();
        }
    }
}
