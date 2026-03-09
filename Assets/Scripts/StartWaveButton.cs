using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartWaveButton : MonoBehaviour
{
    public TargetSpawner targetSpawner;
    public Material normalMaterial;
    public Material highlightMaterial;

    private Renderer buttonRenderer;
    private Color originalColor;
    private bool waveActive = false;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();

        //Store the original color
        if (buttonRenderer != null)
        {
            originalColor = buttonRenderer.material.color;
        }

        if (normalMaterial != null)
        {
            buttonRenderer.material = normalMaterial;
        }
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            Transform rightController = GameObject.Find("RightControllerAnchor").transform;

            RaycastHit hit;
            if (Physics.Raycast(rightController.position, rightController.forward, out hit, 10f))
            {
                if (hit.collider.gameObject == gameObject && !waveActive)
                {
                    StartWave();
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Controller"))
        {
            if (highlightMaterial != null && !waveActive)
            {
                buttonRenderer.material = highlightMaterial;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Controller"))
        {
            if (normalMaterial != null)
            {
                buttonRenderer.material = normalMaterial;
            }
        }
    }

    void StartWave()
    {
        Debug.Log("Wave Started!");
        waveActive = true;

        if (targetSpawner != null)
        {
            targetSpawner.StartWave();
        }

        //reset score
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.ResetScore();
        }

        // Disable button visually
        if (buttonRenderer != null)
        {
            Color disabledColor = Color.gray;
            buttonRenderer.material.color = disabledColor;
        }
    }

    public void ResetButton()
    {
        waveActive = false;

        // Restore original color
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = originalColor;
        }

        Debug.Log("Button reset and ready for next wave!");
    }
}