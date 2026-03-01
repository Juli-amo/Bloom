using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    [Header("Das Rucksack-Fenster")]
    public GameObject inventoryOverlay; 

    void Update()
    {
        // Wenn die Taste "T" gedrückt wird
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }

    // Diese Methode wird per Taste UND per Button aufgerufen
    public void ToggleInventory()
    {
        if (inventoryOverlay != null)
        {
            // activeSelf checkt: Ist es gerade an? 
            // Das Ausrufezeichen (!) dreht den Wert um. An wird Aus, Aus wird An.
            bool currentState = inventoryOverlay.activeSelf;
            inventoryOverlay.SetActive(!currentState);
        }
    }
}