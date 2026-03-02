using UnityEngine;

public class CombiningSlot : MonoBehaviour
{
    public ItemStack stack;
    public bool isOutput;

    public void Clear()
    {
        stack = null;
    }
}
