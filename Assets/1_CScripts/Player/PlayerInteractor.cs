using TMPro;
using UnityEngine;


/// <summary>
/// RayCastを一括で管理するクラス
/// </summary>
public class PlayerInteractor : MonoBehaviour
{
    public static PlayerInteractor Instance { get; private set; }

    [SerializeField] private float interactDistance = 5f;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private GameObject interactCanvas;

    public bool CanInteract = true;

    private Camera currentCamera;
    private NearbyItemHighlighter nearbyItemHighlighter;


    // 他クラス
    private CameraSwitcher cameraSwitcher;
    private GameStateManager gameStateManager;




    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 他クラスを取得
        cameraSwitcher = CameraSwitcher.Instance;
        gameStateManager = GameStateManager.Instance;
        nearbyItemHighlighter = NearbyItemHighlighter.Instance;

    }
    private void Update()
    {
        // ゲームプレイ中のみ実行
        if (gameStateManager.CurrentGameState == GameState.Playing || gameStateManager.CurrentGameState == GameState.Hiding)
        {

            if (Camera.main != null) currentCamera = Camera.main; // または cameraSwitcher.currentClosetCamera
            else currentCamera = cameraSwitcher.currentClosetCamera;

            if (!CanInteract) return;


            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // マウスの位置（クロスヘアの位置）から少し光線を出して、見ているモノを把握
            if (Physics.Raycast(ray, out hit, interactDistance, obstacleLayer))
            {

                //Debug.Log("見ているモノ　：　"+hit.collider.gameObject);

                if (((1 << hit.collider.gameObject.layer) & interactLayer) != 0)
                {
                    IInteractable target = hit.collider.GetComponent<IInteractable>();
                    if (target != null)
                    {
                        // アイテムを強調
                        if (nearbyItemHighlighter.currentHighlightedItem != hit.collider.gameObject)
                        {
                            nearbyItemHighlighter.ClearHighlight();  // 既存のハイライトを解除
                            nearbyItemHighlighter.ApplyHighlight(hit.collider.gameObject);  // 新しいアイテムを強調
                            Debug.Log("強調されたアイテム: " + hit.collider.gameObject.name);
                        }

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
                            if (cameraSwitcher)
                                cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);

                        }
                        else
                        {
                            if (cameraSwitcher)
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
                        // interactLayerだがIInteractableがない場合
                        if (cameraSwitcher)
                            cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);
                        nearbyItemHighlighter.ClearHighlight();
                    }
                }
                else
                {
                    // 障害物だがinteractLayerではない場合
                    if (cameraSwitcher)
                        cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);
                    nearbyItemHighlighter.ClearHighlight();
                }
            }
            else
            {
                // Raycastが何もヒットしなかった場合
                if (cameraSwitcher)
                    cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);

                nearbyItemHighlighter.ClearHighlight();
            }

            interactCanvas.SetActive(false);
        }
    }

    #endregion

    /// <summary>
    /// インタラクトUIをクリアする（上記のUpdateに上書きされるため注意）
    /// </summary>
    public void ClearInteractUI()
    {
        interactCanvas.SetActive(false);
        cameraSwitcher.ClosshairAnimation(cameraSwitcher.crosshairActiveSize, cameraSwitcher.crosshairNormalSize, cameraSwitcher.crosshairDurarion, cameraSwitcher.crosshairRectTransform);
        nearbyItemHighlighter.ClearHighlight();

    }
}



/// <summary>
/// インタラクト可能なオブジェクトが実装するインターフェース
/// </summary>
public interface IInteractable
{
    string GetInteractText();        // 「取る」「開ける」など
    bool ShowInteractText { get; }   // テキストを表示するかどうか
    bool ActivateCrosshair { get; }  //クロスヘアをアニメーションするか 
    void Interact(GameObject targetObject);          // 実行アクション
}

