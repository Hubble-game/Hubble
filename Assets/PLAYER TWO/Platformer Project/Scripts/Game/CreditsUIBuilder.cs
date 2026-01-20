using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/Platformer Project/UI/Credits UI Builder")]
    public class CreditsUIBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class Section
        {
            public string title;
            public string[] names;
        }

        [Header("References")]
        public GameObject creditsPanel;
        [Tooltip("Content RectTransform of a ScrollView where sections will be instantiated. If empty, script will try to find a ScrollRect under creditsPanel.")]
        public RectTransform contentParent;

        [Header("Prefabs (optional)")]
        [Tooltip("Prefab for a section container. Should contain a TMP_Text for the title and a VerticalLayoutGroup for children. If empty the builder will create a simple one at runtime.")]
        public GameObject sectionPrefab;
        [Tooltip("Prefab for a single name entry (should contain a TMP_Text). If empty the builder will create one at runtime.")]
        public GameObject nameEntryPrefab;

        [Header("Buttons")]
        public Button backButton;
        public Button quitButton;

        [Header("Layout")]
        public bool centerText = true;
        public float titleSize = 28f;
        public float nameSize = 20f;

        [Header("Credits Data")]
        public Section[] sections = new Section[]
        {
            new Section { title = "Développement", names = new string[]{ "Votre nom", "Autre personne" } }
        };

        void Start()
        {
            if (creditsPanel != null)
                creditsPanel.SetActive(false);

            if (contentParent == null && creditsPanel != null)
            {
                var sr = creditsPanel.GetComponentInChildren<ScrollRect>(true);
                if (sr != null)
                    contentParent = sr.content;
            }

            Build();

            if (backButton != null)
                backButton.onClick.AddListener(HideCredits);

            if (quitButton != null)
                quitButton.onClick.AddListener(QuitGame);
        }

        public void Build()
        {
            if (contentParent == null)
            {
                Debug.LogWarning("CreditsUIBuilder: contentParent is not set and no ScrollRect was found under creditsPanel.");
                return;
            }

            // Clear existing
            for (int i = contentParent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(contentParent.GetChild(i).gameObject);
            }

            // For each section, instantiate a container and fill names
            foreach (var sec in sections)
            {
                if (sec == null)
                    continue;

                GameObject secGO = null;

                if (sectionPrefab != null)
                {
                    secGO = Instantiate(sectionPrefab, contentParent);
                }
                else
                {
                    // create a simple section container with VerticalLayoutGroup
                    secGO = new GameObject(string.IsNullOrEmpty(sec.title) ? "Section" : sec.title, typeof(RectTransform));
                    secGO.transform.SetParent(contentParent, false);
                    var layout = secGO.AddComponent<VerticalLayoutGroup>();
                    layout.childAlignment = TextAnchor.MiddleCenter;
                    layout.spacing = 6f;
                    var fitter = secGO.AddComponent<ContentSizeFitter>();
                    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                    // title
                    if (!string.IsNullOrEmpty(sec.title))
                    {
                        var titleGO = new GameObject("Title", typeof(RectTransform));
                        titleGO.transform.SetParent(secGO.transform, false);
                        var titleText = titleGO.AddComponent<TMP_Text>();
                        titleText.text = sec.title;
                        titleText.fontSize = titleSize;
                        titleText.alignment = centerText ? TextAlignmentOptions.Center : TextAlignmentOptions.Left;
                        titleText.fontStyle = FontStyles.Bold;
                    }
                }

                // Ensure we have a container transform for names
                Transform namesParent = secGO.transform;

                // Add names
                if (sec.names != null)
                {
                    foreach (var n in sec.names)
                    {
                        if (string.IsNullOrWhiteSpace(n))
                            continue;

                        GameObject nameGO = null;

                        if (nameEntryPrefab != null)
                        {
                            nameGO = Instantiate(nameEntryPrefab, namesParent);
                        }
                        else
                        {
                            nameGO = new GameObject("Name", typeof(RectTransform));
                            nameGO.transform.SetParent(namesParent, false);
                            var tmp = nameGO.AddComponent<TMP_Text>();
                            tmp.text = n;
                            tmp.fontSize = nameSize;
                            tmp.alignment = centerText ? TextAlignmentOptions.Center : TextAlignmentOptions.Left;
                        }
                    }
                }
            }

            // Force rebuild layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent);
        }

        public void ShowCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
            }
        }

        public void HideCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
