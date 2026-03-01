using UnityEngine;

public enum Tool
{
    None = -1,
    
    // --- Standard Aktion ---
    Hand = 0,

    // --- Basis Werkzeuge ---
    Hoe = 1,          // umgraben
    WateringCan = 2,  // gießen
    Axe = 3,          // Bäume fällen
    Pickaxe = 4,      // Felsen abbauen
    Sickle = 5,       // Gras/Ernte schneiden

    // --- Upgrade Werkzeuge (Holz) ---
    HoeWood = 10,
    SpadeWood = 11,   // Neu: Holzspaten
    AxeWood = 12,
    PickaxeWood = 13,
    SickleWood = 14,

    // --- Upgrade Werkzeuge (Dense / Crystal) ---
    HoeDense = 20,
    AxeDense = 21,
    AxeCrystal = 22,
    PickaxeDense = 23,
    SickleDense = 24,

    // --- Items & Ressourcen ---
    Seeds = 50,       // pflanzen (stackbar)
    Wood = 51,        // Ressource (stackbar)
    Stone = 52        // Ressource (stackbar)
}