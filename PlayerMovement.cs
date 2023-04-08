using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Var
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    private Vector3 moveDirection;
    private Vector3 velocity;

    [SerializeField] private bool isGrounded;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;

    [SerializeField] private float jumpHeight;

     public Transform attackPoint;
     public LayerMask enemyLayers;
     public float attackRange = 0.5f;
     public int attackDamage = 20;
     public float attackRate = 2f;
     private float nextAttackTime = 0f;


    //Ref
    private CharacterController controller;
    private Animator anim;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Move();

        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(Attack());
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    private void Move()
        {
        //ChecksIfSphere is transformed to proper grounding
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        moveDirection = new Vector3(moveX * moveSpeed, moveDirection.y, moveZ * moveSpeed);
        moveDirection = transform.TransformDirection(moveDirection);


        if(isGrounded)
        {
            if (moveDirection != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
            {
                Walk();

            }
            else if (moveDirection != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
            {
                Run();
            }

            else if (moveDirection == Vector3.zero)
            {
                Idle();
            }
            moveDirection *= moveSpeed;

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }

        controller.Move(moveDirection * Time.deltaTime);


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    private void Idle()
    {
        anim.SetFloat("Speed",0, 0.1f , Time.deltaTime);
    }

    private void Walk()
    {
        moveSpeed = walkSpeed;
        anim.SetFloat("Speed", 0.5f, 0.1f , Time.deltaTime);

    }

    private void Run()
    {
        moveSpeed = runSpeed;
        anim.SetFloat("Speed", 1, 0.1f , Time.deltaTime);

    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight *  -2 * gravity);
    }

     IEnumerator Attack()
    {
        anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 1);
        anim.SetTrigger("Attack");

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }

        yield return new WaitForSeconds(0.9f);
        anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);

    }


    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
