/************************************************************
* COMPONENT OF: Player
* REQUIRED DEPENDENCIES: Rigidbody Component
* DESCRIPTION: It listens for WASD and Arrow key presses to set 
*              vertical and horizontal directions. It uses those
*              to push the player's Rigidbody in that direction
*              at a preset force.
* AUTHOR: Sky
* VERSION 1.0: adding the palyer for movement
* VERSION 1.1: when hitting collectibles, if the level is higher than player the player die(disappear)
* VERSION 1.2: provide data related to the player's level
* VERSION 1.3: Player's level increase after it beat a collectible, how much level the player increase depends on the level of the collectible 
* VERSION 1.4: Calculates level increases using float precision with the formula (itemLevel / 3.0f) / playerLevel
* VERSION 1.5: increasing the player's speed while upgrading
Level 1: 10f
Level 2: 20f
*************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{

// Data required in Inspector: Set the current level of the player
[SerializeField] private float playerLevel = 1.0f;

private float horizontalMovement;
private float verticalMovement;
private float force;

// Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
{
    force = 10f;
}

// FixedUpdate is called at fixed physics intervals
void FixedUpdate()
{
    MovePlayer();
}

// Moves the player
private void MovePlayer()
{
    Vector3 direction = new Vector3(horizontalMovement, 0, verticalMovement);
    GetComponent<Rigidbody>().AddForce(direction * force);
}

// Gets the user key input and uses it to assign movement directions
private void SetMoveDirection(Vector2 input)
{
    horizontalMovement = input.x;
    verticalMovement = input.y;
}

// Listens for WASD and arrow key input then calls SetMoveDirection
public void OnMoveInput(InputAction.CallbackContext ctx)
{
    SetMoveDirection(ctx.ReadValue<Vector2>());  
}

// Public getter method to safely share the player's current level with collectibles
public float getPlayerLevel()
{
    return playerLevel;
}

// Public method to safely increase the player's level using float addition
public void IncreaseLevel(float amount)
{
    playerLevel += amount;
}
}