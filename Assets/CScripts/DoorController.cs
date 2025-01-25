using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask layerMask; // レイキャストの対象レイヤー

    public Animator animator; // ドアのAnimator
    public Transform player; // プレイヤーのTransform
    private float interactionDistance = 10f; // ドアとのインタラクション距離

    private bool isOpen = false;
    private bool isLookingAtDoor = false; // クローゼットを見ている状態か

    public GameObject doortext;
    TextMeshProUGUI doorGUI;

    public Image crosshair;   // クロスヘアのImageコンポーネント
    private float currentSize; // 現在のサイズ
    CameraSwitcher cameraSwitcher;

    private bool isLockedDoor = true; // ドアがしまっているか
    private string requiredKeyName; // 必要なカギの名前

    private AudioSource audioSource; // 音を再生するAudioSource
    public AudioClip UnLockSound; // 開錠音
    public AudioClip CardKeySound; // ピッというカードキー認証音
    public AudioClip LockedSound; // ガチャガチャという開けられない音

    public Inventory inventory; // プレイヤーのインベントリ

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        inventory = FindObjectOfType<Inventory>();


        // 自分の親オブジェクトを取得
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // 親の子供の中からCanvasを探す
            Transform canvasTransform = parentTransform.Find("Canvas");

            if (canvasTransform != null)
            {
                // Canvasの子供の中から開閉Textを探す
                Transform textTransform = canvasTransform.Find("開閉Text");

                if (textTransform != null)
                {
                    // Textコンポーネントを取得
                    doortext = textTransform.gameObject;
                }
            }
        }

        // TextMeshProUGUIへの参照
        doorGUI = doortext.GetComponent<TextMeshProUGUI>();
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
        animator = GetComponent<Animator>();

        // AnimatorのisOpenパラメータを初期状態に同期
        if (animator != null)  animator.SetBool("isOpen", isOpen);

        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        // オブジェクト名から数字を抽出して必要なカギを設定
        requiredKeyName = GetRequiredKeyNameFromObjectName(gameObject.name);
        if(requiredKeyName == null) isLockedDoor= false;
    }
    void Update()
    {

        // クローゼットにカーソルがあるかを判定
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // デバッグ用：レイキャストの可視化
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.magenta);

        // プレイヤーが近づいたらClickで開閉
        if (Physics.Raycast(ray, out hit, interactionDistance, layerMask))
        {
            if (hit.collider.gameObject == gameObject) // 現在のドアに一致する場合
            {
                isLookingAtDoor = true;
                if (doortext != null) doortext.SetActive(true); // 開閉Textを非表示

            }
            else
            {
                isLookingAtDoor = false;
                if (doortext != null) doortext.SetActive(false); // 開閉Textを非表示

            }
        }
        else
        {
            isLookingAtDoor = false;
            if (doortext != null) doortext.SetActive(false); // 開閉Textを非表示

            cameraSwitcher.ClosshairAnimation(10f, 35f, 5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);
        }

        cameraSwitcher.ClosshairAnimation(10f, 500f, 0.5f, cameraSwitcher.crosshairRectTransform, isLookingAtDoor);

        // 左クリック時にドアを開閉
        if (Input.GetMouseButtonDown(0) && isLookingAtDoor)
        {
            if (isLockedDoor)
            {
                if (HasRequiredKey())
                {
                    isLockedDoor = false;
                    audioSource.PlayOneShot(CardKeySound);
                    StartCoroutine(PlaySoundWithDelay(UnLockSound, 0.35f));
                }
                else
                {
                    audioSource.PlayOneShot(LockedSound);
                    StartCoroutine(DelayText());
                }
            }
            else
            {
                ToggleDoor();
            }

        }



    }

    public void ToggleDoor()
    {

        isOpen = !isOpen;

        animator.SetBool("isOpen", isOpen);


        if (doorGUI != null) doorGUI.text = isOpen ? "閉める" : "開ける";

    }


    // 指定した音を指定した遅延時間後に再生
    private IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay); // 指定した秒数だけ待つ
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip); // 音を再生
        }
    }

    // オブジェクト名から必要なカギの名前を取得
    private string GetRequiredKeyNameFromObjectName(string objectName)
    {
        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(objectName, @"\d+");
        Debug.Log(objectName);
        Debug.Log(match.Value);
        if (match.Success)
        {
            return $"カードキー({match.Value}号室)"; // 必要なカギの名前を生成
        }
        else
        {
            return null; // 数字がない場合はカギ不要
        }
    }

    // 必要なカギを持っているか確認
    private bool HasRequiredKey()
    {
        foreach (var item in inventory.items)
        {
            if (item.item.name == requiredKeyName)
            {
                return true; // カギを持っている
            }
        }
        return false; // カギがない
    }

    IEnumerator DelayText()
    {
        doorGUI.text = "開かない";
        yield return new WaitForSeconds(1.0f);
        doorGUI.text = "開ける";
    }
}
