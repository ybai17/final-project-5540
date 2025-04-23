using UnityEngine;
using System.Collections;

public class HRController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float damagePerTick = 5f;           // one-time damage
    public float detectionRange = 5f;
    public float catchDistance = 1f;
    public Transform player;
    public Animator animator;
    public Transform respawnPoint;

    [Header("Catch Settings")]
    public float catchAnimationDuration = 1.5f; // wait this long before applying damage

    public AudioClip detectedSound;
    public AudioClip talkingSound;

    enum State { Idle, Walk, Run, Catch }
    State currentState = State.Idle;

    float stateTimer = 0f;
    float idleDuration;
    float walkDuration;
    Vector3 walkDirection;

    bool isCatching = false;

    string captureDialogue =
        "HR: Hello, the boss wants to talk to you about a salary adjustment! " +
        "Please go back to the waiting area.";

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        idleDuration = Random.Range(1f, 3f);
        walkDuration = Random.Range(2f, 5f);
        SetRandomWalkDirection();
        SetState(State.Idle);
    }

    void SetRandomWalkDirection()
    {
        float angle = Random.Range(0f, 360f);
        walkDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
    }

    void SetState(State newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case State.Idle:
                animator.SetInteger("animState", 0);
                StopCatching();
                break;
            case State.Walk:
                animator.SetInteger("animState", 1);
                StopCatching();
                break;
            case State.Run:
                animator.SetInteger("animState", 2);
                StopCatching();
                break;
            case State.Catch:
                animator.SetInteger("animState", 3);
                if (!isCatching)
                    StartCoroutine(CatchRoutine());
                break;
        }
    }

    void StopCatching()
    {
        isCatching = false;
        StopAllCoroutines();
    }

    void Update()
    {
        if (currentState == State.Catch) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectionRange)
        {
            SetState(dist <= catchDistance ? State.Catch : State.Run);
        }
        else
        {
            if (currentState == State.Run || currentState == State.Catch)
            {
                idleDuration = Random.Range(1f, 3f);
                SetState(State.Idle);
                SetRandomWalkDirection();
            }
            else
            {
                stateTimer += Time.deltaTime;
                if (currentState == State.Idle && stateTimer >= idleDuration)
                {
                    SetState(State.Walk);
                    walkDuration = Random.Range(2f, 5f);
                }
                else if (currentState == State.Walk && stateTimer >= walkDuration)
                {
                    idleDuration = Random.Range(1f, 3f);
                    SetState(State.Idle);
                    SetRandomWalkDirection();
                }
            }
        }

        if (currentState == State.Run)
        {
            Vector3 dir = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                Time.deltaTime * 5f
            );
            transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
        }
        else if (currentState == State.Walk)
        {
            transform.Translate(
                walkDirection * walkSpeed * Time.deltaTime,
                Space.World
            );
            if (walkDirection != Vector3.zero)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(walkDirection),
                    Time.deltaTime * 2f
                );
        }
    }

    IEnumerator CatchRoutine()
{
    isCatching = true;

    // 1) Play detection sound
    if (detectedSound != null)
        AudioSource.PlayClipAtPoint(detectedSound, transform.position);

    // 2) Calculate the fixed offset in front of HR
    Vector3 dir      = (player.position - transform.position).normalized;
    Vector3 lockPos  = transform.position + dir * catchDistance;
    float  timer     = 0f;

    // 3) While the catch animation is playing...
    while (timer < catchAnimationDuration)
    {
        timer += Time.deltaTime;

        // a) Force the player to stay at lockPos
        player.position = lockPos;

        // b) Ensure HR faces the player
        Vector3 lookPos = player.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);

        yield return null;
    }

    // 4) One‐time damage
    var playerHealth = player.GetComponent<PlayerHealth>();
    if (playerHealth != null)
        playerHealth.TakeDamage(damagePerTick);

    // 5) Talking sound & dialogue
    if (talkingSound != null)
        AudioSource.PlayClipAtPoint(talkingSound, transform.position);
    var ui = GameObject.FindGameObjectWithTag("UI")?.GetComponent<UIManager>();
    if (ui != null)
        ui.DisplayDialogue(captureDialogue);

    // 6) Respawn player
    if (respawnPoint != null)
        player.position = respawnPoint.position;

    // 7) Reset HR and go back to Idle
    isCatching = false;
    SetState(State.Idle);
}


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetState(State.Catch);
        }
    }
    void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player") && currentState == State.Catch)
    {
        // Push the player back so they stay inside the trigger
        Vector3 dir       = (other.transform.position - transform.position).normalized;
        Vector3 keepPos   = transform.position + dir * catchDistance;
        other.transform.position = keepPos;
    }
}

}

