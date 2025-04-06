using UnityEngine.UI;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject patronusProjectile;
    public GameObject defaultProjectile;
    public float projectileSpeed = 100;
    public float spellRange = 20;

    public AudioClip spellSFX;

    [Header("Reticle Settings")]
    public Image reticleImage;
    //public Color targetColorDementor;
    public float animationSpeed = 5;

    //Color originalReticleColor;
    Vector3 originalReticleScale;
    GameObject currentProjectile;
    Color currentReticleColor;


    void Start()
    {
        //originalReticleColor = reticleImage.color;
        originalReticleScale = reticleImage.transform.localScale;
        if (defaultProjectile)
            currentProjectile = defaultProjectile;

       // currentReticleColor = Color.yellow;
        UpdateReticleColor();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void FixedUpdate()
    {
        if (!reticleImage)
        {
            return;
        }
        InteractiveEffect();

    }
    void Shoot()
    {
        if (currentProjectile)
        {
            GameObject spell = Instantiate(currentProjectile, transform.position + transform.forward, transform.rotation);

            Rigidbody rb = spell.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);
            }
            if (spellSFX)
            {
                AudioSource.PlayClipAtPoint(spellSFX, transform.position);
            }
            spell.transform.SetParent(transform);
        }
    }
   /*  void InteractiveEffect()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, spellRange))
        {
            Debug.Log("Hit something " + hit.collider.name);
            if (hit.collider.CompareTag("Boss"))
            {
                currentProjectile = patronusProjectile;
                UpdateReticleColor();
                ReticleAnimation(originalReticleScale / 2, currentReticleColor, animationSpeed);
            }
        }
        else
        {
            currentProjectile = defaultProjectile;
            UpdateReticleColor();
            ReticleAnimation(originalReticleScale, currentReticleColor, animationSpeed);
        }
    } */
    void InteractiveEffect()
{
    // Create a ray from the center of the screen
    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, spellRange))
    {
        Debug.Log("Hit something: " + hit.collider.name);
        if (hit.collider.CompareTag("Boss"))
        {
            // Switch to patronus projectile if not already set
            if (currentProjectile != patronusProjectile)
            {
                currentProjectile = patronusProjectile;
                UpdateReticleColor();
            }
            ReticleAnimation(originalReticleScale / 2, currentReticleColor, animationSpeed);
            return;
        }
    }
    // If nothing is hit or the object is not the Boss, revert to default projectile.
    if (currentProjectile != defaultProjectile)
    {
        currentProjectile = defaultProjectile;
        UpdateReticleColor();
    }
    ReticleAnimation(originalReticleScale, currentReticleColor, animationSpeed);
}

    void ReticleAnimation(Vector3 targetScale, Color targetColor, float speed)
    {
        var step = speed * Time.deltaTime;
        reticleImage.color = Color.Lerp(reticleImage.color, targetColor, step);
        reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale, targetScale, step);
    }

    void UpdateReticleColor()
    {
         currentReticleColor = currentProjectile.GetComponent<Renderer>().sharedMaterial.color;
    }
}

