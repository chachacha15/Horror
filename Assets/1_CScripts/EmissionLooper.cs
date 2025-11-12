using UnityEngine;

public class EmissionLooper : MonoBehaviour
{
    public static EmissionLooper Instance { get; private set; }

    public Renderer targetRenderer;    // エミッションカラーを操作するレンダラー
    public float loopSpeed = 1f;       // エミッションの変化速度
    public float maxEmissionValue = 0.1f; // エミッションの最大値（インスペクターで調整可能）
    private Material targetMaterial;   // 操作対象のマテリアル
    private Color BaseColor = new Color(0, 0, 0);


    // 他クラス
    private GameStateManager gameStateManager;



    #region Unity Methods


    void Awake()
    {
        Instance = this;
      
    }

    void Start()
    {
        if (targetRenderer == null)
        {
            Debug.LogError("ターゲットのRendererが設定されていません！");
            return;
        }
        gameStateManager = GameStateManager.Instance;

        // マテリアルの取得
        targetMaterial = targetRenderer.material;

        // 机以外のエミッションを有効化
        targetMaterial.EnableKeyword("_EMISSION");
    }


    void Update()
    {     
        ApplyEmissionColor();
    }


    private void OnDisable()
    {
        // 無効化されたときにエミッションをリセット
        if (targetMaterial != null)
        {
            targetMaterial.SetColor("_EmissionColor", BaseColor);
        }
    }


    #endregion



    public void ApplyEmissionColor()
    {
        if (targetMaterial == null) return;

        // エミッション値を 0〜maxEmissionValue の範囲でループ
        float emissionValue = (Mathf.Sin(Time.time * loopSpeed) + 1f) / 2f * maxEmissionValue;

        // エミッションカラーを設定
        Color emissionColor = new Color(emissionValue, emissionValue, emissionValue);

        // マテリアルにエミッションカラーを設定
        targetMaterial.SetColor("_EmissionColor", emissionColor);
    }
}
