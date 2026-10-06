using UnityEngine;
using TMPro;  // Brings in the Text Mesh script package into this script
/************************************************************
* COMPONENT OF: Game Manager
* REQUIRED DEPENDENCIES: TextMeshProUGUI
* DESCRIPTION: Handles collectible tracking and win condition 
*              for the game
* AUTHOR: Your name
* VERSION: 1.0
*************************************************************/
public class GameManager : MonoBehaviour
{ 
    // UI text that shows collectibles remaining (assign in inspector)
    [SerializeField] private TextMeshProUGUI RemainingTextUI;

    // Audio source for victory sound
    [SerializeField] private AudioClip winClip;
    private AudioSource audioSource;

    // Tracks how many collectibles are left to collect
    private int numberOfCollectibles;

    // Initialize everything at start
    void Start()
    {
        // Assigns the AudioSource component to the audioSource field        
        audioSource = GetComponent<AudioSource>();
        
        // Determines how many Collectibles are in this scene
        numberOfCollectibles = GameObject.FindGameObjectsWithTag("Collectible").Length;
        
        // Initialize the UI display
        UpdateRemainingUI();
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
            RemainingTextUI.text = "You Win";
		    audioSource.clip = winClip;
            audioSource.Play();
        }

    }
}
