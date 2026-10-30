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
Level 3: 40f
Level 4: 40f, able to fly
Level 5,level 6,level 7: same ability as level 4
* VERSION 1.6: space button allows the player to fly
*************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{

// Data required in Inspector: Set the current level of the player
[SerializeField] private float playerLevel = 1.0f;

// Data required in Inspector: Upward speed velocity applied during flight mode (set to 40f)
[SerializeField] private float flySpeed = 40.0f;

// Cached reference to player Rigidbody component
private Rigidbody playerRigidbody;

private float horizontalMovement;
private float verticalMovement;

private float force;

// Start is called once before the first execution of Update after the MonoBehaviour is created
void Start()
{
// Caches Rigidbody reference with explicit generic type parameter Rigidbody to resolve CS0411 compile error
playerRigidbody = GetComponent<Rigidbody>();

// Sets initial speed force based on starting player level
UpdatePlayerAbilities();
}

// Update is called once per frame
void Update()
{
// Single method call inside Update to keep method clean and preserve architecture rules
ProcessFrameUpdates();
}

// Handles per-frame method invocations
private void ProcessFrameUpdates()
{
// Dynamically checks level changes each frame to adjust force and abilities
UpdatePlayerAbilities();

// Direct check for keyboard Space key press every frame to apply flight at 40f speed when Level 4+
HandleFlightUpdate();
}

// FixedUpdate is called at fixed physics intervals
void FixedUpdate()
{
MovePlayer();
}

// Recalculates movement force and updates movement traits based on player level thresholds
private void UpdatePlayerAbilities()
{
// Level 1: Standard movement force of 10f
if (playerLevel < 2.0f)
{
force = 10f;
}
// Level 2: Increased movement force of 20f
else if (playerLevel < 3.0f)
{
force = 20f;
}
// Level 3: Increased movement force of 40f
else if (playerLevel < 4.0f)
{
force = 40f;
}
// Level 4+: Max movement force of 40f with flight enabling Y-axis movement
else
{
force = 40f;
}
}

// Applies directional ground movement forces to the player's Rigidbody
private void MovePlayer()
{
if (playerRigidbody == null)
{
return;
}

// Calculates horizontal movement force vector on the X/Z plane
Vector3 horizontalForce = new Vector3(horizontalMovement, 0f, verticalMovement) * force;
playerRigidbody.AddForce(horizontalForce);
}

// Evaluates flight state per-frame and applies upward velocity of 40f when Space is held down at Level 4 or higher
private void HandleFlightUpdate()
{
if (playerRigidbody == null)
{
return;
}

// Checks if player level meets flight requirement (Level 4+) and if the Space key is currently held down
if (playerLevel >= 4.0f && Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
{
    // Applies explicit 40f upward Y velocity directly to overcome gravity and lift player
    Vector3 currentVel = playerRigidbody.linearVelocity;
    playerRigidbody.linearVelocity = new Vector3(currentVel.x, flySpeed, currentVel.z);
}
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
// Explicitly specifies generic type parameter Vector2 on ReadValue to resolve CS0411 compile error
SetMoveDirection(ctx.ReadValue<Vector2>());
}

// Listens for space button fly input context if bound via Player Input component
public void OnFlyInput(InputAction.CallbackContext ctx)
{
// Input callback handler maintained for Unity Input System binding compatibility
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
UpdatePlayerAbilities();
}
}