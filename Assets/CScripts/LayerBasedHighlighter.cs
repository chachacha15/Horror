using UnityEngine;

public class NearbyItemHighlighter : MonoBehaviour
{
    public float interactDistance = 3f;       // ��������ő勗��
    private GameObject currentHighlightedItem; // ���݋�������Ă���A�C�e��
    private int defaultLayer;                 // ���̃��C���[��ۑ�

    public string highlightLayer = "HighLight"; // �����p���C���[��

    private void Update()
    {
        HighlightNearbyItems();
    }

    private void HighlightNearbyItems()
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, interactDistance);

        foreach (Collider collider in nearbyColliders)
        {
            GameObject item = collider.gameObject;

            if (item.CompareTag("Item")) // Item�^�O�����I�u�W�F�N�g�����o
            {
                if (currentHighlightedItem != item)
                {
                    ClearHighlight(); // �����̃n�C���C�g������
                    ApplyHighlight(item); // �V�����A�C�e��������
                }

                return; // �ŏ��Ɍ�����Item�^�O�̃I�u�W�F�N�g������
            }
        }

        ClearHighlight(); // ����������Ȃ������ꍇ�͋���������
    }

    private void ApplyHighlight(GameObject item)
    {
        currentHighlightedItem = item;
        defaultLayer = item.layer; // ���̃��C���[��ۑ�

        // �����p���C���[���擾
        int highlightLayerIndex = LayerMask.NameToLayer(highlightLayer);

        if (highlightLayerIndex == -1)
        {
            Debug.LogError($"���C���[ '{highlightLayer}' �����݂��܂���BUnity�̃��C���[�ݒ���m�F���Ă��������B");
            return;
        }

        // �����p���C���[�ɕύX
        item.layer = highlightLayerIndex;
    }

    private void ClearHighlight()
    {
        if (currentHighlightedItem != null)
        {
            // ���̃��C���[�ɖ߂�
            currentHighlightedItem.layer = defaultLayer;
            currentHighlightedItem = null;
        }
    }
}
