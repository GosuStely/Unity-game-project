using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Eye_Movement : MonoBehaviour
{
    public float speed;
    public float checkRadius;
    public float attackRadius;
    public float bulletForce = 20f;

    public bool shouldRotate;

    public float timeBtwShots;
    public float startTimeBtwShots;

    public LayerMask whatIsPlayer;

    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    public Vector3 direction;

    private bool isInChaseRange;
    private bool isInAttackRange;
    private bool runOutOfHP;

    [SerializeField] private int HP = 2;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        timeBtwShots = startTimeBtwShots;
    }

    private void Update()
    {
        anim.SetBool("isChasing", isInChaseRange);
        anim.SetBool("isAttacking", isInAttackRange);
        anim.SetBool("isDead", runOutOfHP);

        if (runOutOfHP)
        {
            // Trigger death animation and disable further actions
            rb.velocity = Vector2.zero;
            return;
        }
        isInChaseRange = Physics2D.OverlapCircle(transform.position, checkRadius, whatIsPlayer);
        isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, whatIsPlayer);

        direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        direction.Normalize();
        movement = direction;
        if (shouldRotate)
        {
            anim.SetFloat("X", direction.x);
            anim.SetFloat("Y", direction.y);

        }
    }

    private void FixedUpdate()
    {
        if (isInChaseRange && !isInAttackRange)
        {
            MoveCharacter(movement);
        }
        if (isInAttackRange)
        {
            rb.velocity = Vector2.zero;
            if (timeBtwShots <= 0)
            {
                Attack();
                timeBtwShots = startTimeBtwShots;
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }

    }

    private void MoveCharacter(Vector2 direction)
    {
        rb.MovePosition((Vector2)transform.position + (direction * speed * Time.deltaTime));
    }

    void Attack()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Arrow")
        {
            Destroy(collision.gameObject);

            HP -= 1;
            if (HP <= 0)
            {
                speed = 0;
                runOutOfHP = true;
                anim.SetTrigger("isDead");
                StartCoroutine(DestroyAfterDeath());
            }
        }
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(0.9f);
        Destroy(gameObject);
    }
}
