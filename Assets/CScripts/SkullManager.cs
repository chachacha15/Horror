using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;


public class SkullManager : MonoBehaviour
{
    #region Variables
    public GameObject fakeSkullPrefab; // 偽物の骸骨（全て同じもの）
    public List<GameObject> realSkullPrefabs; // 本物の骸骨の候補（リストから1体を選ぶ）
    public Transform[] spawnPoints; // 骸骨を配置する位置（3箇所）

    public GameObject confirmationUI; // 確認用UI
    public TextMeshProUGUI confirmationText; // 「この骸骨が本物ですか？」のメッセージ
    private Skull selectedSkull; // クリックされた骸骨を記録


    private Skull realSkullInstance; // 実際に配置された本物の骸骨

    private bool isConfirmed = false; // UIを一度開いたらtrueになる

    #endregion

    private void Start()
    {

        SpawnSkulls();
    }

    private void SpawnSkulls()
    {
        if (realSkullPrefabs.Count == 0)
        {
            Debug.LogError("本物の骸骨Prefabリストが空です！");
            return;
        }

        // 本物の骸骨をリストからランダムに1つ選ぶ
        GameObject selectedRealSkullPrefab = realSkullPrefabs[Random.Range(0, realSkullPrefabs.Count)];

        if (selectedRealSkullPrefab == null)
        {
            Debug.LogError("選択された本物の骸骨Prefabが null です！");
            return;
        }

        // 3体の骸骨を配置（1つは本物）
        int realSkullIndex = Random.Range(0, spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject spawnedSkull;

            if (i == realSkullIndex)
            {
                // 本物の骸骨を配置
                spawnedSkull = Instantiate(selectedRealSkullPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                realSkullInstance = spawnedSkull.GetComponent<Skull>();
                realSkullInstance.SetReal(true);
            }
            else
            {
                // 偽物の骸骨を配置
                spawnedSkull = Instantiate(fakeSkullPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                spawnedSkull.GetComponent<Skull>().SetReal(false);
            }
        }
    }

    // プレイヤーが骸骨をクリックした時の処理
    public void OnSkullClicked(Skull clickedSkull)
    {
        if (clickedSkull == realSkullInstance)
        {
            clickedSkull.RevealReal(); // 本物なら顎を外す
        }
        else
        {
            Debug.Log("これは偽物だ！");
        }
    }

    public void ShowConfirmationUI(Skull skull)
    {
        // すでに確認済みならUIを表示しない
        if (isConfirmed) return;

        selectedSkull = skull;
        confirmationText.text = "この骸骨が本物ですか？";
        confirmationUI.SetActive(true); // UIを表示
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None; // マウスを表示
        Cursor.visible = true;
    }

    public void OnConfirm() // OKボタンが押されたとき
    {
        if (selectedSkull != null)
        {
            DisableAllSkulls(); // すべての骸骨のコライダーを無効化
            OnSkullClicked(selectedSkull); // 本物を開く
        }
        confirmationUI.SetActive(false); // UIを閉じる
        Cursor.lockState = CursorLockMode.Locked; // マウスを非表示
        Cursor.visible = false;
        Time.timeScale = 1.0f;
        isConfirmed = true; // 以降、UIを開かせない



    }

    public void OnCancel() // キャンセルボタンが押されたとき
    {
        confirmationUI.SetActive(false); // UIを閉じる
        Cursor.lockState = CursorLockMode.Locked; // マウスを非表示
        Cursor.visible = false;
        Time.timeScale = 1.0f;

    }

    private void DisableAllSkulls()
    {
        // シーン内のすべてのSkullを取得して、コライダーを無効化
        Skull[] allSkulls = FindObjectsOfType<Skull>();
        foreach (Skull skull in allSkulls)
        {
            skull.DisableColliders();
        }
    }

}
