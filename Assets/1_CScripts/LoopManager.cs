using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.EditorTools;

public class LoopManager : MonoBehaviour
{
    public float initialLoopDuration = 60f; // �������[�v����
    private float remainingTime; // ���݂̃��[�v�̎c�莞��
    public int loopCount = 0; // ���݂̃��[�v��

    public Text timerText; // UI��Text�I�u�W�F�N�g

    private List<string> carriedItems = new List<string>(); // ���[�v�Ԃň����p���A�C�e��

    private void Awake()
    {
        // ���[�v�}�l�[�W�����V�[���Ԃŕێ�
        if (FindObjectsOfType<LoopManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        remainingTime = initialLoopDuration; // �������Ԃ�ݒ�
    }

    private void Update()
    {
        // ���Ԃ�i�߂�
        remainingTime -= Time.deltaTime;

        // 0�L�[�Ŏc�莞�Ԃ�5�b�Z�k
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            remainingTime -= 5f;
            if (remainingTime < 0) remainingTime = 0; // �}�C�i�X�ɂȂ�Ȃ��悤����
        }

        // ���Ԃ�0�ȉ��ɂȂ����烋�[�v���J�n
        if (remainingTime <= 0)
        {
            StartNewLoop();
        }

        // �c�莞�Ԃ�Text�ɔ��f
        timerText.text = $"�c�莞��: {GetRemainingTime():F1} �b�@(0�L�[�łT�b�Z�k)";

    }

    private void StartNewLoop()
    {
        // ���[�v�񐔂𑝉�
        loopCount++;
        Debug.Log($"���[�v�J�n: {loopCount}���");

        // ���݂̃V�[�����ēǂݍ���
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // �c�莞�Ԃ����Z�b�g
        remainingTime = initialLoopDuration;
    }

    // �A�C�e����ǉ�
    public void AddItem(string itemName)
    {
        if (!carriedItems.Contains(itemName))
            carriedItems.Add(itemName);
    }

    // �A�C�e����ێ����Ă��邩�m�F
    public bool HasItem(string itemName)
    {
        return carriedItems.Contains(itemName);
    }

    // ���[�v�񐔂��擾
    public int GetLoopCount()
    {
        return loopCount;
    }

    // ���݂̎c�莞�Ԃ��擾
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
