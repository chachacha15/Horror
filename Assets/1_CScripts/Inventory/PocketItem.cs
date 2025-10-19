using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class PocketItem
{
    public GameObject item;    //アイテムの見た目
    [Header("初ゲットのときに表示する説明文")] public string explainText; //アイテムの説明文

    [Header("出現しやすさ")] public int weight = 1; // 重み（スポーンさせやすさ）
    [Header("必ず必要なアイテムか")] public bool isImportant = false; // 必ず必要なアイテムかどうか

    [Header("アイテム取得時のモノローグ(あれば)")]
    public MonologueType MonologueTypeToActivation; // アイテム取得時に再生するモノローグの種類

    [Header("初ゲットでディスプレイするときのTransform")]
    public Vector3 displayPosition;            // ディスプレイ時の位置
    public Quaternion displayRotation;         // ディスプレイ時の回転
    public Vector3 displayScale = Vector3.one; // ディスプレイ時の大きさ

    [Header("インベントリのアイコンでディスプレイするときのTransform")]
    public Vector3 iconPosition;               // アイテムアイコン時の位置
    public Quaternion iconRotation;            // アイテムアイコン時の回転
    public Vector3 iconScale = Vector3.one;    // アイテムアイコン時の大きさ
}
