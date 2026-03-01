using UnityEngine;

public class Interaction : MonoBehaviour
{
    public LayerMask interactableMask;   // Layer „Interactable“ anhaken
    public float radius = 0.8f;          // Reichweite
    public Transform probe;              // optional: leer lassen = Player-Transform

    ToolManager tools;

    void Awake()
    {
        tools = FindFirstObjectByType<ToolManager>();
        if (!probe) probe = transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();
    }

    void TryInteract()
    {
        if (!tools) return;

        // alle Treffer in Reichweite holen
        var hits = Physics2D.OverlapCircleAll(probe.position, radius, interactableMask);
        if (hits == null || hits.Length == 0) return;

        // nächstes IInteractable suchen
        Collider2D best = null;
        float bestDist = float.MaxValue;
        foreach (var h in hits)
        {
            if (!h) continue;
            var ia = h.GetComponentInParent<IInteractable>();
            if (ia == null) continue;

            float d = Vector2.SqrMagnitude((Vector2)h.transform.position - (Vector2)probe.position);
            if (d < bestDist) { bestDist = d; best = h; }
        }
        if (best == null) return;

        var interactable = best.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            // Das ist der entscheidende Call:
            // -> tools.Current ist das in der Hotbar ausgewählte Item (Seeds, WateringCan, Hand, Hoe, Axe …)
            interactable.Interact(tools.Current);
        }
    }

    // nur zur Sichtbarkeit im Editor
    void OnDrawGizmosSelected()
    {
        var p = probe ? probe.position : transform.position;
        Gizmos.color = new Color(0,1,1,0.35f);
        Gizmos.DrawWireSphere(p, radius);
    }
}