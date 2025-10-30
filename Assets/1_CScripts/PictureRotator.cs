using Mono.Cecil;
using System;
using UnityEngine;

public class PictureRotator : MonoBehaviour
{
    public ArrowType arrowType;

    [Header("左側の絵かどうか")]
    public bool isLeft = true; // true: 左, false: 右

    private float currentRotation = 0f;

    public static event Action<string> OnPictureRotated;

    // ✅ マウスクリック時に自動で呼ばれる
    private void OnMouseDown()
    {
        Rotate();
    }

    public void Rotate()
    {
        transform.Rotate(0f, 10f, 0f);
        currentRotation += 10f;
        if (currentRotation >= 360f) currentRotation -= 360f;

        OnPictureRotated?.Invoke(isLeft ? "L" : "R");

        Debug.Log($"[{gameObject.name}] を回転！現在角度: {currentRotation}");
    }

    public void ResetRotation()
    {
        transform.localRotation = Quaternion.identity;
        currentRotation = 0f;
    }
}
