using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    [Tooltip("�ǂ�������Ώ�")]
    private GameObject player;

    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        // NavMeshAgent��ێ����Ă���
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[��ڎw���Đi��
        navMeshAgent.destination = player.transform.position;
    }
}
