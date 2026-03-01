using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Butterfly_Spawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject butterflyPrefab;

    [Header("Spawn Einstellungen")]
    public int butterflyCount = 10;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public Tilemap[] grassTilemaps;

    [Header("Abstand zwischen Butterflies")]
    public float minDistance = 1f;

    [System.Serializable]
    public struct ForbiddenZone
    {
        public Vector2 center;
        public Vector2 size;
    }
    public ForbiddenZone[] forbiddenZones;

    private List<Vector2> spawnedPositions = new List<Vector2>();

    void Start()
    {
        SpawnButterflies();
    }

    void SpawnButterflies()
    {
        int spawned = 0;
        int maxAttempts = butterflyCount * 20;
        int attempts = 0;

        while (spawned < butterflyCount && attempts < maxAttempts)
        {
            attempts++;
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (IsOnGrass(randomPos) && !IsInForbiddenZone(randomPos) && !IsTooClose(randomPos))
            {
                Instantiate(butterflyPrefab, randomPos, Quaternion.identity);
                spawnedPositions.Add(randomPos);
                spawned++;
            }
        }

        Debug.Log($"{spawned} Butterflies gespawnt nach {attempts} Versuchen");
    }

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