/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Onimanager : MonoBehaviour
{
    /*
     * 
     �����G���ǂ������Ă���v���O����
    public GameObject target;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {     
        agent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        if (target)
        { 
            agent.destination =target.transform.position;
        }
    }
    */

/*
    [SerializeField]
    [Tooltip("���񂷂�n�_�̔z��")]
    private Transform[] waypointArray;

    // NavMeshAgent�R���|�[�l���g������ϐ�
    private NavMeshAgent navMeshAgent;
    // ���݂̖ړI�n
    private int currentWaypointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        // navMeshAgent�ϐ���NavMeshAgent�R���|�[�l���g������
        navMeshAgent = GetComponent<NavMeshAgent>();
        // �ŏ��̖ړI�n������
        navMeshAgent.SetDestination(waypointArray[currentWaypointIndex].position);
    }

    // Update is called once per frame
    void Update()
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyScript : MonoBehaviour
{
    public Transform Target;
    public Transform random;
    NavMeshAgent agent;
    bool sensor;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sensor == false)
        {
            agent.destination = random.transform.position;
        }
        else
        {
            agent.destination = Target.transform.position;
        }
    }

    public void tuiseki()
    {
        sensor = true;
    }

    public void haikai()
    {
        sensor = false;
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}