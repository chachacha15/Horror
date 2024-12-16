using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
   [SerializeField]
    private GameObject soundOptionPrefab;

    // インスタンス化したサウンドオプションの参照
    private GameObject soundOptionInstance;


    [SerializeField] private string horizontalInputName = "Horizontal";
    [SerializeField] private string verticalInputName = "Vertical";

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private bool isRunning;
    private AudioSource audiowalk;
    private AudioSource audiorun;
    private CharacterController charController;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        
    }

    private void Start()
    {
        staminaBar = FindObjectOfType<StaminaBar>();
        GameObject player = GameObject.Find("Player");

        Transform walk = player.transform.Find("walk sound");
        Transform run = player.transform.Find("run sound");

        audiowalk = walk.GetComponent<AudioSource>();
        audiorun = run.GetComponent<AudioSource>();
      
    }

    private void Update()
    {
    PlayerMovement();
        if (Input.GetKeyDown(KeyCode.Space))
        {

            //Debug.Log(this.transform.position);
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
            if (!isRunning && staminaBar.CanStartRunning() && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) //走っていなくて、スタミナ50以上、wasd入力がある
            {
                isRunning = true;
                staminaBar.SetRunning(true);
                //charController.SimpleMove((forwardMovement + rightMovement) * (runSpeed / movementSpeed)); // ダッシュ時の速度を調整
                audiorun.Play();
            }
        }
        else //走っているときに、スペースキーを離したら走るのをやめるように。
        {
            isRunning = false;
            staminaBar.SetRunning(false);
            
        }


        if (isRunning && staminaBar.CanContinueRunning())　//走っていて走り続けられるとき(スタミナ０になったら走れないように。)
        {
           
            charController.SimpleMove((forwardMovement + rightMovement) * (runSpeed / movementSpeed)); // ダッシュ時の速度を調整
            // 走る音を再生（再生中でない場合のみ）
            if (!audiorun.isPlaying)
            {
                audiorun.Play();
            }

            // 歩く音を止める
            if (audiowalk.isPlaying)
            {
                audiowalk.Stop();
            }
        }
        else
        {
            charController.SimpleMove(forwardMovement + rightMovement);
            isRunning = false;
            staminaBar.SetRunning(false);
            // 歩く音を再生（再生中でない場合のみ）
            if (!audiowalk.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            {
                audiowalk.Play();
            }

            // 走る音を止める
            if (audiorun.isPlaying)
            {
                audiorun.Stop();
            }
        }

        if (audiowalk.isPlaying && !(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            audiowalk.Stop();
        }

    }
}
