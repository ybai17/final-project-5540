using UnityEngine;

public class HRBehavior : MonoBehaviour
{

    public float movementSpeed = 1f;

    public float waitTime = 5f;
    public float turnSpeed = 5f;

    public float damage = 10f;

    public Transform target;
    //Vector3 targetGrounded;

    public Transform[] patrolPoints;
    Transform currentPoint;
    int currentPointIndex;
    Transform nextPoint;
    int nextPointIndex;

    bool isReturning;

    float startWaitTime;
    float endWaitTime;

    float goalAngle;
    Quaternion goalAngleQ;

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

            //get angle we need to turn towards to look at the next point
            goalAngle = Mathf.Atan2(nextPoint.position.x - transform.position.x, nextPoint.position.z - transform.position.z);
            goalAngle *= Mathf.Rad2Deg;
            goalAngleQ = Quaternion.Euler(0, goalAngle, 0);
        }

        if (Time.time <= endWaitTime) {
            WaitAndTurn();
            return;
        }

        //Debug.Log("Moving towards: " + nextPoint);
        transform.position = Vector3.MoveTowards(transform.position, nextPoint.position, movementSpeed * Time.deltaTime);
        //transform.LookAt(nextPoint);
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
        
        transform.rotation = Quaternion.Lerp(transform.rotation, goalAngleQ, turnSpeed * Time.deltaTime);
    }

    /*
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("Triggered with " + other.gameObject.name);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
    */
}
