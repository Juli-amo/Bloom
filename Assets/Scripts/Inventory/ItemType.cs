using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Type")]
public class ItemType : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public Sprite worldSprite;
    public int maxStackSize = 120;
}
