using UnityEngine;

public class PlayerYSort : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float feetY = transform.position.y - sr.bounds.extents.y;
        sr.sortingOrder = Mathf.RoundToInt(-feetY * 10);
    }
}