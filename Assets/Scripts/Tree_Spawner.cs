using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tree_Spawner : MonoBehaviour
{
    public GameObject treePrefab;
    public Sprite[] treeSprites;
    public int treeCount = 20;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public Tilemap[] grassTilemaps;

    [Header("Abstand zwischen Bäumen")]
    public float minDistance = 1.5f; // <-- Das hier im Inspector anpassen!

    [System.Serializable]
    public struct ForbiddenZone
    {
        public Vector2 center;
        public Vector2 size;
    }
    public ForbiddenZone[] forbiddenZones;

    private List<Vector2> spawnedPositions = new List<Vector2>(); // NEU

    void Start()
    {
        SpawnTrees();
    }

    void SpawnTrees()
    {
        int spawned = 0;
        int maxAttempts = treeCount * 20;
        int attempts = 0;

        while (spawned < treeCount && attempts < maxAttempts)
        {
            attempts++;
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (IsOnGrass(randomPos) && !IsInForbiddenZone(randomPos) && !IsTooClose(randomPos)) // NEU
            {
                GameObject tree = Instantiate(treePrefab, randomPos, Quaternion.identity);
                SpriteRenderer sr = tree.GetComponent<SpriteRenderer>();
                if (sr != null && treeSprites.Length > 0)
                    sr.sprite = treeSprites[Random.Range(0, treeSprites.Length)];

                spawnedPositions.Add(randomPos); // NEU
                spawned++;
            }
        }

        Debug.Log($"{spawned} Bäume gespawnt nach {attempts} Versuchen");
    }

    // NEU
    bool IsTooClose(Vector2 pos)
    {
        foreach (Vector2 existing in spawnedPositions)
        {
            if (Vector2.Distance(pos, existing) < minDistance)
                return true;
        }
        return false;
    }

    bool IsInForbiddenZone(Vector2 pos)
    {
        foreach (ForbiddenZone zone in forbiddenZones)
        {
            if (pos.x > zone.center.x - zone.size.x / 2 &&
                pos.x < zone.center.x + zone.size.x / 2 &&
                pos.y > zone.center.y - zone.size.y / 2 &&
                pos.y < zone.center.y + zone.size.y / 2)
                return true;
        }
        return false;
    }

    bool IsOnGrass(Vector2 worldPosition)
    {
        foreach (Tilemap tilemap in grassTilemaps)
        {
            Vector3Int cellPos = tilemap.WorldToCell(worldPosition);
            if (tilemap.HasTile(cellPos)) return true;
        }
        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2;
        Vector2 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red;
        foreach (ForbiddenZone zone in forbiddenZones)
            Gizmos.DrawWireCube(zone.center, zone.size);
    }
}