using UnityEngine;

public class Switcher : MonoBehaviour
{
    public GameObject[] paintings;
    private int currentPaintingIndex;

    public void ShowPainting(int index)
    {
        for (int i = 0; i < paintings.Length; i++) paintings[i].SetActive(i == index);
        currentPaintingIndex = index;
    }

    public void NextPainting()
    {
        currentPaintingIndex = (currentPaintingIndex + 1) % paintings.Length;
        ShowPainting(currentPaintingIndex);
    }

    public void PreviousPainting()
    {
        currentPaintingIndex = (currentPaintingIndex - 1 + paintings.Length) % paintings.Length;
        ShowPainting(currentPaintingIndex);
    }
}