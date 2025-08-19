using UnityEngine;

public class GravityTrigger : MonoBehaviour
{
    [Header("Rigidbody à activer")]
    public Rigidbody targetRigidbody;

    [Header("Tag des objets déclencheurs")]
    public string triggerTag = "Player";  // Ne réagit qu'aux objets avec ce tag

    void Start()
    {
        if (targetRigidbody != null)
        {
            // Setup initial : trap ne tombe pas au départ
            targetRigidbody.isKinematic = true;
            targetRigidbody.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ne réagir que si l'objet entrant a le tag défini
        if (other.CompareTag(triggerTag))
        {
            if (targetRigidbody != null)
            {
                targetRigidbody.isKinematic = false;
                targetRigidbody.useGravity = true;
            }
            else
            {
                Debug.LogWarning("Aucun Rigidbody assigné !");
            }
        }
    }
}
