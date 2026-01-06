using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Hubble.DialogueSystem
{
    /// <summary>
    /// References to your existing Dialogue UI elements.
    /// Attach this component to the root of your DialogueBox UI.
    /// </summary>
    public class DialogueUI : MonoBehaviour
    {
        [Header("Root")]
        public GameObject dialogueBox;

        [Header("Name")]
        public GameObject dialogNameBox;

        public TMP_Text dialogName;
        public TMP_Text dialogText;

        [Header("Continue")]
        public Button dialogButton;

        public void SetVisible(bool visible)
        {
            if (dialogueBox != null)
                dialogueBox.SetActive(visible);
            else
                gameObject.SetActive(visible);
        }

        public void SetNameVisible(bool visible)
        {
            if (dialogNameBox != null)
                dialogNameBox.SetActive(visible);
        }

        public void SetName(string name)
        {
            if (dialogName != null)
                dialogName.text = name;
        }

        public void SetText(string text)
        {
            if (dialogText != null)
                dialogText.text = text;
        }

        public void SetContinueInteractable(bool interactable)
        {
            if (dialogButton != null)
            {
                dialogButton.interactable = interactable;
            }
        }
    }
}
