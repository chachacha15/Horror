using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    // 本が開いているかどうかを管理するフラグ
    public bool isBookOpen = false;

    private Bookreader bookreader;

    private void Start()
    {
        bookreader = FindObjectOfType<Bookreader>();
        if (bookreader == null)
        {
            Debug.LogError("Bookreaderが見つかりません。");
        }
    }

    void Update()
    {
        // 本が開いており、かつマウスの左クリックが押された場合
        if (isBookOpen && Input.GetMouseButtonDown(0))
        {
            // BookreaderのCloseBookメソッドを呼び出す
            bookreader.CloseBook();
        }
    }
}