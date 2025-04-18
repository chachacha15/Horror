using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using Unity.VisualScripting.Antlr3.Runtime;


[RequireComponent(typeof(Collider))]
public class ValveController : MonoBehaviour, IInteractable
{
    #region Variables

    [Tooltip("回転速度")]
    public float rotationSpeed = 120f; // 回転速度（360度回転にかける時間に基づいて調整）

    private bool isRotated = false; // 回転中かどうかのフラグ
    private float rotationDuration = 3f; // 回転にかける時間

    // 他クラスを取得
    ValveManager valveManager;

    #endregion

    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        if (!isRotated) return "回す";
        return "";
    }

    public bool ShowInteractText => !isRotated; // テキスト表示するかどうか
    public bool ActivateCrosshair => !isRotated;

    /// <summary>
    /// バルブをクリック時、回転させる
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        if (!isRotated)
        {
            StartCoroutine(RotateValve());
        }
    }

    #endregion


    #region Methods

    private void Start()
    {
        valveManager = FindObjectOfType<ValveManager>();
    }

    private void Update()
    {
        
    }


  

    /// <summary>
    /// バルブを回転させるメソッド
    /// </summary>
    /// <returns></returns>
    private IEnumerator RotateValve()
    {
        // 回したことを記録する
        isRotated = true;
        valveManager.ValveCountIncrement();

        // 回転アニメーション
        float elapsedTime = 0f;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, 90f, 300f); 

        while (elapsedTime < rotationDuration)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationAmount);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = targetRotation;

  
    }

    #endregion
}
