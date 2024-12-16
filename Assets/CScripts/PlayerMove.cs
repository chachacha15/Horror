using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
   [SerializeField]
    private GameObject soundOptionPrefab;

    // �C���X�^���X�������T�E���h�I�v�V�����̎Q��
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

        if (Input.GetKey(KeyCode.Space)) //�X�y�[�X�L�[�����Ă���Ƃ�
        {
            if (!isRunning && staminaBar.CanStartRunning() && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) //�����Ă��Ȃ��āA�X�^�~�i50�ȏ�Awasd���͂�����
            {
                isRunning = true;
                staminaBar.SetRunning(true);
                //charController.SimpleMove((forwardMovement + rightMovement) * (runSpeed / movementSpeed)); // �_�b�V�����̑��x�𒲐�
                audiorun.Play();
            }
        }
        else //�����Ă���Ƃ��ɁA�X�y�[�X�L�[�𗣂����瑖��̂���߂�悤�ɁB
        {
            isRunning = false;
            staminaBar.SetRunning(false);
            
        }


        if (isRunning && staminaBar.CanContinueRunning())�@//�����Ă��đ��葱������Ƃ�(�X�^�~�i�O�ɂȂ����瑖��Ȃ��悤�ɁB)
        {
           
            charController.SimpleMove((forwardMovement + rightMovement) * (runSpeed / movementSpeed)); // �_�b�V�����̑��x�𒲐�
            // ���鉹���Đ��i�Đ����łȂ��ꍇ�̂݁j
            if (!audiorun.isPlaying)
            {
                audiorun.Play();
            }

            // ���������~�߂�
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
            // ���������Đ��i�Đ����łȂ��ꍇ�̂݁j
            if (!audiowalk.isPlaying && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            {
                audiowalk.Play();
            }

            // ���鉹���~�߂�
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
