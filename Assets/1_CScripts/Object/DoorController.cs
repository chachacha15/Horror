using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DoorController : MonoBehaviour, IInteractable
{
    #region Variables

    public static DoorController Instance;

    public Camera mainCamera;

    public Animator animator; // ドアのAnimator
    public Transform player; // プレイヤーのTransform

    public bool isOpen = false;
    public TextMeshProUGUI lockedText;

    public bool isLockedDoor = true; // ドアがしまっているか
    private string requiredKeyName; // 必要なカギの名前

    public bool isEnemyDoor;        // 敵が潜む場合があるドアかどうか
    public bool isRoomDoor = true;  // 部屋にあるドアかどうか（トイレなどはオフ）

    // 状態を保存する変数
    private bool isCantOpenDisplayed = false; // 「開かない」テキストが表示されているかどうか

    // サウンド
    public AudioClip openSound; // 開ける音
    public AudioClip closeSound; // 閉める音
    private AudioSource audioSource; // 音を再生するAudioSource
    public AudioClip UnLockSound; // 開錠音
    public AudioClip CardKeySound; // ピッというカードキー認証音
    public AudioClip LockedSound; // ガチャガチャという開けられない音


    // 他クラス
    public PlayerMove playerMove;
    public Inventory inventory; // プレイヤーのインベントリ


    public GameObject objectPrefab;


    #endregion


    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        if (isCantOpenDisplayed)
        {
            return "開かない";
        }
        if (!isOpen) return "開ける";
        return "閉める";
    }

    public bool ShowInteractText => true; // テキスト表示するかどうか
    public bool ActivateCrosshair => true;

    /// <summary>
    /// クリック時、開閉
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        if (isLockedDoor)
        {
            if (HasRequiredKey())
            {
                isLockedDoor = false;
                audioSource.pitch = 1;
                audioSource.PlayOneShot(CardKeySound);
                StartCoroutine(PlaySoundWithDelay(UnLockSound, 0.35f));
            }
            else
            {
                audioSource.pitch = 1;
                audioSource.PlayOneShot(LockedSound);
                StartCoroutine(ChangeTextToCantOpen());
            }
        }
        else
        {
            ToggleDoor();
        }

    }

    #endregion



    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        // 必要なコンポーネント・クラスを取得
        playerMove = PlayerMove.Instance;
        inventory = FindObjectOfType<Inventory>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        

        // AnimatorのisOpenパラメータを初期状態に同期
        if (animator != null)  animator.SetBool("isOpen", isOpen);

        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        // オブジェクト名から数字を抽出して必要なカギを設定
        requiredKeyName = GetRequiredKeyNameFromObjectName(gameObject.name);
        if (requiredKeyName == null) isLockedDoor = false;




    }

    /// <summary>
    /// ドアを開閉するメソッド
    /// </summary>
    public void ToggleDoor()
    {

        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);



        if (isOpen)
        {
            audioSource.pitch = 1.3f;
            audioSource.PlayOneShot(openSound);
        }
        else
        {
            audioSource.pitch = 0.85f;
            StartCoroutine(PlaySoundWithDelay(closeSound, 0.35f));
        }



        if (isOpen)
        {
            InstantiateEnemyAtDoorFront();

        }

    }

    /// <summary>
    /// ドアにとっての表側から開けたときに確率で敵を生成する
    /// </summary>
    void InstantiateEnemyAtDoorFront()
    {
        // ドアのローカル空間でプレイヤー位置を取得
        Vector3 localPos = transform.parent.InverseTransformPoint(playerMove.transform.localPosition);

        // 20%の確率で敵を生成する
        int Ra = Random.Range(0, 101);
        if (Ra >= 21) return;

        // ローカルZ座標を使って前後判定（Z+が前、Z-が後ろ）
        if (localPos.z >= 0)
        {
            if (isRoomDoor)
            {
                Debug.Log("表側から開けた");

                // 表側に生成
                Vector3 frontPosition = transform.parent.position - transform.parent.forward * 2.5f;
                Instantiate(objectPrefab, frontPosition, Quaternion.identity);
            }
            else
            {
                Debug.Log("裏側から開けた");
            }
        }
        else
        {

            if (isRoomDoor)
            {
                Debug.Log("裏側から開けた");
            }
            else
            {
                Debug.Log("表側から開けた");

                // 表側に生成
                Vector3 frontPosition = transform.parent.position + transform.parent.forward * 2.5f;
                Instantiate(objectPrefab, frontPosition, Quaternion.identity);
            }

        }
    }


    /// <summary>
    /// 指定した音を指定した遅延時間後に再生
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay); // 指定した秒数だけ待つ
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // 音を再生
        }
    }

    /// <summary>
    /// オブジェクト名から必要なカギの名前を取得
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    private string GetRequiredKeyNameFromObjectName(string objectName)
    {
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(objectName, @"\d+");


        if (match.Success && isRoomDoor)
        {
            return $"カードキー_{match.Value}"; // 必要なカギの名前を生成
        }
        else
        {
            return null; // 数字がない場合はカギ不要
        }
    }

    /// <summary>
    /// 必要なカギを持っているか確認
    /// </summary>
    /// <returns></returns>
    private bool HasRequiredKey()
    {
        if (inventory.selectedItem != null && inventory.selectedItem.item.name == requiredKeyName)
        {
            return true; // カギを持っている
        }
        
        return false; // カギがない
    }

    /// <summary>
    /// 鍵が無いときに一定の間特別なテキストを表示するコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeTextToCantOpen()
    {
        isCantOpenDisplayed = true;
        yield return new WaitForSeconds(1.0f);
        isCantOpenDisplayed = false;
    }
}
