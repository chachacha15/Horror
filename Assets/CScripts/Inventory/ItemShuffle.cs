using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShuffle : MonoBehaviour
{
    #region Variables
    public GameObject[] item;
    public GameObject red;
    public GameObject yellow;
    public GameObject blue;
    public GameObject battery;

    public GameObject posGameobject;

    int i = 0;

    Vector3[] Pos = new Vector3[5];
    #endregion

    void Start()
    {
        
        foreach (Transform child in transform)
        {
            Pos[i++] = child.localPosition;
        }
        Shuffle(Pos);

        for (int i = 0; i < item.Length; i++)
        {
            GameObject instance = Instantiate(item[i], Pos[i], Quaternion.identity);
            instance.name = item[i].name;
        }
    }

    void Update()
    {
        
    }

    // �t�B�b�V���[�E�C�F�[�c�̃V���b�t���A���S���Y���𗘗p�����z��V���b�t��
    void Shuffle(Vector3[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1); // 0����i�܂ł̃����_���ȃC���f�b�N�X���擾
            Vector3 temp = array[i];                 // ���݂̗v�f���ꎞ�ۑ�
            array[i] = array[randomIndex];           // �����_���Ȉʒu�̗v�f�����݂̈ʒu��
            array[randomIndex] = temp;               // �ꎞ�ۑ������v�f�������_���Ȉʒu��
        }
    }
}
