using UnityEngine;
using UnityEngine.UI;

public class InventoryTabManager : MonoBehaviour
{
    [Header("1. Die Seiten (Aus Hierarchie)")]
    public GameObject backpackPage;
    public GameObject encycloPage;

    [Header("2. Die Reiter/Tabs (Aus Hierarchie)")]
    public Image backpackTabImage; // Das Objekt "BackpackTab"
    public Image encycloTabImage;  // Das Objekt "EncycloTab"

    [Header("3. Reiter-Grafiken (Aus Projekt-Ordner)")]
    public Sprite tabActiveSprite;   // Bild: Reiter ist hell/ausgewählt
    public Sprite tabInactiveSprite; // Bild: Reiter ist dunkel/im Hintergrund

    [Header("4. Der große Hintergrund (Aus Hierarchie)")]
    public Image mainBackgroundImage; // Das Objekt "Body" oder "InventoryBox"

    [Header("5. Hintergrund-Grafiken (Aus Projekt-Ordner)")]
    public Sprite backpackBgSprite;  // Bild: Rucksack-Papier
    public Sprite encycloBgSprite;   // Bild: Kombinier-Papier

    void Start()
    {
        // Beim Start direkt Rucksack-Ansicht laden
        ShowBackpack();
    }

    public void ShowBackpack()
    {
        // Seiten umschalten
        if (backpackPage != null) backpackPage.SetActive(true);
        if (encycloPage != null) encycloPage.SetActive(false);

        // Reiter (Tabs) updaten
        if (backpackTabImage != null) backpackTabImage.sprite = tabActiveSprite;
        if (encycloTabImage != null) encycloTabImage.sprite = tabInactiveSprite;

        // Großen Hintergrund updaten
        if (mainBackgroundImage != null && backpackBgSprite != null)
            mainBackgroundImage.sprite = backpackBgSprite;
    }

    public void ShowEncyclopedia()
    {
        // Seiten umschalten
        if (backpackPage != null) backpackPage.SetActive(false);
        if (encycloPage != null) encycloPage.SetActive(true);

        // Reiter (Tabs) updaten
        if (backpackTabImage != null) backpackTabImage.sprite = tabInactiveSprite;
        if (encycloTabImage != null) encycloTabImage.sprite = tabActiveSprite;

        // Großen Hintergrund updaten
        if (mainBackgroundImage != null && encycloBgSprite != null)
            mainBackgroundImage.sprite = encycloBgSprite;
    }
}