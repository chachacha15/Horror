/*
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
*/

using System.Collections.Generic;
using UnityEngine;

public class PicturePuzzleManager : MonoBehaviour
{
    [Header("絵の情報")]
    public PictureRotator leftPicture;
    public PictureRotator rightPicture;
    /*
    [Header("鍵出現")]
    public GameObject keyPrefab;
    public Transform keySpawnPoint;
    */
    [Header("絵落下 & 音")]
    public Rigidbody fallingPicture;         // 落とす絵の Rigidbody（isKinematic = true で初期化）
    public AudioSource dropSoundSource;      // 割れる音など
    public Transform soundSourcePosition;    // 音が発生した位置（敵が向かう座標）

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
        Debug.Log("間違い。絵をリセット。");
        inputSequence.Clear();
        leftPicture.ResetRotation();
        rightPicture.ResetRotation();
    }

    private void PuzzleClear()
    {
        if (puzzleSolved) return;
        puzzleSolved = true;

        Debug.Log("正解！ギミック開始：絵落下→音→敵を誘導");

        // 鍵を出す
        //Instantiate(keyPrefab,keySpawnPoint.position,Quaternion.Euler(0f, 90f, 0f));  // ← Y軸に90度回転させて出現！);



        // 絵を落とす
        if (fallingPicture != null)
        {
            fallingPicture.isKinematic = false;
        }

        // 落下音を鳴らす
        if (dropSoundSource != null)
        {
            dropSoundSource.Play();
        }

        // ===== 🔥 敵に音の発生を知らせる（これが一番重要） =====
        if (soundSourcePosition != null)
        {
            // radius は detectionRadius に合わせて広めに（20以上推奨）
            SoundEventManager.Emit(soundSourcePosition.position, 25f, "PictureDrop");
            Debug.Log("SoundEvent 発火（位置：" + soundSourcePosition.position + "）");
        }

        if (fallingPicture != null)
        {
            fallingPicture.isKinematic = false;

            // 壁に対して前方向に回転させる（絵の向きによって調整）
            Vector3 forwardTilt = transform.right * -10; // ← transform.forwardでもOK。方向は調整
            fallingPicture.AddTorque(forwardTilt, ForceMode.Impulse);
        }

    }
}
