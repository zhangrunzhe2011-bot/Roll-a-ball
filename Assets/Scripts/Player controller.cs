/************************************************************
* COMPONENT OF: Player
* REQUIRED DEPENDENCIES: Rigidbody Component
* DESCRIPTION: It listens for WASD and Arrow key presses to set 
*              vertical and horizontal directions. It uses those
*              to push the player's Rigidbody in that direction
*              at a preset force.
* AUTHOR: Sky
* VERSION: 1.0
*************************************************************/

using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{ // Movement fields

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    private float horizontalMovement = 0.707f;
    private float verticalMovement = 0.707f;
    private float force = 4.75f;
    }

    // Update is called once per frame
    void Update()
    {
      MovePlayer();
    }
// Moves the player
private void MovePlayer()
{
  Vector3 direction = new Vector3(horizontalMovement, 0, verticalMovement);
  GetComponent<Rigidbody>().AddForce(direction * force);
}

    
}


