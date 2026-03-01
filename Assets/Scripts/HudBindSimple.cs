using UnityEngine;
using TMPro;

public class HudBindSimple : MonoBehaviour
{
    public TextMeshProUGUI txt;

    void Start(){
        if (GameModel.I != null) GameModel.I.OnChange += UpdateHud;
        UpdateHud();
    }
    void OnDestroy(){
        if (GameModel.I != null) GameModel.I.OnChange -= UpdateHud;
    }
    void UpdateHud(){
        if (GameModel.I == null || txt == null) return;
        string water = GameModel.I.hasCan ? "Can" : "—";
        txt.text = $"Seeds: {GameModel.I.seeds}   |   Wood: {GameModel.I.wood}   |   Water: {water}";
    }
}