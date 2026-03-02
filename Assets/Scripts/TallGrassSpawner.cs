using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TallGrass_Spawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject tallGrassPrefab;

    [Header("Spawn Einstellungen")]
    public int grassCount = 30;
    public Vector2 spawnAreaMin;
    public Vector2 spawnAreaMax;
    public Tilemap[] grassTilemaps;

    [Header("Abstand zwischen Gras")]
    public float minDistance = 0.8f;

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
        SpawnGrass();
    }

    void SpawnGrass()
    {
        int spawned = 0;
        int maxAttempts = grassCount * 20;
        int attempts = 0;

        while (spawned < grassCount && attempts < maxAttempts)
        {
            attempts++;
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (!IsInForbiddenZone(randomPos) && !IsTooClose(randomPos))
            {
                GameObject grass = Instantiate(tallGrassPrefab, randomPos, Quaternion.identity);

                SpriteRenderer sr = grass.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    float feetY = randomPos.y - sr.bounds.extents.y;
                    sr.sortingOrder = Mathf.RoundToInt(-feetY * 10);
                }

                spawnedPositions.Add(randomPos);
                spawned++;
            }
        }

        Debug.Log($"{spawned} Tall Grass gespawnt nach {attempts} Versuchen");
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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2;
        Vector2 size = spawnAreaMax - spawnAreaMin;
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red;
        foreach (ForbiddenZone zone in forbiddenZones)
            Gizmos.DrawWireCube(zone.center, zone.size);
    }
}