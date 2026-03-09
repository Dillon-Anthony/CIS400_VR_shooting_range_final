using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float lifetime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }
        else
        {
            Debug.LogError("Bullet missing Rigidbody!");
        }

        // Ignore collisions with other projectiles
        GameObject[] allProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject proj in allProjectiles)
        {
            if (proj != gameObject)
            {
                Collider projCollider = proj.GetComponent<Collider>();
                Collider myCollider = GetComponent<Collider>();
                if (projCollider != null && myCollider != null)
                {
                    Physics.IgnoreCollision(myCollider, projCollider);
                }
            }
        }

        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit: " + other.gameObject.name);

        // Check if we hit a target
        Target target = other.GetComponent<Target>();
        if (target != null)
        {
            Debug.Log("TARGET HIT!");
            target.Hit();
            Destroy(gameObject);
            return;
        }

        // Ignore weapon collisions
        if (other.name.Contains("Pistol") || other.name.Contains("Shotgun") || other.name.Contains("Bow"))
        {
            return;
        }

        // Ignore controller/hand collisions
        if (other.name.Contains("Controller") || other.name.Contains("Hand"))
        {
            return;
        }

        // Ignore spawn points
        if (other.name.Contains("SpawnPoint"))
        {
            return;
        }

        // Hit something else - destroy bullet
        Destroy(gameObject);
    }
}