using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// Volume d'eau qui détecte quand le joueur entre et sort.
    /// À placer sur un GameObject avec un Collider en mode Trigger.
    /// </summary>
    [AddComponentMenu("Quest System/Water Volume Detector")]
    [RequireComponent(typeof(Collider))]
    public class WaterVolumeDetector : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Tag du joueur")]
        public string playerTag = "Player";

        [Tooltip("Couleur du gizmo pour visualiser la zone")]
        public Color gizmoColor = new Color(0f, 0.5f, 1f, 0.3f);

        [Header("État")]
        [Tooltip("Le joueur est-il dans l'eau?")]
        [SerializeField]
        private bool playerInWater = false;

        private void Start()
        {
            // Vérifier que le collider est en mode trigger
            Collider col = GetComponent<Collider>();
            if (col != null && !col.isTrigger)
            {
                Debug.LogWarning("[WaterVolumeDetector] Le Collider devrait être en mode Trigger!");
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (!playerInWater)
                {
                    playerInWater = true;
                    GameEvents.RaisePlayerEnterWater();
                    Debug.Log("[WaterVolumeDetector] Joueur entré dans l'eau");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                if (playerInWater)
                {
                    playerInWater = false;
                    GameEvents.RaisePlayerExitWater();
                    Debug.Log("[WaterVolumeDetector] Joueur sorti de l'eau");
                }
            }
        }

        /// <summary>
        /// Dessine le volume d'eau dans l'éditeur.
        /// </summary>
        private void OnDrawGizmos()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                Gizmos.color = gizmoColor;
                
                if (col is BoxCollider boxCol)
                {
                    Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawCube(boxCol.center, boxCol.size);
                }
                else if (col is SphereCollider sphereCol)
                {
                    Gizmos.DrawSphere(transform.position + sphereCol.center, sphereCol.radius);
                }
            }
        }

        /// <summary>
        /// Vérifie si le joueur est dans l'eau.
        /// </summary>
        public bool IsPlayerInWater => playerInWater;
    }
}
