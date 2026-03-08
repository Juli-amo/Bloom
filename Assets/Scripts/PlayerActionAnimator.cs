using System.Collections;
using UnityEngine;

/// <summary>
/// Spielt Aktions-Animationen (Gießen, Axt, Sichel) in der richtigen
/// Himmelsrichtung ab und sperrt kurz die Bewegung.
/// </summary>
[RequireComponent(typeof(PlayerAnimDriver))]
[RequireComponent(typeof(PlayerController2D))]
public class PlayerActionAnimator : MonoBehaviour
{
    [Tooltip("Wie lange (Sekunden) die Bewegung nach einer Aktion gesperrt bleibt. " +
             "Sollte der Länge des längsten Action-Clips entsprechen.")]
    public float lockDuration = 0.5f;

    // ── interne Referenzen ────────────────────────────────────────────────
    Animator            anim;
    PlayerController2D  movement;
    bool                isActing;

    // ── ActionType-Konstanten (passen zu den Animator-Integer-Werten) ─────
    const int ACTION_WATERING = 1;
    const int ACTION_AXE      = 2;
    const int ACTION_SICKLE   = 3;

    void Awake()
    {
        anim     = GetComponent<Animator>();
        movement = GetComponent<PlayerController2D>();
    }

    // ── Öffentliche API ───────────────────────────────────────────────────

    /// <summary>
    /// Wird von Interaction.cs aufgerufen, bevor interactable.Interact() läuft.
    /// </summary>
    public void OnInteract(Tool tool)
    {
        int actionType = ToolToActionType(tool);
        if (actionType == 0) return;          // kein visuelles Feedback nötig

        if (isActing) StopAllCoroutines();    // vorherige Aktion abbrechen
        StartCoroutine(PlayAction(actionType));
    }

    // ── Interne Logik ─────────────────────────────────────────────────────

    IEnumerator PlayAction(int actionType)
    {
        isActing = true;

        // Bewegung sperren
        movement.enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        // FaceX/FaceY sind bereits korrekt gesetzt durch PlayerAnimDriver.
        // Wir setzen nur ActionType + feuern den Trigger.
        anim.SetInteger("ActionType", actionType);
        anim.SetTrigger("Action");

        yield return new WaitForSeconds(lockDuration);

        // Bewegung wieder freigeben
        anim.SetInteger("ActionType", 0);
        movement.enabled = true;
        isActing = false;
    }

    int ToolToActionType(Tool tool)
    {
        switch (tool)
        {
            case Tool.WateringCan: return ACTION_WATERING;
            case Tool.Axe:         return ACTION_AXE;
            case Tool.Sickle:      return ACTION_SICKLE;
            default:               return 0;
        }
    }
}
