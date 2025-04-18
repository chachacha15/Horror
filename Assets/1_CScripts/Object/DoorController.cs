using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DoorController : MonoBehaviour, IInteractable
{
    #region Variables

    public Camera mainCamera;

    public Animator animator; // ドアのAnimator
    public Transform player; // プレイヤーのTransform

    public bool isOpen = false;
    public TextMeshProUGUI lockedText;

    public bool isLockedDoor = true; // ドアがしまっているか
    private string requiredKeyName; // 必要なカギの名前

    // サウンド
    public AudioClip openSound; // 開ける音
    public AudioClip closeSound; // 閉める音
    private AudioSource audioSource; // 音を再生するAudioSource
    public AudioClip UnLockSound; // 開錠音
    public AudioClip CardKeySound; // ピッというカードキー認証音
    public AudioClip LockedSound; // ガチャガチャという開けられない音

    public Inventory inventory; // プレイヤーのインベントリ


    #endregion


    #region Interactable (IInteractable)

    public string GetInteractText()
    {
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
                StartCoroutine(DelayText());
            }
        }
        else
        {
            ToggleDoor();
        }

    }

    #endregion



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        inventory = FindObjectOfType<Inventory>();


       
       

        // TextMeshProUGUIへの参照
        animator = GetComponent<Animator>();

        // AnimatorのisOpenパラメータを初期状態に同期
        if (animator != null)  animator.SetBool("isOpen", isOpen);

        //MainCameraをタグで動的に取得
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();


        // オブジェクト名から数字を抽出して必要なカギを設定
        requiredKeyName = GetRequiredKeyNameFromObjectName(gameObject.name);
        if(requiredKeyName == null) isLockedDoor= false;

    }

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
        if (inventory.selectedItem != null && inventory.selectedItem.item.name == requiredKeyName)
        {
            return true; // カギを持っている
        }
        
        return false; // カギがない
    }

    IEnumerator DelayText()
    {
        lockedText.text = "開かない";
        yield return new WaitForSeconds(1.0f);
        lockedText.text = "開ける";
    }
}
