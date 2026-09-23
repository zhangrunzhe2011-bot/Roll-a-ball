using UnityEngine;

//The script is attached to main camera 
//The purpose of the script is to make camera follow the player 
//Writer: Sky
//Date:sep 23
//Version: 1
public class CameraController : MonoBehaviour
{   
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset;
    // Update is called once per frame
    
    void LateUpdate()
    {
      transform.position = playerTransform.position + offset;  
    }
}
