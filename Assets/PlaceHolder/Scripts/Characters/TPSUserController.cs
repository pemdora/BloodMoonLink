using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// TPSController is responsible for main user Input
/// 
/// </summary>
[RequireComponent(typeof(TPSCharacter))]
public class TPSUserController : MonoBehaviour
{
    private TPSCharacter mainCharacter; // A reference to the ThirdPersonCharacter on the object
    // A reference to the main camera in the scenes transform
    // We use main camera reference to adapt player movement to it (player will move foward camera looking direction)
    private Transform mainCamera;
    private Vector3 mainCameraFoward; // The current forward direction of the camera
    private Vector3 mainCharacterMove;  // the world-relative desired move direction, calculated from the camForward and user input.
    private bool isJumping;

    private void Start()
    {
        // get the transform of the main camera
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        mainCharacter = GetComponent<TPSCharacter>();
    }

    /*
    private void Update()
    {
        if (!isJumping)
        {
            isJumping = CrossPlatformInputManager.GetButtonDown("Jump");
        }
    }*/


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");
        //bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
        if (mainCamera != null)
        {
            // calculate camera relative direction to move:
            mainCameraFoward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
            mainCharacterMove = v * mainCameraFoward + h * mainCamera.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            mainCharacterMove = v * Vector3.forward + h * Vector3.right;
        }
#if !MOBILE_INPUT
        // walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift)) mainCharacterMove *= 0.5f;
#endif

        // pass all parameters to the character control script
        mainCharacter.Move(mainCharacterMove,false, isJumping); 
        isJumping = false;
    }
}
