using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.SceneManagement; // �V�[���Ǘ����C���|�[�g

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private string horizontalInputName = "Horizontal";
    [SerializeField] private string verticalInputName = "Vertical";

    [SerializeField] private float movementSpeed = 4f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private bool isRunning;

    private AudioSource audiowalk;
    private AudioSource audiorun;
    private CharacterController charController;
    private Camera MainCamera;
    private Transform cameraTransform;
    Vector3 HeadBob;
    [SerializeField] private CurveControlledBob bob = new CurveControlledBob();

    [SerializeField] private GameOverScript gameOverScript; // GameOver�p�̃X�N���v�g�Q��

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

        // �J�����̗h��
        MainCamera = Camera.main;
        cameraTransform = MainCamera.transform;
        bob.Setup(MainCamera, 1.0f);

        // GameOverScript���擾
        gameOverScript = FindObjectOfType<GameOverScript>();
        if (gameOverScript == null)
        {
            Debug.LogError("GameOverScript��������܂���I");
        }
    }

    private void Update()
    {
        PlayerMovement();

        // ��Ԃɉ������h��{����ݒ�
        float bobSpeedMultiplier;

        if (isRunning) // ���s���iSpace�L�[��������Ă���ꍇ�j
        {
            bobSpeedMultiplier = 3.0f; // ���s���̗h��
        }
        else if (Input.GetAxis(verticalInputName) != 0 || Input.GetAxis(horizontalInputName) != 0) // ���s���i�ړ�������ꍇ�j
        {
            bobSpeedMultiplier = 1.8f; // ���s���̗h��
        }
        else // �����~�܂��Ă���ꍇ
        {
            bobSpeedMultiplier = 0.15f; // ������Ƃ����h��
        }
        HeadBob = bob.DoHeadBob(bobSpeedMultiplier);
        cameraTransform.localPosition = HeadBob;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameOverScript?.TriggerGameOver(); // GameOver�𔭓�
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            gameOverScript?.TriggerGameOver(); // GameOver�𔭓�
        }
    }
}
