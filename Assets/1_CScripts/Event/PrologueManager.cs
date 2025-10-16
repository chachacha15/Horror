using UnityEngine;
using UnityStandardAssets.Utility;

public class PrologueManager : MonoBehaviour
{
    private const float SHAKE_SPEED = 0.023f;


    public static PrologueManager Instance;


    [SerializeField] private GameObject elevatorObject; // エレベーター全体オブジェクト
    private Vector3 elevatorOriginalPosition; // エレベーターの初期位置

    // 他クラス
    private ShakeCamera shakeCamera;
    private PlayerMove playerMove;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 他クラスを取得
        playerMove = PlayerMove.Instance;
        shakeCamera = ShakeCamera.Instance;

        elevatorOriginalPosition = elevatorObject.transform.position;

    }

    private void Update()
    {
        ShakeInElevator();
    }

    public void ShakeInElevator()
    {

        elevatorObject.transform.position = elevatorOriginalPosition + Random.insideUnitSphere * SHAKE_SPEED;
    }


}
