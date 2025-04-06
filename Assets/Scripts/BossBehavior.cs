using UnityEngine;
using UnityEngine.UI;

public class BossBehavior : MonoBehaviour
{
    Animator animator;

    public Transform target;

    public enum BossState {
        Resting,
        Approaching,
        Attacking,
        Dying,
    }

    public BossState currentState;

    public Slider healthSlider;
    public float maxHP = 100f;
    public float currentHP;
    public float moveSpeed = 2f;
    public float attackRange = 4f;
    public float damage = 10f;
    public float attackForce = 2f;
    public AudioClip deathSound;
    public AudioClip punchSound;
    bool isDying;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!target)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        animator = transform.GetChild(0).GetComponent<Animator>();

        currentState = BossState.Resting;
        isDying = false;
        currentHP = maxHP;

        if (!healthSlider)
        {
            healthSlider = GameObject.FindGameObjectWithTag("UI")
                                     .transform.GetChild(5)
                                     .transform.GetChild(1)
                                     .gameObject.GetComponent<Slider>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug only
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(5);
        }

        switch (currentState)
        {
            case BossState.Resting:
                Rest();
                break;
            case BossState.Approaching:
                Approach();
                break;
            case BossState.Attacking:
                Attack();
                break;
            case BossState.Dying:
                Die();
                break;
        }
    }

    void Rest()
    {
        currentState = BossState.Approaching;
    }

    void Approach()
    {
        Vector3 groundedTarget = target.position;
        groundedTarget.y = 0.5f;

        transform.position = Vector3.MoveTowards(transform.position, groundedTarget, moveSpeed * Time.deltaTime);
        transform.LookAt(groundedTarget);

        CheckIfCanAttack();
    }

    void Attack()
    {
        Vector3 groundedTarget = target.position;
        groundedTarget.y = 0.5f;

        transform.position = Vector3.MoveTowards(transform.position, groundedTarget, 0.5f * moveSpeed * Time.deltaTime);
        transform.LookAt(groundedTarget);
    }

    void Die()
    {
        Debug.Log("Die state");

        if (isDying)
            return;

        isDying = true;
        animator.SetTrigger("DieTrigger");

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

        AudioSource.PlayClipAtPoint(deathSound, transform.position, 100);
        
        Destroy(gameObject, 2);
    }

    void CheckIfCanAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach(Collider curr in colliders)
        {
            if (curr.gameObject.CompareTag("Player"))
            {
                currentState = BossState.Attacking;
            }
        }
    }

    void TakeDamage(float damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, 100);

        healthSlider.value = currentHP;

        if (currentHP <= 0)
        {
            currentState = BossState.Dying;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            TakeDamage(5);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 playerPos = collision.gameObject.GetComponent<Transform>().position;

            //collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);

            collision.gameObject.GetComponent<CharacterController>().Move((playerPos - transform.position) * attackForce);

            AudioSource.PlayClipAtPoint(punchSound, transform.position);
        }
    }
}
