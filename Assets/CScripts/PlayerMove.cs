using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName = "Horizontal";
    [SerializeField] private string verticalInputName = "Vertical";

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private bool isRunning;
    private CharacterController charController;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        
    }

    private void Start()
    {
        staminaBar = FindObjectOfType<StaminaBar>();
    }

    private void Update()
    {
        PlayerMovement();
        if (Input.GetKeyDown(KeyCode.Space))
        {

            Debug.Log(this.transform.position);
        }
    }

    private void PlayerMovement()
    {
        float vertInput = Input.GetAxis(verticalInputName) * movementSpeed;     //CharacterController.SimpleMove() applies deltaTime
        float horizInput = Input.GetAxis(horizontalInputName) * movementSpeed;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        if (Input.GetKey(KeyCode.Space)) //スペースキー押しているとき
        {
            if (!isRunning && staminaBar.CanStartRunning()) //スタミナ50以上のとき
            {
                isRunning = true;
                staminaBar.SetRunning(true);
                charController.SimpleMove((forwardMovement + rightMovement) * (runSpeed / movementSpeed)); // ダッシュ時の速度を調整
            }
        }
        else
        {
            isRunning = false;
            staminaBar.SetRunning(false);

        }
        if (isRunning && staminaBar.CanContinueRunning())
        {
            
            charController.SimpleMove((forwardMovement + rightMovement) * (runSpeed / movementSpeed)); // ダッシュ時の速度を調整
        }
        else
        {
            charController.SimpleMove(forwardMovement + rightMovement);
            isRunning = false;
            staminaBar.SetRunning(false);
        }
    
    }
}
