using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyExplosion : MonoBehaviour
{
    public GameObject itemPrefab; // The prefab of the item to spawn
    public int numberOfItems = 5; // Number of items to spawn
    public float spawnInterval = 0.5f; // Time between each item spawn
    public float destroyDelay = 5f; // Time before items are destroyed
    public Transform spawnOrigin; // The origin point for item spawns

    private bool hasSpawnedItems = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpawnedItems)
        {
            Debug.Log("Player triggered with " + gameObject.name);
            StartCoroutine(SpawnAndDestroyItems());
            hasSpawnedItems = true;
        }
    }

    private IEnumerator SpawnAndDestroyItems()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("Item prefab not set!");
            yield break;
        }

        if (spawnOrigin == null)
        {
            Debug.LogError("Spawn origin not set!");
            yield break;
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            Vector3 spawnPosition = spawnOrigin.position;
            Quaternion spawnRotation = Quaternion.identity;

            GameObject item = Instantiate(itemPrefab, spawnPosition, spawnRotation);
            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(destroyDelay);
        DestroyItems();
    }

    private void DestroyItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");

        foreach (GameObject item in items)
        {
            Destroy(item);
        }

        Debug.Log("Items destroyed!");
    }
}
