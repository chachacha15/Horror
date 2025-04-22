using UnityEngine;

public class PictureRotator : MonoBehaviour
{
    [SerializeField] private float rotateAngle = 15f;  // ← ここを15度に変更！
    public string pictureID;                          // "L" または "R" をInspectorで設定

    public delegate void PictureRotated(string pictureID);
    public static event PictureRotated OnPictureRotated;

    private void OnMouseDown()
    {
        transform.Rotate(0f, rotateAngle,0f);
        OnPictureRotated?.Invoke(pictureID); // 回転したことを通知
    }
}


