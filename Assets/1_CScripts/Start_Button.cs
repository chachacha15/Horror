using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start_Button : MonoBehaviour
{
    // Start is called before the first frame update
    // 始まった時に実行する関数	
    void Start () 
    { // ボタンが押された時、StartGame関数を実行
        gameObject.GetComponent<Button>().onClick.AddListener(StartGame);	
    } 
    // StartGame関数
    void StartGame() 
    { // GameSceneをロード
        SceneManager.LoadScene("demoScene"); 
    }

}
