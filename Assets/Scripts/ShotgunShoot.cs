using UnityEngine;

public class ShotgunShoot : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public int pelletsPerShot = 8;
    public float spreadAngle = 15f;
    public float fireRate = 1f;

    [Header("Aiming Settings")]
    public Transform rightController;
    public Vector3 positionOffset = new Vector3(0, -0.05f, 0.1f);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [Header("Optional Effects")]
    public AudioClip shootSound;
    public GameObject muzzleFlashEffect;

    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        // Find right controller if not assigned
        if (rightController == null)
        {
            GameObject controllerObj = GameObject.Find("RightControllerAnchor");
            if (controllerObj == null)
            {
                controllerObj = GameObject.Find("RightHandAnchor");
            }
            if (controllerObj != null)
            {
                rightController = controllerObj.transform;
            }
            else
            {
                Debug.LogError("Could not find right controller!");
            }
        }

        // Set up audio
        if (shootSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = shootSound;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Update weapon position to follow controller
        FollowController();

        // Shoot when trigger pressed (single shot per press for shotgun)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FollowController()
    {
        if (rightController == null) return;

        // Direct follow - no jitter
        Vector3 targetPosition = rightController.position + rightController.TransformDirection(positionOffset);
        Quaternion targetRotation = rightController.rotation * Quaternion.Euler(rotationOffset);

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    void Shoot()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogError("Bullet prefab or spawn point not assigned!");
            return;
        }

        // Fire multiple pellets in a spread
        for (int i = 0; i < pelletsPerShot; i++)
        {
            // Calculate random spread
            float randomX = Random.Range(-spreadAngle, spreadAngle);
            float randomY = Random.Range(-spreadAngle, spreadAngle);

            Quaternion spreadRotation = bulletSpawnPoint.rotation * Quaternion.Euler(randomX, randomY, 0);

            // Spawn pellet
            GameObject pellet = Instantiate(bulletPrefab, bulletSpawnPoint.position, spreadRotation);

            Debug.Log("Pellet " + i + " spawned with rotation: " + spreadRotation.eulerAngles);
        }

        Debug.Log("Shotgun fired! " + pelletsPerShot + " pellets");

        // Effects
        if (audioSource != null && shootSound != null)
        {
            audioSource.Play();
        }

        if (muzzleFlashEffect != null)
        {
            GameObject flash = Instantiate(muzzleFlashEffect, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Destroy(flash, 0.1f);
        }

        // Stronger haptic feedback for shotgun
        OVRInput.SetControllerVibration(0.8f, 0.2f, OVRInput.Controller.RTouch);
    }
}