using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Eye : MonoBehaviour
{
    public float checkRadius;
    public float attackRadius;
    public float bulletForce = 20f;

    public LayerMask whatIsPlayer;

    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private BossBomb_Spawner bombSpawner;

    private bool isInAttackRange;
    private bool runOutOfHP;


    [SerializeField] private int HP = 50;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;

        bombSpawner = GetComponentInChildren<BossBomb_Spawner>();
        // Start the boss attack cycle coroutine
        StartCoroutine(BossAttackCycle());
    }

    IEnumerator BossAttackCycle()
    {
        while (true)
        {
            // Wait for 6 seconds before starting the attack
            yield return new WaitForSeconds(bombSpawner.waveInterval);

            // Set isAttacking trigger to true, play the attack animation
            anim.SetBool("isAttacking", true);

            // Wait for 5 seconds while in the attack animation
            yield return new WaitForSeconds(bombSpawner.waveDuration);

            // Set isAttacking trigger to false, play the idle animation
            anim.SetBool("isAttacking", false);
        }
    }

    private void Update()
    {

        if (runOutOfHP)
        {
            // Trigger death animation and disable further actions
            rb.velocity = Vector2.zero;
            return;
        }
        // Set the isAttacking parameter based on the isInAttackRange condition
        anim.SetBool("isAttacking", isInAttackRange);

    }

    private void FixedUpdate()
    {
        isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, whatIsPlayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Arrow")
        {
            Destroy(collision.gameObject);

            HP -= 1;
            if (HP <= 0)
            {
                runOutOfHP = true;
                anim.SetTrigger("isDead");
                StartCoroutine(DestroyAfterDeath());
            }
        }
    }

    IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
