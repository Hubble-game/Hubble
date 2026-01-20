using UnityEngine;
using TMPro;

namespace PLAYERTWO.PlatformerProject
{
    [AddComponentMenu("PLAYER TWO/Platformer Project/Game/Credits Menu")]
    public class CreditsMenu : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Le panneau qui contient les crédits (active/désactive)")]
        public GameObject creditsPanel;

        [Tooltip("Le composant TextMeshPro (TMP_Text) où seront écrits les noms.")]
        public TMP_Text creditsText;

        [Header("Comportement")]
        [Tooltip("Liste d'objets à désactiver automatiquement quand on affiche les crédits (ex: autres panneaux UI)")]
        public GameObject[] objectsToHide;

        // stocke l'état précédent des objets à masquer pour les restaurer
        private System.Collections.Generic.Dictionary<GameObject, bool> _previousStates;

        [System.Serializable]
        public class CreditSection
        {
            public string title;
            public string[] names;
        }
        [Header("Crédits")]
        [Tooltip("Sections des crédits (titre + noms). Remplissez les rôles et les personnes pour chaque section.")]
        public CreditSection[] sections = new CreditSection[]
        {
            new CreditSection { title = "Développement", names = new string[]{ "Votre nom", "Autre personne" } }
        };

        [Tooltip("Ancienne liste simple (compatibilité). Utilisée si 'sections' est vide)")]
        public string[] contributors = new string[] { "Votre nom", "Autre personne" };

        void Start()
        {
            if (creditsPanel != null)
                creditsPanel.SetActive(false);

            // center the TMP text
            if (creditsText != null)
            {
                creditsText.alignment = TMPro.TextAlignmentOptions.Center;
                creditsText.enableWordWrapping = true;
            }

            RefreshCreditsText();
        }

        // Met à jour le texte des crédits à partir du tableau contributors
        public void RefreshCreditsText()
        {
            if (creditsText == null)
                return;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            // Title
            sb.AppendLine("<size=36><b>CRÉDITS</b></size>");
            sb.AppendLine();

            bool hasSections = sections != null && sections.Length > 0;

            if (!hasSections)
            {
                // fallback to simple contributors list
                if (contributors == null || contributors.Length == 0)
                {
                    creditsText.text = "Aucun contributeur listé.";
                    return;
                }

                sb.AppendLine("<size=24><b>Remerciements</b></size>");
                sb.AppendLine();
                foreach (var c in contributors)
                {
                    if (string.IsNullOrWhiteSpace(c))
                        continue;
                    sb.AppendLine(c);
                }
            }
            else
            {
                // For each section, write the section title and the names
                foreach (var sec in sections)
                {
                    if (sec == null)
                        continue;

                    if (!string.IsNullOrWhiteSpace(sec.title))
                    {
                        sb.AppendLine($"<size=28><b>{sec.title}</b></size>");
                    }

                    if (sec.names != null && sec.names.Length > 0)
                    {
                        foreach (var n in sec.names)
                        {
                            if (string.IsNullOrWhiteSpace(n))
                                continue;
                            sb.AppendLine(n);
                        }
                    }
                    sb.AppendLine();
                }
            }

            creditsText.text = sb.ToString();
        }

        // Appelé par le bouton du menu principal pour ouvrir l'écran crédits
        public void ShowCredits()
        {
            if (creditsPanel == null)
                return;

            RefreshCreditsText();
            // active le panneau des crédits
            creditsPanel.SetActive(true);

            // cache les autres éléments listés
            if (objectsToHide != null && objectsToHide.Length > 0)
            {
                if (_previousStates == null)
                    _previousStates = new System.Collections.Generic.Dictionary<GameObject, bool>();

                foreach (var go in objectsToHide)
                {
                    if (go == null || go == creditsPanel)
                        continue;

                    // enregistre l'état précédent puis désactive
                    if (!_previousStates.ContainsKey(go))
                        _previousStates[go] = go.activeSelf;

                    go.SetActive(false);
                }
            }
        }

        // Appelé par le bouton "Retour" dans l'écran crédits
        public void HideCredits()
        {
            if (creditsPanel == null)
                return;

            // désactive le panneau des crédits
            creditsPanel.SetActive(false);

            // restaure l'état des objets cachés
            if (_previousStates != null)
            {
                foreach (var kv in _previousStates)
                {
                    var go = kv.Key;
                    if (go == null)
                        continue;

                    go.SetActive(kv.Value);
                }

                _previousStates.Clear();
            }
        }

        // Bouton quitter : quitte l'application (et arrête la lecture dans l'éditeur)
        public void QuitGame()
        {
            // NOTE : Application.Quit() ne fait rien dans l'éditeur
            Application.Quit();

#if UNITY_EDITOR
            // Si on est dans l'éditeur, arrête le mode Play
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
