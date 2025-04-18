using TMPro;
using UnityEngine;


/// <summary>
/// RayCastを一括で管理するクラス
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private GameObject interactCanvas;

    private Camera currentCamera;

    CameraSwitcher cameraSwitcher;
    private void Start()
    {
        cameraSwitcher = FindObjectOfType<CameraSwitcher>();
    }
    private void Update()
    {
        if (Camera.main != null) currentCamera = Camera.main; // または cameraSwitcher.currentClosetCamera
        else currentCamera = cameraSwitcher.currentClosetCamera;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // マウスの位置（クロスヘアの位置）から少し光線を出して、見ているモノを把握
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            IInteractable target = hit.collider.GetComponent<IInteractable>();
            if (target != null)
            {
                // インタラクトテキストを表示するものは、内容を更新して表示
                if (target.ShowInteractText)
                {
                    interactText.text = target.GetInteractText();
                    interactCanvas.SetActive(true);
                }
                else
                {
                    interactCanvas.SetActive(false); // 表示なし
                }

                // クロスヘアをアニメーションするものか確認して実行
                if (target.ActivateCrosshair)
                {
                   cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);

                }
                else
                {
                    cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);
                }

                // 左クリックでインタラクト、各々の処理を実行
                if (Input.GetMouseButtonDown(0))
                {
                    target.Interact(hit.collider.gameObject);
                }

                return;
            }
            else
            {
                cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);
            }
        }
        else
        {
            cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);

        }

        interactCanvas.SetActive(false);
    }
}

public interface IInteractable
{
    string GetInteractText();        // 「取る」「開ける」など
    bool ShowInteractText { get; }   // テキストを表示するかどうか
    bool ActivateCrosshair { get; }  //クロスヘアをアニメーションするか 
    void Interact(GameObject targetObject);                 // 実行アクション
}

