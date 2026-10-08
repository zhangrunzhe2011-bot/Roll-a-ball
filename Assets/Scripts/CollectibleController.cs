/****************************************************************************
* COMPONENT OF: Collectible Prefabs
* REQUIRED DEPENDENCIES: Game manager, collider Trigger and particle system components
* DESCRIPTION: THis script calls update reamaining of game manager is triggered by the player
* AUTHOR: Sky Zhang
* VERSION 1.0
* RELEASE NOTES VERSION 1.1: Add a behavior so that the Collectible rotates slowly about the y-axis.
****************************************************************************/

using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectParticlePrefab;
    
    // Call with this object collides with a trigger
    private void OnTriggerEnter(Collider other)
    {
        // Only executes if the collision was with the Player
        if (other.CompareTag("Player"))
        {
            gameManager.UpdateRemaining();
            // Spawn audio at the collectible's position (auto-destroys)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Spawn particles (auto-destroys if Stop Action is set to Destroy)
            Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);

            // Safely destroy the collectible immediately
            Destroy(gameObject);
        }
    }

    
    void Start()
    {
        // Finds the Game Manager in the Scene
gameManager = FindAnyObjectByType<GameManager>();

    }

    void Update()
    {
        
    }
}
