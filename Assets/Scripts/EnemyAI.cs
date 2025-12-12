using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private NavMeshAgent agent;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Animator animator;

    private Transform player;

    private PlayerStats playerStats;

    [Header("Stats")]

    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float chaseSpeed;

    [SerializeField]
    private float detectionRange;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private float attackDelay;

    [SerializeField]
    private float damageDealt;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float Maxhealth;

    [SerializeField]
    private float currentHealth;

    [Header("Wandering Settings")]
    [SerializeField]
    private float wanderingWaitTimeMin;

    [SerializeField]
    private float wanderingWaitTimeMax;

    [SerializeField]
    private float wanderingDistanceMin;

    [SerializeField]
    private float wanderingDistanceMax;


    private bool isAwaitingDestination;

    private bool isAttacking;

    private bool isDead = false;

    private void Awake()
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform;
        playerStats = player.GetComponent<PlayerStats>();
        currentHealth = Maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= detectionRange && !playerStats.isDead)
        {
            agent.speed = chaseSpeed;

            Quaternion lookRotation = Quaternion.LookRotation((player.position - transform.position).normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            if (!isAttacking)
            {
                if (Vector3.Distance(transform.position, player.position) <= attackRange)
                {
                    StartCoroutine(AttackPlayer());
                }
                else {
                    agent.SetDestination(player.position); 
                }
            }
            
        }
        else if (agent.remainingDistance < 0.75f && !isAwaitingDestination)
        {
            agent.speed = walkSpeed;
            StartCoroutine(GetNewDestination());
        }
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    IEnumerator GetNewDestination()
    {
        isAwaitingDestination = true;
        yield return new WaitForSeconds(Random.Range(wanderingWaitTimeMin, wanderingWaitTimeMax));
        Vector3 nextDestination = transform.position;
        nextDestination += Random.Range(wanderingDistanceMin,wanderingDistanceMax) * new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(nextDestination, out hit, wanderingDistanceMax, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        isAwaitingDestination = false;
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        agent.isStopped = true;
        audioSource.Play();
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);
        playerStats.TakeDamage(damageDealt);
        if (agent.enabled)
        {
            agent.isStopped = false;
        }
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            isDead = true;
            animator.SetTrigger("Die");
            agent.enabled = false;
            this.enabled = false;
        }
        else
        {
            animator.SetTrigger("GetHit");
        }
        
    }
}
