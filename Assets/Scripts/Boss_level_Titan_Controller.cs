/****************************************************************************
COMPONENT OF: Collectible Prefabs (Boss Level Titan)
REQUIRED DEPENDENCIES: Game manager, collider Trigger and particle system components
DESCRIPTION: This script manages Boss Level Titan interactions (Level 7),
rotates around its y-axis, assigns fixed level 7 (100% probability),
and handles level comparison when triggered by the player.
AUTHOR: Sky Zhang
VERSION 1.0: Created dedicated Boss Level Titan script for Level 7 with 100% fixed level assignment and explicit generic type parameters for object lookup
****************************************************************************/

using UnityEngine;

public class Boss_level_Titan_Controllerscript : MonoBehaviour
{
private GameManager gameManager;

// Data required in Inspector: AudioClip played when collected by player
[SerializeField] private AudioClip collectSound;

// Data required in Inspector: Particle system prefab instantiated upon collection
[SerializeField] private GameObject collectParticlePrefab;

// Data required in Inspector: Rotational speed around Y-axis (default 50f)
[SerializeField] private float rotationSpeed = 50f;

// Data required in Inspector: Current level of this cultivator (fixed at Level 7 for Boss Level Titan)
[SerializeField] private int itemLevel = 7;

// Data required in Inspector: Movement speed for chasing the player (default 3.0f)
[SerializeField] private float moveSpeed = 3.0f;

// Data required in Inspector: Distance threshold to detect and chase the player (default 10.0f)
[SerializeField] private float detectionRange = 10.0f;

// Reference to player object transform to track position
private Transform playerTransform;

void Start()
{
// Finds the Game Manager in the Scene using explicit generic type parameter GameManager to resolve CS0411 compile error
gameManager = FindAnyObjectByType<GameManager>();

// Assigns fixed level 7 for the Boss Titan when initialized
GenerateRandomLevel();

// Sets up initial movement references to locate the player transform
InitializeMovement();
}

void Update()
{
// Single method call inside Update to handle object updates and adhere to single responsibility architecture
ProcessFrameUpdates();
}

// Handles per-frame method invocations to keep Update clean
private void ProcessFrameUpdates()
{
RotateCollectible();
HandleMovement();
}

// Locates the player transform in the scene to establish position tracking
private void InitializeMovement()
{
// Locates the player object in scene using component lookup first
NewMonoBehaviourScript player = FindAnyObjectByType<NewMonoBehaviourScript>();
if (player != null)
{
playerTransform = player.transform;
}
else
{
// Fallback lookup using GameObject tag "Player" if script lookup returns null
GameObject playerObj = GameObject.FindWithTag("Player");
if (playerObj != null)
{
playerTransform = playerObj.transform;
}
}
}

// Rotates the collectible continuously around the Y-axis
private void RotateCollectible()
{
transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
}

// Manages proximity detection to chase the player when within range
private void HandleMovement()
{
// Re-checks for player transform reference if missing
if (playerTransform == null)
{
InitializeMovement();
}

// Calculates distance to player if player exists in scene
if (playerTransform != null)
{
float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

// Chases player if within detection range
if (distanceToPlayer <= detectionRange)
{
ChasePlayer();
}
}
}

// Moves the collectible towards the player's current position
private void ChasePlayer()
{
transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
}

// Assigns fixed level 7 (100% probability for Boss Titan)
private void GenerateRandomLevel()
{
itemLevel = 7; // 100% probability
}

// Public getter method to safely retrieve the item's level for external scripts
public int getItemLevel()
{
return itemLevel;
}

// Called when another object enters the trigger collider
private void OnTriggerEnter(Collider other)
{
// Executes logic if the colliding object is tagged as Player
if (other != null && other.CompareTag("Player"))
{
HandlePlayerInteraction(other.gameObject);
}
}

// Handles the core level comparison and outcome between player and collectible
private void HandlePlayerInteraction(GameObject playerObject)
{
if (playerObject == null)
{
return;
}

// Try to retrieve the NewMonoBehaviourScript (Player) component using explicit generic type parameter NewMonoBehaviourScript to resolve CS0411 compile error
NewMonoBehaviourScript player = playerObject.GetComponent<NewMonoBehaviourScript>();
if (player != null)
{
// Send current player level and cultivator level to GameManager UI
if (gameManager != null)
{
gameManager.UpdatePlayerLevelUI(player.getPlayerLevel());
gameManager.UpdateCollectibleLevelUI(itemLevel);
}

// If cultivator's level is greater than the player's level, destroy player
if (itemLevel > player.getPlayerLevel())
{
Destroy(playerObject);
}
// If player's level is higher or equal to cultivator's level, collect the item and increase player level
else
{
// Calculates level gain using Option C formula: (Cultivator Level / 3) / Player Level
float levelGain = CalculateLevelGain(itemLevel, player.getPlayerLevel());

// Increase player's level by adding the calculated level gain
player.IncreaseLevel(levelGain);

// Update UI immediately to display the player's new level
if (gameManager != null)
{
gameManager.UpdatePlayerLevelUI(player.getPlayerLevel());
}

CollectItem();
}
}
}

// Calculates the player level increase based on Option C formula
private float CalculateLevelGain(int collectibleLevel, float currentPlayerLevel)
{
if (currentPlayerLevel <= 0f)
{
return 0f;
}

// Formula: (collectibleLevel / 3.0f) / currentPlayerLevel
return (collectibleLevel / 3.0f) / currentPlayerLevel;
}

// Manages the collection effects, game manager update, and self-destruction
private void CollectItem()
{
if (gameManager != null)
{
gameManager.UpdateRemaining();
}

// Spawn audio at the collectible's position
if (collectSound != null)
{
AudioSource.PlayClipAtPoint(collectSound, transform.position);
}

// Spawn particles
if (collectParticlePrefab != null)
{
Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);
}

// Destroy the collectible object
Destroy(gameObject);
}
}