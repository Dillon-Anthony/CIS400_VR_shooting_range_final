using UnityEngine;

public class BowShoot : MonoBehaviour
{
    [Header("Arrow Settings")]
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float maxDrawStrength = 30f;
    public float drawTime = 1f;

    [Header("Aiming Settings")]
    public Transform rightController;
    public Vector3 positionOffset = new Vector3(0, -0.05f, 0.1f);
    public Vector3 rotationOffset = new Vector3(0, 0, 0);

    [Header("Optional Effects")]
    public AudioClip drawSound;
    public AudioClip releaseSound;

    private float currentDrawStrength = 0f;
    private bool isDrawing = false;
    private float drawStartTime = 0f;
    private AudioSource audioSource;
    private GameObject currentArrow;

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
        if (drawSound != null || releaseSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        // Update bow position to follow controller
        FollowController();

        // Start drawing - hold grip button
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            StartDraw();
        }

        // Continue drawing - calculate strength
        if (isDrawing && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            UpdateDraw();
        }

        // Release arrow - let go of grip
        if (isDrawing && OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            ReleaseArrow();
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

    void StartDraw()
    {
        isDrawing = true;
        drawStartTime = Time.time;
        currentDrawStrength = 0f;

        // Spawn visual arrow
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            currentArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
            Rigidbody arrowRb = currentArrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = true; // Don't let it fall while drawing
            }
            currentArrow.transform.SetParent(arrowSpawnPoint); // Attach to bow
        }

        // Play draw sound
        if (audioSource != null && drawSound != null)
        {
            audioSource.clip = drawSound;
            audioSource.Play();
        }

        Debug.Log("Started drawing bow...");
    }

    void UpdateDraw()
    {
        // Calculate draw strength based on time held
        float drawProgress = (Time.time - drawStartTime) / drawTime;
        drawProgress = Mathf.Clamp01(drawProgress); // 0 to 1
        currentDrawStrength = drawProgress * maxDrawStrength;

        // Haptic feedback increases with draw
        OVRInput.SetControllerVibration(drawProgress * 0.3f, 0.1f, OVRInput.Controller.RTouch);

        // Debug draw strength
        if (Time.frameCount % 30 == 0) // Log every 30 frames
        {
            Debug.Log("Draw strength: " + currentDrawStrength.ToString("F1") + " / " + maxDrawStrength);
        }
    }

    void ReleaseArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null)
        {
            Debug.LogError("Arrow prefab or spawn point not assigned!");
            isDrawing = false;
            return;
        }

        // Use the visual arrow we created, or create new one
        GameObject arrow;
        if (currentArrow != null)
        {
            arrow = currentArrow;
            arrow.transform.SetParent(null); // Detach from bow
            Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();
            if (arrowRb != null)
            {
                arrowRb.isKinematic = false; // Enable physics
            }
        }
        else
        {
            arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        }

        // Launch arrow with draw strength
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = arrowSpawnPoint.up * currentDrawStrength;
            rb.useGravity = true; // Arrows arc with gravity
        }

        Debug.Log("Released arrow with strength: " + currentDrawStrength.ToString("F1"));

        // Play release sound
        if (audioSource != null && releaseSound != null)
        {
            audioSource.clip = releaseSound;
            audioSource.Play();
        }

        // Strong haptic feedback on release
        OVRInput.SetControllerVibration(0.7f, 0.15f, OVRInput.Controller.RTouch);

        // Reset
        isDrawing = false;
        currentArrow = null;
        currentDrawStrength = 0f;
    }
}