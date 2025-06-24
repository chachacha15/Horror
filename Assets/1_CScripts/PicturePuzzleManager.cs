/*
using System.Collections.Generic;
using UnityEngine;

public enum ArrowType { Left, Right }

public class PicturePuzzleManager : MonoBehaviour
{
    [Header("操作する2枚の絵")]
    public PictureRotator leftPicture;
    public PictureRotator rightPicture;

    [Header("ヒント生成元")]
    public ArrowHintSpawner hintSpawner;

    [Header("落下対象")]
    public Rigidbody fallingPicture; // 右の絵画（倒れる対象）

    private List<string> correctSequence = new List<string>();
    private List<string> currentInputSequence = new List<string>();
    private bool puzzleSolved = false;

    void Start()
    {
        correctSequence = new List<string>(hintSpawner.generatedAnswerSequence);
        if (fallingPicture != null) fallingPicture.isKinematic = true; // 初期状態で固定
    }

    private void OnEnable()
    {
        PictureRotator.OnPictureRotated += OnPictureRotated;
    }

    private void OnDisable()
    {
        PictureRotator.OnPictureRotated -= OnPictureRotated;
    }

    private void OnPictureRotated(string arrow)
    {
        if (puzzleSolved) return;

        currentInputSequence.Add(arrow);
        Debug.Log("入力: " + string.Join(" ", currentInputSequence));

        if (currentInputSequence.Count > correctSequence.Count)
        {
            Debug.Log("❌ 入力数超過。回転リセット");
            ResetAllPictures();
            return;
        }

        for (int i = 0; i < currentInputSequence.Count; i++)
        {
            if (currentInputSequence[i] != correctSequence[i])
            {
                Debug.Log("❌ 間違い！回転リセット");
                ResetAllPictures();
                return;
            }
        }

        if (currentInputSequence.Count == correctSequence.Count)
        {
            Debug.Log("\uD83C\uDF1F 正解！Puzzle Clear！");
            PuzzleClear();
        }
    }

    private void ResetAllPictures()
    {
        leftPicture.ResetRotation();
        rightPicture.ResetRotation();
        currentInputSequence.Clear();
    }

    private void PuzzleClear()
    {
        puzzleSolved = true;

        if (fallingPicture != null)
        {
            fallingPicture.isKinematic = false; // 落下させる
        }

        Debug.Log("絵が落下！");
    }
}*/

using System.Collections.Generic;
using UnityEngine;

public enum ArrowType { Left, Right }

public class PicturePuzzleManager : MonoBehaviour
{
    [Header("操作する2枚の絵")]
    public PictureRotator leftPicture;
    public PictureRotator rightPicture;

    [Header("ヒント生成元")]
    public ArrowHintSpawner hintSpawner;

    [Header("落下対象")]
    public Rigidbody fallingPicture; // 右の絵画（倒れる対象）

    [Header("パズル失敗設定")]
    public int maxWrongAttempts = 5; // 許容される最大間違い回数
    private int currentWrongAttempts = 0; // 現在の間違い回数

    // 他のスクリプトから呼び出すイベント（火災報知器を鳴らす、敵に通知するなど）
    public static event System.Action OnPuzzleFailedTooManyTimes;

    private List<string> correctSequence = new List<string>();
    private List<string> currentInputSequence = new List<string>();
    private bool puzzleSolved = false;

    void Start()
    {
        correctSequence = new List<string>(hintSpawner.generatedAnswerSequence);

        if (fallingPicture != null)
        {
            fallingPicture.isKinematic = true; // 初期状態で固定
        }
    }

    private void OnEnable()
    {
        PictureRotator.OnPictureRotated += OnPictureRotated;
    }

    private void OnDisable()
    {
        PictureRotator.OnPictureRotated -= OnPictureRotated;
    }

    private void OnPictureRotated(string arrow)
    {
        if (puzzleSolved) return;

        currentInputSequence.Add(arrow);
        Debug.Log("入力: " + string.Join(" ", currentInputSequence));

        if (currentInputSequence.Count > correctSequence.Count)
        {
            Debug.Log("❌ 入力数超過。回転リセット");
            HandleWrongAttempt();
            return;
        }

        for (int i = 0; i < currentInputSequence.Count; i++)
        {
            if (currentInputSequence[i] != correctSequence[i])
            {
                Debug.Log("❌ 間違い！回転リセット");
                HandleWrongAttempt();
                return;
            }
        }

        if (currentInputSequence.Count == correctSequence.Count)
        {
            Debug.Log("\\uD83C\\uDF1F 正解！Puzzle Clear！");
            PuzzleClear();
        }
    }

    private void HandleWrongAttempt()
    {
        currentWrongAttempts++;
        Debug.Log($"間違い回数: {currentWrongAttempts} / {maxWrongAttempts}");

        if (currentWrongAttempts >= maxWrongAttempts)
        {
            Debug.Log("<color=red>!!!!! 火災報知器が鳴り響く！敵が近づいてくる... !!!!!</color>");
            if (OnPuzzleFailedTooManyTimes != null)
            {
                OnPuzzleFailedTooManyTimes.Invoke(); // イベントを発火
            }
            currentWrongAttempts = 0; // ★ 間違い回数をリセット
        }

        ResetAllPictures();
    }

    private void ResetAllPictures()
    {
        leftPicture.ResetRotation();
        rightPicture.ResetRotation();
        currentInputSequence.Clear();
    }

    private void PuzzleClear()
    {
        puzzleSolved = true;
        currentWrongAttempts = 0; // パズルクリアで間違い回数をリセット

        if (fallingPicture != null)
        {
            fallingPicture.isKinematic = false;
        }

        Debug.Log("絵が落下！");
    }
}