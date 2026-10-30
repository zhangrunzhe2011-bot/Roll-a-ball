using UnityEngine;

//The script is attached to main camera 
//The purpose of the script is to make camera follow the player 
//Writer: Sky
//Version: 1
//version 2: Added null reference check to prevent errors when player is destroyed.

using UnityEngine;

public class CameraController : MonoBehaviour
{
// Data required in Inspector: Assign the Player's Transform component here
[SerializeField] private Transform playerTransform;

  // Data required in Inspector: Offset position relative to the player (e.g., X:0, Y:5, Z:-10)
  [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);

   void Update()
   {
   FollowPlayer();
   }

   // Handles camera movement to match player position plus offset
   private void FollowPlayer()
   {
      // Validates if the player object reference still exists before attempting access
      if (playerTransform != null)
      {
      transform.position = playerTransform.position + offset;
      }
    }
}
