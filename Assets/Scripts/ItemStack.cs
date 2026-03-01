using UnityEngine;
[System.Serializable]
public class ItemStack
{
    public ItemType item;
    public int amount;

    public bool IsFull => amount >= item.maxStackSize;
}
