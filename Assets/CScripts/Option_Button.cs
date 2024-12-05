using UnityEngine;

public class Option_Button : MonoBehaviour
{
    public GameObject optionPrefab; // Option Prefab���w�肷��
    private GameObject instantiatedOption; // �C���X�^���X������Option�̎Q��

    public void OnOption_ButtonClick()
    {
        if (instantiatedOption == null)
        {
            // Option Prefab���C���X�^���X��
            instantiatedOption = Instantiate(optionPrefab, transform);

            // �����ʒu�����i�K�v�Ȃ�j
            RectTransform rectTransform = instantiatedOption.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero; // Canvas�����ɔz�u
            }
        }
        else
        {
            // �\��/��\����؂�ւ�
            instantiatedOption.SetActive(!instantiatedOption.activeSelf);
        }
    }
}