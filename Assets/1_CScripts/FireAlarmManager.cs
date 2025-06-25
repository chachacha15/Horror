using UnityEngine;
using System.Collections; // Coroutineのために必要

public class FireAlarmManager : MonoBehaviour
{
    public AudioClip alarmSound;
    private AudioSource audioSource;
    public GameObject fireAlarmObject; // 火災報知器のモデルなど（任意）
    public float alarmDuration = 60f; // アラームが鳴る時間（秒）

    // 敵にプレイヤーの位置を通知するための参照（必要であれば）
    // private EnemyManager enemyManager; // GhostAIから直接呼び出す場合は不要

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = alarmSound;
        audioSource.loop = true; // 鳴り続けるように設定
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        PicturePuzzleManager.OnPuzzleFailedTooManyTimes += ActivateAlarmAndNotifyEnemies;
    }

    void OnDisable()
    {
        PicturePuzzleManager.OnPuzzleFailedTooManyTimes -= ActivateAlarmAndNotifyEnemies;
    }

    private void ActivateAlarmAndNotifyEnemies()
    {
        // 既にアラームが鳴っている場合は重複して開始しない
        if (audioSource.isPlaying) return;

        Debug.Log("火災報知器が作動しました！");

        if (fireAlarmObject != null)
        {
            fireAlarmObject.SetActive(true); // 火災報知器の視覚効果などを有効にする
            // ここで火災報知器の点滅エフェクトなどを開始しても良いでしょう
        }

        audioSource.Play();
        StartCoroutine(StopAlarmAfterDuration(alarmDuration));

        // 範囲内の敵が感知を発火
        SoundEventManager.Emit(fireAlarmObject.transform.position, 100f, "Alarm");

        // ここで敵に通知する（今回はGhostAIに直接通知する方式をGhostAI側で実装します）
        // もしEnemyManager経由で通知したい場合は、ここでEnemyManagerのメソッドを呼び出す
        // if (enemyManager == null) enemyManager = FindObjectOfType<EnemyManager>();
        // if (enemyManager != null)
        // {
        //     enemyManager.OnAlarmActivated(transform.position); // アラームの発生源を敵に伝える
        // }
    }

    private IEnumerator StopAlarmAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (fireAlarmObject != null)
        {
            fireAlarmObject.SetActive(false); // 火災報知器の視覚効果を無効にする
        }
        Debug.Log("火災報知器が鳴り止みました。");
    }

    // 他の場所からアラームを停止させたい場合のためのパブリックメソッド
    public void ForceStopAlarm()
    {
        StopAllCoroutines(); // 実行中のコルーチンを全て停止
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (fireAlarmObject != null)
        {
            fireAlarmObject.SetActive(false);
        }
        Debug.Log("強制的に火災報知器を停止しました。");
    }
}
