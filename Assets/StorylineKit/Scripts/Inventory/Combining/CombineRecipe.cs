using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Items/Combine Recipe")]
public class CombineRecipe : ScriptableObject
{
    public List<ItemAmount> inputs;
    public ItemType output;
    public int outputAmount = 1;
}

[System.Serializable]
public class ItemAmount
{
    public ItemType item;
    public int amount;
}
