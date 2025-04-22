using System.Collections.Generic;
using UnityEngine;

public class PicturePuzzleManager : MonoBehaviour
{
    public GameObject keyPrefab;                // 鍵のPrefab
    public Transform keySpawnPoint;            // 鍵の出現位置

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
        // 入力数オーバーしたらリセット
        if (inputSequence.Count > correctSequence.Length)
        {
            inputSequence.Clear();
            return;
        }

        // 現在までの入力と正解を順次チェック
        for (int i = 0; i < inputSequence.Count; i++)
        {
            if (inputSequence[i] != correctSequence[i])
            {
                inputSequence.Clear();  // 間違えたらリセット
                return;
            }
        }

        // 完全一致したらクリア！
        if (inputSequence.Count == correctSequence.Length)
        {
            PuzzleClear();
        }
    }

    private void PuzzleClear()
    {
        puzzleSolved = true;
        Debug.Log("ギミッククリア！ 鍵が出現！");
        Instantiate(keyPrefab, keySpawnPoint.position, Quaternion.identity);
    }
}
