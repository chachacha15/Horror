using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class PocketItem
{
    public GameObject item;    //アイテムの見た目
    public string explainText; //アイテムの説明文
    public Sprite icon;        // アイテムのアイコン画像

    public Vector3 displayPosition;    // ディスプレイ時の位置
    public Quaternion displayRotation; // ディスプレイ時の回転
    public Vector3 displayScale = Vector3.one;       // ディスプレイ時の大きさ

    public Vector3 iconPosition;    // アイテムアイコン時の位置
    public Quaternion iconRotation; // アイテムアイコン時の回転
    public Vector3 iconScale = Vector3.one;       // アイテムアイコン時の大きさ
}
