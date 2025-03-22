using UnityEngine;
using UnityEngine.AI;

public class HRBehaviorNEW : MonoBehaviour
{
    public enum HRState {
        Wait,
        Patrol,
        Chase,
    }

    [Header("General")]
    public HRState currentState;

    public float movementSpeed = 1f;
    public float turnSpeed = 5f;
    public float damage = 10f;
    public Transform target;
    public Light spotlight;
    public float spotlightChaseThreshold = 0.9f;

    [Header("Patrol Settings")]
    public float waitTime = 5f;
    public Transform[] patrolPoints;
    Transform currentPoint;
    int currentPointIndex;
    Transform nextPoint;
    int nextPointIndex;

    bool isReturning;

    float startWaitTime;
    float endWaitTime = 5;

    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        isReturning = false;

        currentPointIndex = 0;
        currentPoint = patrolPoints[0];

        nextPointIndex = 1;
        nextPoint = patrolPoints[1];

        currentState = HRState.Patrol;

        if (!spotlight)
            spotlight = transform.GetChild(0).gameObject.GetComponent<Light>();

        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState) {
            case HRState.Wait:
                Wait();
                break;
            case HRState.Patrol:
                Patrol();
                break;
            case HRState.Chase:
                Chase();
                break;
        }
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

    void Wait()
    {
        Debug.Log("Waiting");

        if (Time.time <= endWaitTime)
        {
            //look at next point
            Vector3 lookDirection = (nextPoint.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
            return;
        }

        //wait time is up, so we need to start moving
        currentState = HRState.Patrol;
        agent.SetDestination(nextPoint.localPosition);
    }
    
    void Patrol()
    {  
        Debug.Log("Goal checkpoint: " + nextPoint + " with coordinates " + nextPoint.position);
        Debug.Log("Patrolling towards " + agent.destination);
        //moving towards next point

        //check if we reached next point
        if (transform.position == nextPoint.position)
        {
            Debug.Log("reached checkpoint");
            ReachedCheckpoint();
            currentState = HRState.Wait;
        }
    }

    void Chase()
    {
        Debug.Log("Chasing");
    }
}
