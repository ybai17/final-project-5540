using UnityEngine;

public class HRBehavior : MonoBehaviour
{

    public float movementSpeed = 1f;

    public float waitTime = 5f;

    public float damage = 100f;

    public Transform target;
    Vector3 targetGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        targetGrounded = target.position;
        targetGrounded.y = transform.position.y;

        transform.position = Vector3.MoveTowards(transform.position, targetGrounded, Time.deltaTime);

        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("HR Collided with " + collision.gameObject.name);
            collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }
}
