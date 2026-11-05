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
* VERSION 1.5: adding music into the game
* VERSION 1.6: aligning gamemanager with the new cultivator controller scripts, Updated UI text references from Collectible to Cultivator and adapted nearest item checking across all split cultivator scripts
* VERSION 1.7: changing the requirement for winning (beat the boss level titan)

*************************************************************/

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // UI text that shows cultivators remaining (assign in inspector)
    [SerializeField] private TextMeshProUGUI RemainingTextUI;

    // Data required in Inspector: TextMeshProUGUI component to display the Player's level
    [SerializeField] private TextMeshProUGUI playerLevelTextUI;

    // Data required in Inspector: TextMeshProUGUI component to display the nearest Cultivator level
    [SerializeField] private TextMeshProUGUI collectibleLevelTextUI;

    // Data required in Inspector: AudioClip asset for background game music
    [SerializeField] private AudioClip backgroundMusicClip;

    // Audio source for victory sound
    [SerializeField] private AudioClip winClip;
    private AudioSource audioSource;

    // Tracks how many cultivators are left to collect
    private int numberOfCollectibles;

    // Reference to the player GameObject to calculate distance
    private GameObject playerObject;

    // Initialize everything at start
    void Start()
    {
        // Assigns the AudioSource component using explicit generic type parameter AudioSource to resolve CS0411 compile error
        audioSource = GetComponent<AudioSource>();

        // Initializes background music clip and playback settings
        InitializeAudio();

        // Determines how many Cultivators are in this scene tagged Collectible
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
        // First attempts lookup via script component NewMonoBehaviourScript
        NewMonoBehaviourScript playerScript = FindAnyObjectByType<NewMonoBehaviourScript>();
        if (playerScript != null)
        {
            playerObject = playerScript.gameObject;
        }
        else
        {
            // Fallback lookup using Player tag if script lookup returns null
            playerObject = GameObject.FindWithTag("Player");
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
            // Uses explicit generic type parameter NewMonoBehaviourScript on GetComponent to retrieve player level
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
            // Uses explicit generic type parameter NewMonoBehaviourScript on GetComponent to retrieve player level
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

    // Updates UI text - shows triggered cultivator level
    public void UpdateCollectibleLevelUI(int level)
    {
        if (collectibleLevelTextUI != null)
        {
            collectibleLevelTextUI.text = "Nearest Cultivator Level: " + level;
        }
    }

    // Calculates and displays the level of the cultivator closest to the player
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

        // Finds all cultivator objects in the scene tagged Collectible
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");

        if (collectibles.Length == 0)
        {
            collectibleLevelTextUI.text = "Nearest Cultivator Level: N/A";
            return;
        }

        GameObject nearestCollectible = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 playerPosition = playerObject.transform.position;

        // Iterates through every cultivator object to calculate the closest distance to the player
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

        // If a nearest cultivator object is found, dynamically query its level and display it on the UI immediately
        if (nearestCollectible != null)
        {
            // Fetches all MonoBehaviour components attached to the nearest cultivator object using explicit generic parameter MonoBehaviour
            MonoBehaviour[] components = nearestCollectible.GetComponents<MonoBehaviour>();

            // Iterates through each component to dynamically locate the getItemLevel method via C# Reflection
            foreach (MonoBehaviour component in components)
            {
                if (component != null)
                {
                    // Retrieves MethodInfo for getItemLevel to support all cultivator tier controller classes without hardcoded dependencies
                    System.Reflection.MethodInfo method = component.GetType().GetMethod("getItemLevel");
                    if (method != null)
                    {
                        // Dynamically invokes getItemLevel to get the cultivator's pre-assigned level integer
                        int level = (int)method.Invoke(component, null);

                        // Updates UI display with the nearest cultivator's level continuously every frame as the player approaches
                        UpdateCollectibleLevelUI(level);
                        return;
                    }
                }
            }
        }
    }

    // Called when a regular cultivator is picked up - decrements count without triggering game over/win
    public void UpdateRemaining()
    {
        numberOfCollectibles--;
        UpdateRemainingUI();
    }

    // Called specifically by the Boss Level Titan Controller when defeated to trigger the win condition
    public void TriggerBossDefeated()
    {
        numberOfCollectibles--;
        UpdateRemainingUI();
        StartCoroutine("EndGame");
    }

    // Updates UI text - shows remaining cultivator count message
    private void UpdateRemainingUI()
    {
        if (RemainingTextUI != null)
        {
            RemainingTextUI.text = "Cultivators Remaining: " + numberOfCollectibles;
        }
    }

    // Coroutine that displays victory message, plays audio, and restarts scene after completion
    IEnumerator EndGame()
    {
        if (RemainingTextUI != null)
        {
            RemainingTextUI.text = "You Win";
        }

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