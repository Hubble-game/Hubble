using UnityEngine;
using UnityEngine.Playables;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/Platformer Project/Game/PlayableDirector End Scene Changer")]
    public class PlayableDirectorEndSceneChanger : MonoBehaviour
    {
        [Tooltip("Le PlayableDirector (Timeline) à écouter. Si vide, cherchera sur le même GameObject.")]
        public PlayableDirector director;

        [Tooltip("Le nom de la scène à charger quand la timeline est terminée.")]
        public string sceneToLoad;

        [Tooltip("Si vrai, utilise GameLoader.instance.Load(sceneToLoad) (recommandé). Si faux, utilise SceneManager.LoadScene(sceneToLoad).")]
        public bool useGameLoader = true;

        void Reset()
        {
            // tente de trouver un PlayableDirector sur le même GameObject par défaut
            if (director == null)
                director = GetComponent<PlayableDirector>();
        }

        void OnEnable()
        {
            if (director == null)
                director = GetComponent<PlayableDirector>();

            if (director != null)
                director.stopped += OnPlayableStopped;
        }

        void OnDisable()
        {
            if (director != null)
                director.stopped -= OnPlayableStopped;
        }

        void OnPlayableStopped(PlayableDirector pd)
        {
            if (string.IsNullOrEmpty(sceneToLoad))
                return;

            if (useGameLoader && GameLoader.instance != null)
            {
                GameLoader.instance.Load(sceneToLoad);
            }
            else
            {
                // fallback direct
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
