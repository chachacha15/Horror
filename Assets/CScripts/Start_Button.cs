using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start_Button : MonoBehaviour
{
    // Start is called before the first frame update
    // �n�܂������Ɏ��s����֐�	
    void Start () 
    { // �{�^���������ꂽ���AStartGame�֐������s
        gameObject.GetComponent<Button>().onClick.AddListener(StartGame);	
    } 
    // StartGame�֐�
    void StartGame() 
    { // GameScene�����[�h
        SceneManager.LoadScene("demoScene"); 
    }

}
