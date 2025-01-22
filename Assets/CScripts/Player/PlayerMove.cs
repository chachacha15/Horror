using System.Collections;
using UnityEngine;
using UnityStandardAssets.Utility;
using UnityEngine.SceneManagement; // シーン管理をインポート

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

    [SerializeField] private GameOverScript gameOverScript; // GameOver用のスクリプト参照

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

        // カメラの揺れ
        MainCamera = Camera.main;
        cameraTransform = MainCamera.transform;
        bob.Setup(MainCamera, 1.0f);

        // GameOverScriptを取得
        gameOverScript = FindObjectOfType<GameOverScript>();
        if (gameOverScript == null)
        {
            Debug.LogError("GameOverScriptが見つかりません！");
        }
    }

    private void Update()
    {
        PlayerMovement();

        // 状態に応じた揺れ倍率を設定
        float bobSpeedMultiplier;

        if (isRunning) // 走行中（Spaceキーが押されている場合）
        {
            bobSpeedMultiplier = 3.0f; // 走行中の揺れ
        }
        else if (Input.GetAxis(verticalInputName) != 0 || Input.GetAxis(horizontalInputName) != 0) // 歩行中（移動がある場合）
        {
            bobSpeedMultiplier = 1.8f; // 歩行中の揺れ
        }
        else // 立ち止まっている場合
        {
            bobSpeedMultiplier = 0.15f; // ちょっとだけ揺れ
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameOverScript?.TriggerGameOver(); // GameOverを発動
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            gameOverScript?.TriggerGameOver(); // GameOverを発動
        }
    }
}
