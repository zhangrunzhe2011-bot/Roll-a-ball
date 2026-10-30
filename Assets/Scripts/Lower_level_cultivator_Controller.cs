/****************************************************************************
COMPONENT OF: Collectible Prefabs
REQUIRED DEPENDENCIES: Game manager, collider Trigger and particle system components
DESCRIPTION: This script manages collectible interactions, rotates around its y-axis,
assigns a random level between 1 and 7 on start, and handles level comparison 
when triggered by the player.
AUTHOR: Sky Zhang
VERSION 1.0
VERSION 1.1: Add a behavior so that the Collectible rotates slowly about the y-axis.
VERSION 1.2: adding random level to the collectible(from 1 to 7)
collectible disappear when the player level is higher than the collectible and swallow
(make the player disappear) if the collectible Level is higher than player
VERSION 1.3: provide data related to the collectible levels
VERSION 1.4: Increases player level based on collectible level upon successful collection
VERSION 1.5: Increases player level based on Option C formula: (itemLevel / 3.0) / playerLevel
VERSION 1.6: make the high level collectibles appear rarer than the lower level collectibles, 
possibility:
level 1 Collectible: 50%
Level 2 Collectible: 25%
Level 3 Collectible: 12.5%
Level 4 Collectible: 6.25%
Level 5 Collectible: 3.625%
Level 6 Collectible: 2.625%
Level 7 Collectible: 1%
VERSION 1.7: Patrol back and forth between two points 
and Move toward the player when within a certain distance
VERSION 1.8: Only move toward the player within certain distane,no patrol back and forth
VERSION 1.9: make the high level collectibles appear rarer than the lower level collectibles, 
possibility:
level 1 Collectible: 80%
Level 2 Collectible: 10%
Level 3 Collectible: 5$
Level 4 Collectible: 2.5%
Level 5 Collectible: 1.5%
Level 6 Collectible: 0.9%
Level 7 Collectible: 0.1%
VERSION 2.0: Changing Collectible name into cultivador on UI and separate different level of cultivator into different sripts
Lower_level_cultivator_Controllerscript (original):
Level 1 cultivator 60%
Level 2 Cultivator 40%
Middle_level_Demon_cultivator_Controllerscript:
Level 3 cultivator 50%
Level 4 cultivator 60%
Upper_level_Culticator_Controllerscript
Level 5 Cultivator 50%
Level 6 cultivator 50%
Boss_level_Titan_Controller
Level 7 100%

****************************************************************************/

using UnityEngine;

public class CollectibleController : MonoBehaviour
{
private GameManager gameManager;
[SerializeField] private AudioClip collectSound;
[SerializeField] private GameObject collectParticlePrefab;
[SerializeField] private float rotationSpeed = 50f;

// Data required in Inspector: Current level of this collectible (automatically assigned via weighted random probability at Start)
[SerializeField] private int itemLevel = 1;

// Data required in Inspector: Movement speed for chasing the player (e.g., 3.0)
[SerializeField] private float moveSpeed = 0.01f;

// Data required in Inspector: Distance threshold to detect and chase the player (e.g., 6.0)
[SerializeField] private float detectionRange = 0.005f;

// Reference to player object transform to track position
private Transform playerTransform;

void Start()
{
// Finds the Game Manager in the Scene using explicit generic type GameManager
gameManager = FindAnyObjectByType<GameManager>();

// Assigns a weighted random level between 1 and 7 based on probability percentages when initialized
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
// Locates the player object in scene to enable distance tracking using explicit generic type parameter NewMonoBehaviourScript
NewMonoBehaviourScript player = FindAnyObjectByType<NewMonoBehaviourScript>();
if (player != null)
{
playerTransform = player.transform;
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
// Re-checks for player transform reference if missing using explicit generic type parameter NewMonoBehaviourScript
if (playerTransform == null)
{
NewMonoBehaviourScript player = FindAnyObjectByType<NewMonoBehaviourScript>();
if (player != null)
{
playerTransform = player.transform;
}
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

// Generates a weighted random level integer from 1 to 7 using cumulative percentage thresholds
private void GenerateRandomLevel()
{
// Rolls a random float value between 0.0 and 100.0 representing percentage roll
float randomRoll = Random.Range(0f, 100f);

// Cumulative threshold checks evaluating new VERSION 1.9 probabilities: 80%, 10%, 5%, 2.5%, 1.5%, 0.9%, 0.1%
if (randomRoll < 80.0f)
{
itemLevel = 1; // 80% probability (0.0 to 79.999)
}
else if (randomRoll < 90.0f)
{
itemLevel = 2; // 10% probability (80.0 to 89.999)
}
else if (randomRoll < 95.0f)
{
itemLevel = 3; // 5% probability (90.0 to 94.999)
}
else if (randomRoll < 97.5f)
{
itemLevel = 4; // 2.5% probability (95.0 to 97.499)
}
else if (randomRoll < 99.0f)
{
itemLevel = 5; // 1.5% probability (97.5 to 98.999)
}
else if (randomRoll < 99.9f)
{
itemLevel = 6; // 0.9% probability (99.0 to 99.899)
}
else
{
itemLevel = 7; // 0.1% probability (99.9 to 100.0)
}
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

// Try to retrieve the NewMonoBehaviourScript (Player) component from the player GameObject
NewMonoBehaviourScript player = playerObject.GetComponent<NewMonoBehaviourScript>();

if (player != null)
{
// Send current player level and collectible level to GameManager UI
if (gameManager != null)
{
gameManager.UpdatePlayerLevelUI(player.getPlayerLevel());
gameManager.UpdateCollectibleLevelUI(itemLevel);
}

// If collectible's level is greater than the player's level, destroy player
if (itemLevel > player.getPlayerLevel())
{
    Destroy(playerObject);
}
// If player's level is higher or equal to collectible's level, collect the item and increase player level
else
{
    // Calculates level gain using Option C formula: (Collectible Level / 3) / Player Level
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