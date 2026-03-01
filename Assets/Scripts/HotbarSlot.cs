using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HotbarSlot : MonoBehaviour {
    public Image icon;
    public GameObject highlight;
    public Button button;
    public TextMeshProUGUI label;
    [HideInInspector] public int index;
    public void SetSelected(bool sel){ if(highlight) highlight.SetActive(sel); }
}