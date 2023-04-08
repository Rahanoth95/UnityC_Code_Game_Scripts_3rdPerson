using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public int maxHealth = 100;
    private int currentHealth;

    public NavMeshAgent agent;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;


    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;



    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && !playerInSightRange) AttackPlayer();


    }

    //Patrolling Func
    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Reached Walkpoint
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    //Searches for Walkpoints
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, whatIsGround))
            walkPointSet = true;
    }

    //Finds and Chases Player
    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    //Attacks Player
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        if (!alreadyAttacked)
        {


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //Die Animation + Function
    void Die()
    {
        Debug.Log("Enemy Died!");

        animator.SetBool("IsDead", true);

        GetComponent<Collider>().enabled = false;

        this.enabled = false;
    }
}