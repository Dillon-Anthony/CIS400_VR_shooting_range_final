using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public int targetsPerWave = 10;
    public float timeBetweenSpawns = 2f;
    public float targetLifetime = 3f; // How long target stays before despawning

    [Header("Spawn Area - Irregular Support")]
    public List<Transform> spawnPoints = new List<Transform>(); // Manual spawn points
    public bool useRandomArea = false; // Toggle between spawn points and random area
    public float randomSpawnRadius = 5f; // If using random area

    [Header("Target Prefab")]
    public GameObject targetPrefab;

    [Header("References")]
    public ScoreManager scoreManager;

    private int targetsSpawned = 0;
    private int targetsDestroyed = 0;
    private bool waveActive = false;

    public void StartWave()
    {
        if (!waveActive)
        {
            waveActive = true;
            targetsSpawned = 0;
            targetsDestroyed = 0;
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        while (targetsSpawned < targetsPerWave)
        {
            SpawnTarget();
            targetsSpawned++;
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    void SpawnTarget()
    {
        Vector3 spawnPosition;

        if (useRandomArea)
        {
            // Random spawn in circular area around this object
            Vector2 randomCircle = Random.insideUnitCircle * randomSpawnRadius;
            spawnPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        }
        else
        {
            // Use predefined spawn points for irregular areas
            if (spawnPoints.Count > 0)
            {
                Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                spawnPosition = randomPoint.position;
            }
            else
            {
                Debug.LogWarning("No spawn points assigned!");
                return;
            }
        }

        // Spawn the target
        Vector3 directionToPlayer = (Camera.main.transform.position - spawnPosition).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        GameObject target = Instantiate(targetPrefab, spawnPosition, targetRotation);


        // Set up the target
        Target targetScript = target.GetComponent<Target>();
        if (targetScript != null)
        {
            targetScript.spawner = this;
            targetScript.lifetime = targetLifetime;
        }
    }

    public void OnTargetDestroyed(bool wasHit)
    {
        targetsDestroyed++;

        if (wasHit && scoreManager != null)
        {
            scoreManager.AddScore(10); // 10 points per hit
        }

        // Check if wave is complete
        if (targetsDestroyed >= targetsPerWave)
        {
            EndWave();
        }
    }

    void EndWave()
    {
        waveActive = false;
        Debug.Log("Wave Complete!");

        if (scoreManager != null)
        {
            scoreManager.ShowFinalScore();
        }

        // Reset the start button so player can play again
        StartWaveButton startButton = FindObjectOfType<StartWaveButton>();
        if (startButton != null)
        {
            startButton.ResetButton();
        }
    }
}