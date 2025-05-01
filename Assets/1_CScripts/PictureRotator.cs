using UnityEngine;

public class PictureRotator : MonoBehaviour
{
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
        transform.Rotate(0f, rotateAngle, 0f);
        OnPictureRotated?.Invoke(pictureID);
    }

    public void ResetRotation()
    {
        transform.rotation = initialRotation;
    }
}


