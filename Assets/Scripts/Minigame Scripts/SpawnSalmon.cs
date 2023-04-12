using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnSalmon : MonoBehaviour
{
    public GameObject salmonPrefab;

    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public float initialSpawnInterval;
    public float finalSpawnInterval;
    public float gameTime;
    private float elapsedTime;
    private float spawnTimer;
    [HideInInspector] public float timeRemaining;

    public  TMP_Text timerText;

    private void Start()
    {
        spawnTimer = 0f;
        timeRemaining = gameTime;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        spawnTimer -= Time.deltaTime;

        timeRemaining = gameTime - elapsedTime;

        if (spawnTimer <= 0)
        {
            spawnTimer = spawnInterval();

            float x = Random.Range(xMin, xMax);
            float y = Random.Range(yMin, yMax);

           // Instantiate(salmonPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }

        UpdateTimerText();
    }

    private float spawnInterval()
    {
        float t = elapsedTime / gameTime;
        return Mathf.Lerp(initialSpawnInterval, finalSpawnInterval, t);
    }

    private void UpdateTimerText()
    {
        string formattedTime = timeRemaining.ToString("N0");
        timerText.text = formattedTime;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        Vector3 size = new Vector3(xMax - xMin, yMax - yMin, 0f);
        Gizmos.DrawWireCube(new Vector3(pos.x + (xMin + xMax) / 2f, pos.y + (yMin + yMax) / 2f, pos.z), size);
    }
}