using UnityEngine;

public class HRController : MonoBehaviour
{public float walkSpeed = 2f;          
    public float runSpeed = 4f;           
    public float detectionRange = 5f;     
    public float catchDistance = 1f;      
    public Transform player;             
    public Animator animator;            

    enum State { Idle, Walk, Run, Catch }
    State currentState = State.Idle;
    float stateTimer = 0f;
    float idleDuration;          
    float walkDuration;           
    Vector3 walkDirection;        

      void Start()
    {
       if(animator == null)
            animator = GetComponent<Animator>();

        idleDuration = Random.Range(1f, 3f);
        walkDuration = Random.Range(2f, 5f);
        SetState(State.Idle);
        SetRandomWalkDirection();
        
    }

    void SetRandomWalkDirection()
    {
        float angle = Random.Range(0f, 360f);
        walkDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
    }

    void SetState(State newState)
    {
    
        if(currentState == newState)
            return;
            
        currentState = newState;
        stateTimer = 0f;
        switch(newState)
        {
            case State.Idle:
                animator.SetInteger("animState", 0); // idle
                break;
            case State.Walk:
                animator.SetInteger("animState", 1); // walk
                break;
            case State.Run:
                animator.SetInteger("animState", 2); // run
                break;
            case State.Catch:
                animator.SetInteger("animState", 3); // catch
                break;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // catch and run if player is detected
        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= catchDistance)
                SetState(State.Catch);
            else
                SetState(State.Run);
        }
        else
        {
            // if player is out of range, then go back to idle
            if (currentState == State.Run || currentState == State.Catch)
            {
                idleDuration = Random.Range(1f, 3f);
                SetState(State.Idle);
                SetRandomWalkDirection();
            }
            else
            {
                
                stateTimer += Time.deltaTime;
                if (currentState == State.Idle)
                {
                    if (stateTimer >= idleDuration)
                    {
                        SetState(State.Walk);
                        walkDuration = Random.Range(2f, 5f);
                    }
                }
                else if (currentState == State.Walk)
                {
                    // walk randomly
                    transform.Translate(walkDirection * walkSpeed * Time.deltaTime, Space.World);
                    
                    if (walkDirection != Vector3.zero)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(walkDirection), Time.deltaTime * 2f);

                    if (stateTimer >= walkDuration)
                    {
                        idleDuration = Random.Range(1f, 3f);
                        SetState(State.Idle);
                        SetRandomWalkDirection(); 
                    }
                }
            }
        }

        // 处理追逐和捕捉状态
        if(currentState == State.Run)
        {
            // 追逐玩家
            Vector3 direction = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
            transform.Translate(Vector3.forward * runSpeed * Time.deltaTime);
        }
        else if(currentState == State.Catch)
        {
            // 捕捉状态下可以停止移动，或加入捕捉的其他逻辑
            // 此处仅设置动画为捕捉状态
        }
    }
}
