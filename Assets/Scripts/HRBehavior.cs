using UnityEngine;

public class HRBehavior : MonoBehaviour
{

    public float movementSpeed = 1f;

    public float waitTime = 5f;
    public float turnSpeed = 5f;

    public float damage = 10f;

    public Transform target;

    public Transform[] patrolPoints;

    public AudioClip detectedSound;
    public AudioClip talkingSound;

    Transform currentPoint;
    int currentPointIndex;
    Transform nextPoint;
    int nextPointIndex;

    bool isReturning;

    float startWaitTime;
    float endWaitTime;

    string captureDialogue = "HR: Hello, the boss wants to talk to you about a salary adjustment! " +
        "Please go back to the waiting area.";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        isReturning = false;

        currentPointIndex = 0;
        currentPoint = patrolPoints[0];

        nextPointIndex = 1;
        nextPoint = patrolPoints[1];
    }

    // Update is called once per frame
    void Update()
    {
        //reached a checkpoint
        if (transform.position == nextPoint.position) {
            
            Debug.Log("Reached checkpoint: " + nextPoint);
            ReachedCheckpoint();
            
            Debug.Log("startWaitTime: " + startWaitTime);
            Debug.Log("endWaitTime: " + endWaitTime);
        }

        if (Time.time <= endWaitTime) {
            WaitAndTurn();
            return;
        }

        //Debug.Log("Moving towards: " + nextPoint);
        transform.position = Vector3.MoveTowards(transform.position, nextPoint.position, movementSpeed * Time.deltaTime);
    }

    void ReachedCheckpoint()
    {
        startWaitTime = Time.time;
        endWaitTime = startWaitTime + waitTime;

        currentPointIndex = nextPointIndex;
        currentPoint = nextPoint;

        //reached end
        if (currentPointIndex == patrolPoints.Length - 1) {
            isReturning = true;
        }
        //reached start
        else if (currentPointIndex == 0) {
            isReturning = false;
        }

        if (isReturning) {
            nextPointIndex = currentPointIndex - 1;
        } else {
            nextPointIndex = currentPointIndex + 1;
        }
        nextPoint = patrolPoints[nextPointIndex];
    }

    void WaitAndTurn()
    {
        Debug.Log("WAITING");
        
        //look at next point
        Vector3 lookDirection = (nextPoint.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("HR Triggered with " + other.gameObject.name);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            CapturePlayer();
        }
    }

    public void CapturePlayer()
    {
        Debug.Log("Captured player!");

        AudioSource.PlayClipAtPoint(detectedSound, transform.position);
        AudioSource.PlayClipAtPoint(talkingSound, transform.position);
        
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().TakeDamage(damage);

        GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>().DisplayDialogue(captureDialogue);
    }
}
