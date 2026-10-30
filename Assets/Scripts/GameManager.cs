/************************************************************
* COMPONENT OF: Game Manager
* REQUIRED DEPENDENCIES: TextMeshProUGUI
* DESCRIPTION: Handles collectible tracking, win condition, and UI 
*              updates for player level, active collectible level, 
*              and remaining collectibles.
* AUTHOR: Sky Zhang
* VERSION: 1.0
* VERSION 1.2: Receive the Player's level and collectible's level from the Player controller and CollectibleController and demonstrates them on the UI
* VERSION 1.3: Tracks and displays the level of the nearest collectible relative to the player position
* VERSION 1.4: correcting the error that player level text and collectible level text is not showed on the UI
* VERSION 2.5: adding music into the game
*************************************************************/

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
// UI text that shows collectibles remaining (assign in inspector)
[SerializeField] private TextMeshProUGUI RemainingTextUI;

// Data required in Inspector: TextMeshProUGUI component to display the Player's level
[SerializeField] private TextMeshProUGUI playerLevelTextUI;

// Data required in Inspector: TextMeshProUGUI component to display the nearest Collectible/Enemy level
[SerializeField] private TextMeshProUGUI collectibleLevelTextUI;

// Data required in Inspector: AudioClip asset for background game music
[SerializeField] private AudioClip backgroundMusicClip;

// Audio source for victory sound
[SerializeField] private AudioClip winClip;
private AudioSource audioSource;

// Tracks how many collectibles are left to collect
private int numberOfCollectibles;

// Reference to the player GameObject to calculate distance
private GameObject playerObject;

// Initialize everything at start
void Start()
{
// Assigns the AudioSource component using explicit generic type AudioSource
audioSource = GetComponent<AudioSource>();

// Initializes background music clip and playback settings
InitializeAudio();

// Determines how many Collectibles are in this scene
numberOfCollectibles = GameObject.FindGameObjectsWithTag("Collectible").Length;

// Caches the Player GameObject reference
FindPlayerObject();

// Initialize the UI display
UpdateRemainingUI();

// Fetch and display initial player level
InitializePlayerLevelUI();
}

// Called once per frame
void Update()
{
// Executes single method call to update per-frame UI operations and keep Update clean
ProcessFrameUpdates();
}

// Single method called inside Update to handle per-frame logic and keep Update clean
private void ProcessFrameUpdates()
{
UpdatePlayerLevelUIFromPlayer();
UpdateNearestCollectibleLevelUI();
}

// Configures and starts playing background music if an AudioClip is assigned
private void InitializeAudio()
{
if (audioSource != null && backgroundMusicClip != null)
{
audioSource.clip = backgroundMusicClip;
audioSource.loop = true;
audioSource.Play();
}
}

// Caches the player object reference in the scene
private void FindPlayerObject()
{
// Uses explicit generic type parameter NewMonoBehaviourScript to locate player script in scene
NewMonoBehaviourScript player = FindAnyObjectByType<NewMonoBehaviourScript>();
if (player != null)
{
playerObject = player.gameObject;
}
}

// Finds player in scene and updates initial level display
private void InitializePlayerLevelUI()
{
if (playerObject == null)
{
FindPlayerObject();
}

if (playerObject != null)
{
    NewMonoBehaviourScript player = playerObject.GetComponent<NewMonoBehaviourScript>();
    if (player != null)
    {
        UpdatePlayerLevelUI(player.getPlayerLevel());
    }
}
}

// Continuously checks and updates player level display on UI each frame
private void UpdatePlayerLevelUIFromPlayer()
{
if (playerObject == null)
{
FindPlayerObject();
}

if (playerObject != null)
{
    NewMonoBehaviourScript player = playerObject.GetComponent<NewMonoBehaviourScript>();
    if (player != null)
    {
        UpdatePlayerLevelUI(player.getPlayerLevel());
    }
}
}

// Updates UI text - shows player current level formatted to 2 decimal places using float parameter
public void UpdatePlayerLevelUI(float level)
{
if (playerLevelTextUI != null)
{
// Formats float level value to 2 decimal places for clean UI display
playerLevelTextUI.text = "Player Level: " + level.ToString("F2");
}
}

// Updates UI text - shows triggered collectible or enemy level
public void UpdateCollectibleLevelUI(int level)
{
if (collectibleLevelTextUI != null)
{
collectibleLevelTextUI.text = "Nearest Item Level: " + level;
}
}

// Calculates and displays the level of the collectible closest to the player
private void UpdateNearestCollectibleLevelUI()
{
if (playerObject == null)
{
FindPlayerObject();
}

if (playerObject == null || collectibleLevelTextUI == null)
{
    return;
}

GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");

if (collectibles.Length == 0)
{
    collectibleLevelTextUI.text = "Nearest Item Level: N/A";
    return;
}

GameObject nearestCollectible = null;
float shortestDistance = Mathf.Infinity;
Vector3 playerPosition = playerObject.transform.position;

foreach (GameObject collectible in collectibles)
{
    if (collectible != null)
    {
        float distance = Vector3.Distance(playerPosition, collectible.transform.position);
        if (distance < shortestDistance)
        {
            shortestDistance = distance;
            nearestCollectible = collectible;
        }
    }
}

if (nearestCollectible != null)
{
    // Uses explicit generic type parameter CollectibleController to retrieve script reference
    CollectibleController controller = nearestCollectible.GetComponent<CollectibleController>();
    if (controller != null)
    {
        UpdateCollectibleLevelUI(controller.getItemLevel());
    }
}
}

// Called by the CollectibleController when a collectible is picked up - decrements count
public void UpdateRemaining()
{
numberOfCollectibles--;
UpdateRemainingUI(); // Update display immediately when score changes
}

// Updates UI text - shows remaining count message
private void UpdateRemainingUI()
{
if (RemainingTextUI != null && numberOfCollectibles > 0)
{
RemainingTextUI.text = "Collectibles Remaining: " + numberOfCollectibles;
}
else
{
StartCoroutine("EndGame");
}
}

IEnumerator EndGame()
{
RemainingTextUI.text = "You Win";

// Stop background music before playing victory sound effect
if (audioSource != null)
{
    audioSource.Stop();
    audioSource.loop = false;
    audioSource.clip = winClip;
    audioSource.Play();
}

if (audioSource != null && audioSource.clip != null)
{
    yield return new WaitForSeconds(audioSource.clip.length);
}
else
{
    yield return new WaitForSeconds(2.0f);
}

// Calls GetActiveScene() with parentheses to get active Scene object
Scene currentScene = SceneManager.GetActiveScene();
SceneManager.LoadScene(currentScene.name);
}
}