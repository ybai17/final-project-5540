using UnityEngine;
using System.Collections;

public class HRController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float detectionRange = 5f;
    public float catchDistance = 1f;
    public Transform player;
    public Transform respawnPoint;

    [Header("Penalties")]
    public float salaryPenalty = 50f;

    [Header("Jump-Scare Settings")]
    public float catchAnimationDuration = 1.5f;
    public float jumpScareScale = 1.5f;

    [Header("Audio")]
    public AudioClip detectedSound;
    public AudioClip talkingSound;

    [Header("Dialogue")]
    public string captureDialogue = "HR: Hello, the boss wants to talk to you about a salary adjustment! Please go back to your station.";

    enum State { Idle, Walk, Run, Catch }
    State currentState = State.Idle;

    private float stateTimer;
    private float idleDuration;
    private float walkDuration;
    private Vector3 walkDirection;
    private bool isCatching;

    void Start()
    {
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
        if (currentState == newState) return;
        currentState = newState;
        stateTimer = 0f;

        // Stop catch coroutine if leaving catch state
        if (newState != State.Catch && isCatching)
        {
            StopAllCoroutines();
            isCatching = false;
            transform.localScale = Vector3.one;
        }

        GetComponent<Animator>().SetInteger("animState", (int)newState);

        if (newState == State.Catch && !isCatching)
            StartCoroutine(CatchRoutine());
    }

    void Update()
    {
        if (currentState == State.Catch) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= detectionRange)
        {
            if (dist <= catchDistance)
                SetState(State.Catch);
            else
                SetState(State.Run);
        }
        else
        {
            if (currentState == State.Run)
            {
                idleDuration = Random.Range(1f, 3f);
                SetState(State.Idle);
                SetRandomWalkDirection();
            }
            else if (currentState == State.Idle || currentState == State.Walk)
            {
                stateTimer += Time.deltaTime;
                if (currentState == State.Idle && stateTimer >= idleDuration)
                {
                    walkDuration = Random.Range(2f, 5f);
                    SetState(State.Walk);
                }
                else if (currentState == State.Walk)
                {
                    transform.Translate(walkDirection * walkSpeed * Time.deltaTime, Space.World);
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(walkDirection), Time.deltaTime * 2f);
                    if (stateTimer >= walkDuration)
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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
                transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator CatchRoutine()
    {
        isCatching = true;
        // play detection sound
        AudioSource.PlayClipAtPoint(detectedSound, transform.position);

        // disable player movement
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // jump-scare scale up/down
        Vector3 originalScale = transform.localScale;
        Vector3 scareScale = originalScale * jumpScareScale;
        float halfDur = catchAnimationDuration * 0.5f;
        float t = 0f;
        while (t < halfDur)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, scareScale, t / halfDur);
            yield return null;
        }
        t = 0f;
        while (t < halfDur)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(scareScale, originalScale, t / halfDur);
            yield return null;
        }

        // apply salary penalty via health
        var playerHealthComp = player.GetComponent<PlayerHealth>();
        if (playerHealthComp != null) playerHealthComp.TakeDamage(salaryPenalty);

        // respawn player
        if (respawnPoint != null) player.position = respawnPoint.position;

        // re-enable player
        if (cc != null) cc.enabled = true;

        // show dialogue
        AudioSource.PlayClipAtPoint(talkingSound, transform.position);
        var ui = GameObject.FindGameObjectWithTag("UI")?.GetComponent<UIManager>();
        if (ui != null) ui.DisplayDialogue(captureDialogue);

        // reset
        transform.localScale = Vector3.one;
        isCatching = false;
        SetState(State.Idle);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            SetState(State.Catch);
    }
}

