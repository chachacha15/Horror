using UnityEngine;

public class EmissionLooper : MonoBehaviour
{
    public Renderer targetRenderer;    // エミッションカラーを操作するレンダラー
    public float loopSpeed = 1f;       // エミッションの変化速度
    public float maxEmissionValue = 0.1f; // エミッションの最大値（インスペクターで調整可能）
    private static CameraSwitcher cameraSwitcher; // 全インスタンスで共有する静的変数
    private Material targetMaterial;   // 操作対象のマテリアル
    private Color BaseColor = new Color(0, 0, 0);

    void Awake()
    {
        // CameraSwitcher が未設定の場合、一度だけ FindObjectOfType を呼び出す
        if (cameraSwitcher == null)
        {
            cameraSwitcher = FindObjectOfType<CameraSwitcher>();
            if (cameraSwitcher == null)
            {
                Debug.LogError("CameraSwitcher が見つかりません！");
            }
        }
    }

    void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("ターゲットのRendererが設定されていません！");
            return;
        }

        // マテリアルの取得
        targetMaterial = targetRenderer.material;

        // エミッションを有効化
        targetMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (targetMaterial == null) return;

        // "デスク (Instance)" の場合、カメラの状態を確認
        if (targetMaterial.name == "デスク (Instance)" && cameraSwitcher != null)
        {
            if (cameraSwitcher.hasHiddenUnderDesk == true)
            {
                targetMaterial.SetColor("_EmissionColor",BaseColor);

                return;
            }
        }

        // エミッション値を 0〜maxEmissionValue の範囲でループ
        float emissionValue = (Mathf.Sin(Time.time * loopSpeed) + 1f) / 2f * maxEmissionValue;

        // エミッションカラーを設定
        Color emissionColor = new Color(emissionValue, emissionValue, emissionValue);

        // マテリアルにエミッションカラーを設定
        targetMaterial.SetColor("_EmissionColor", emissionColor);
    }
}
