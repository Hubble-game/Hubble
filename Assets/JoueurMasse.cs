using UnityEngine;

public class Balancoire : MonoBehaviour
{
    public Rigidbody balancoireRb;
    public Transform joueur;       // Transform du joueur
    public float joueurMasse = 3f; // Masse simulée
    public float gravite = 9.81f;  // Gravité globale
    public Vector3 pivotLocal = Vector3.zero; // Position du pivot local

    void FixedUpdate()
    {
        if (joueur != null)
        {
            // Calculer le bras de levier : distance du joueur au pivot
            Vector3 pivotMonde = transform.TransformPoint(pivotLocal);
            Vector3 brasDeLevier = joueur.position - pivotMonde;

            // On ne s'intéresse qu'à la rotation autour de l'axe Z
            float torque = joueurMasse * gravite * brasDeLevier.y;

            // Appliquer le torque
            balancoireRb.AddTorque(Vector3.forward * torque * Time.fixedDeltaTime, ForceMode.Force);
        }
    }
}
