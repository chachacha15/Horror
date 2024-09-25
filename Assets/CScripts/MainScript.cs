using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    // �ʒu�Ɖ�]�����\����
    [System.Serializable] // �C���X�y�N�^�[�ŕ\���\�ɂ��邽��
    public struct TransformData
    {
        public Vector3 position; // �ʒu
        public Quaternion rotation; // ��]
    }

    public static TransformData [] data = new TransformData [16];
    public GameObject RoomsParent; //�����v���t�@�u�����̐e
    private GameObject RoomsChild; //�V�̎q���i�[���邽�߂̂���
    private SceneChanger scenechanger;
    private string scenename = "Jikkenyou";
    // Start is called before the first frame update
    void Start()
    {
        scenechanger = FindObjectOfType<SceneChanger>();

        // �C���X�^���X�������I�u�W�F�N�g�̎q�I�u�W�F�N�g�̃f�[�^���擾
        for (int num = 0; num < 16; ++num)
        {
            data[num].position = RoomsParent.transform.GetChild(num).position;
            data[num].rotation = RoomsParent.transform.GetChild(num).rotation;
        }

        scenechanger.ChangeScene(scenename);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
