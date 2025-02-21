using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class randomScript : MonoBehaviour
{
    /*
    void Start()
    {
        // navMeshAgent�ϐ���NavMeshAgent�R���|�[�l���g������
        navMeshAgent = GetComponent<NavMeshAgent>();
        // �ŏ��̖ړI�n������
        navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
        StartCoroutine(Warp());
    }

    [SerializeField]
    [Tooltip("���񂷂�n�_�̔z��")]
    private Transform[] waypointArray;

    // NavMeshAgent�R���|�[�l���g������ϐ�
    private NavMeshAgent navMeshAgent;
    // ���݂̖ړI�n
    private int currentWaypointIndex = 0;
    private IEnumerator Warp()
    {
        while (true)
        {
            // �ړI�n�_�܂ł̋���(remainingDistance)���ړI�n�̎�O�܂ł̋���(stoppingDistance)�ȉ��ɂȂ�����
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                // �ړI�n�̔ԍ����P�X�V�i�E�ӂ���]���Z�q�ɂ��邱�ƂŖړI�n�����[�v�������j
                currentWaypointIndex = (currentWaypointIndex + 1) % waypointArray.Length;
                // �ړI�n�����̏ꏊ�ɐݒ�
                navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
            }
        }
    }
    */
    void Start()
    {
        StartCoroutine(Warp());
    }

    private IEnumerator Warp()
    {
        while (true)
        {
            // 10�b�ゲ�ƂɃ��[�v�ړ�����B
            yield return new WaitForSeconds(10f);

            // �����_���Ȓl���擾����B
            float posX = Random.Range(-120, 120);
            float posZ = Random.Range(-200, 200);

            transform.position = new Vector3(posX, 0, posZ);
        }
    }
}
