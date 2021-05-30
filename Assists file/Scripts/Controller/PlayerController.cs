using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("the speed which player Moves")]
    public float moveSpeed = 2f;

    [Tooltip("the look speed to left and right of player  (Calculated in degrees) ")]
    public float lookSpeed = 60f;

    [Tooltip("player jump Power")]
    public float jumpPower = 8f;

     [Tooltip("gravity strength")]
    public float Gravity = 9.81f;
    [Header("Required References")]
     [Tooltip("the player shooter script that fires projectTiles")]
    public Shooter playerShooter;
    public Health playerHealth;
    public List<GameObject> disableWhileDead;
    bool doubleJumpAvaliable = false;

    // character Controller component of the player
    private CharacterController controller;
    private InputManager inputManager;
     Vector3 moveDirection;

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        setUpCharacterController();
        setUpInputManager();
    }

    private void setUpCharacterController()
    {
        controller = GetComponent<CharacterController>();
        if(controller == null){
            Debug.LogError("the player controller script does not have character controller on the same page object");
        }
    }

    private void setUpInputManager()
    {
        inputManager = InputManager.instance;
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {
        //check if player is dead or alive
        if(playerHealth.currentHealth <= 0)
        {
            foreach(GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(false);
            }
            return;
        }
        else
        {
            foreach(GameObject inGameObject in disableWhileDead)
            {
                inGameObject.SetActive(true);
            }
        }
        processMovement();
        processRotation();
    }

   

    void processMovement()
    {
        // get the input from the input mangager
        float leftRightInput = inputManager.horizontalMoveAxis;
        float forwardBackwardInput = inputManager.verticalMoveAxis;
        bool jumpPressed = inputManager.jumpPressed;

        //handle player control while its on the ground
        if(controller.isGrounded)
        {
            doubleJumpAvaliable = true;
            //set the movement direction to be the coming input,set y to 0 while in ground
            moveDirection = new Vector3(leftRightInput,0,forwardBackwardInput);
            //set the move direction in relation to the transform
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection*moveSpeed;
            
            if(jumpPressed)
            {
                moveDirection.y = jumpPower;
            }
        }

        //Player Movement while in the Air
        else
        {
            moveDirection = new Vector3(leftRightInput * moveSpeed, moveDirection.y , forwardBackwardInput * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);

            //To double jump
            if(jumpPressed && doubleJumpAvaliable)
            {
                moveDirection.y = jumpPower;
                doubleJumpAvaliable = false;
            }
        }

        moveDirection.y -= Gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    //functioin to make player be able to move camera left and right 
    void processRotation()
    {
         float horizontalLookInput = inputManager.horizontalLookAxis;
         Vector3 playerRotation  = transform.rotation.eulerAngles;
         transform.rotation = Quaternion.Euler(new Vector3(playerRotation.x,playerRotation.y + horizontalLookInput * lookSpeed * Time.deltaTime,
         playerRotation.z));

    }
}
