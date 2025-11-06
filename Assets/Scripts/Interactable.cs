using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public float interactionDistance = 3f;
    public string interactionKey = "e";
    public string interactionMessage = "Appuyez sur E pour interagir";

    private Transform player;
    private bool isInRange = false;
    public GameObject promptUI;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    void Update()
    {
        Debug.Log("Update actif sur " + gameObject.name); // ← vérifie que ça tourne

        float distance = Vector3.Distance(player.position, transform.position);
        Debug.Log("Distance avec le joueur : " + distance);

        isInRange = distance <= interactionDistance;


        // Active/désactive le composant Outline selon la portée
        var outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = isInRange;
        }

        if (promptUI != null)
        {
            promptUI.SetActive(isInRange);
        }
        else
        {
            Debug.LogWarning("PromptUI n'est pas assigné dans l'inspecteur !");
        }

        if (isInRange && Input.GetKeyDown(interactionKey))
        {
            Debug.Log("Interagit avec " + gameObject.name);
            Interact();
        }
    }

    void Interact()
    {
        Debug.Log("Interaction avec " + gameObject.name);
        // Exemple : entrer dans un vaisseau
        // vaisseauController.EnterShip(player);
    }
}
