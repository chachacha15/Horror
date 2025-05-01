using System.Collections.Generic;
using UnityEngine;

public class PicturePuzzleManager : MonoBehaviour
{
    public GameObject keyPrefab;
    public Transform keySpawnPoint;

    public PictureRotator leftPicture;
    public PictureRotator rightPicture;

    private List<string> inputSequence = new List<string>();
    private readonly string[] correctSequence = { "L", "L", "L", "R", "L", "L", "R", "R" };
    private bool puzzleSolved = false;

    private void OnEnable()
    {
        PictureRotator.OnPictureRotated += RecordRotation;
    }

    private void OnDisable()
    {
        PictureRotator.OnPictureRotated -= RecordRotation;
    }

    private void RecordRotation(string pictureID)
    {
        if (puzzleSolved) return;

        inputSequence.Add(pictureID);
        CheckSequence();
    }

    private void CheckSequence()
    {
        // 入力数が超えたらミスとみなす
        if (inputSequence.Count > correctSequence.Length)
        {
            ResetAll();
            return;
        }

        for (int i = 0; i < inputSequence.Count; i++)
        {
            if (inputSequence[i] != correctSequence[i])
            {
                ResetAll();
                return;
            }
        }

        if (inputSequence.Count == correctSequence.Length)
        {
            PuzzleClear();
        }
    }

    private void ResetAll()
    {
        Debug.Log("間違った操作。回転をリセット。");
        inputSequence.Clear();
        leftPicture.ResetRotation();
        rightPicture.ResetRotation();
    }

    private void PuzzleClear()
    {
        puzzleSolved = true;
        Debug.Log("ギミッククリア！ 鍵が出現！");
        Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
    }
}
