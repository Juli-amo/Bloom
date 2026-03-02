using UnityEngine;
public class ObjectYSort : MonoBehaviour
{
    private SpriteRenderer sr;
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        float bottomY = sr.bounds.min.y;
        sr.sortingOrder = Mathf.RoundToInt(-bottomY * 10) + 5;
    }
}