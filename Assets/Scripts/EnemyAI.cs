using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Patrol, Chase, Attack }

    [Header("Movement Settings")]
    public NavMeshAgent agent;
    public float patrolRadius = 10f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;

    [Header("Sight Settings")]
    public float sightRange = 10f;
    [Range(0,360)] public float sightAngle = 120f;
    public float eyeHeight = 1.8f;              // sight origin height
    public LayerMask obstacleMask;              // walls tagged "office"

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    private float attackTimer;

    [Header("Animation")]
    public Animator animator;                  

    private State currentState;
    private Transform player;
    private Vector3 patrolTarget;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.updateUpAxis = true;
        agent.stoppingDistance = attackRange;

        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj) player = playerObj.transform;

        attackTimer = 0f;
        TransitionToState(State.Patrol);
    }

    void Update()
    {
        // update state behavior
        switch (currentState)
        {
            case State.Idle:   StateIdle();   break;
            case State.Patrol: StatePatrol(); break;
            case State.Chase:  StateChase();  break;
            case State.Attack: StateAttack(); break;
        }
        // update animator
        if (animator) animator.SetInteger("state", (int)currentState);
    }

    void TransitionToState(State newState)
    {
        currentState = newState;
        switch (newState)
        {
            case State.Patrol:
                agent.speed = patrolSpeed;
                ChoosePatrolPoint();
                agent.SetDestination(patrolTarget);
                break;
            case State.Chase:
                agent.speed = chaseSpeed;
                break;
            case State.Attack:
                agent.speed = 0f;
                attackTimer = attackCooldown;
                break;
            case State.Idle:
                agent.speed = 0f;
                break;
        }
    }

    void StateIdle()
    {
        // simple idle: just look for player
        if (CanSeePlayer()) TransitionToState(State.Chase);
    }

    void StatePatrol()
    {
        if (CanSeePlayer())
        {
            TransitionToState(State.Chase);
            return;
        }
        // wander until destination reached
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            ChoosePatrolPoint();
            agent.SetDestination(patrolTarget);
        }
    }

    void StateChase()
    {
        if (player == null) { TransitionToState(State.Patrol); return; }
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackRange)
            TransitionToState(State.Attack);
        else if (!CanSeePlayer())
            TransitionToState(State.Patrol);
    }

    void StateAttack()
    {
        if (player == null) { TransitionToState(State.Patrol); return; }
        transform.LookAt(player);
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            // jump-scare lunge
            var rb = GetComponent<Rigidbody>();
            if (rb) rb.AddForce((player.position - transform.position).normalized * attackRange * 5f, ForceMode.Impulse);
            attackTimer = attackCooldown;
        }
        // after lunging, check distance
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > attackRange * 1.5f)
            TransitionToState(State.Chase);
    }

    void ChoosePatrolPoint()
    {
        // pick random point on NavMesh within patrolRadius
        Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
        randomDir += transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            patrolTarget = hit.position;
        else
            patrolTarget = transform.position;
    }

    bool CanSeePlayer()
    {
        if (player == null) return false;
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 dir = (player.position - origin);
        if (dir.magnitude > sightRange) return false;
        if (Vector3.Angle(transform.forward, dir) > sightAngle * 0.5f) return false;
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, sightRange, obstacleMask))
        {
            if (hit.collider.CompareTag("Player")) return true;
            return false;
        }
        return false;
    }
}


