using UnityEngine;

public class PictureRotator : MonoBehaviour, IInteractable
{

    #region Interactable (IInteractable)

    public string GetInteractText()
    {
        return "";
    }

    public bool ShowInteractText => false; // テキスト表示するかどうか
    public bool ActivateCrosshair => true;

    /// <summary>
    /// クリック時、開閉
    /// </summary>
    public void Interact(GameObject targetObject)
    {
        transform.Rotate(0f, rotateAngle, 0f);
        OnPictureRotated?.Invoke(pictureID);

    }

    #endregion


    [SerializeField] private float rotateAngle = 15f;
    public string pictureID; // "L" または "R"

    public delegate void PictureRotated(string pictureID);
    public static event PictureRotated OnPictureRotated;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void OnMouseDown()
    {

    }

    public void ResetRotation()
    {
        transform.rotation = initialRotation;
    }
}


