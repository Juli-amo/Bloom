using UnityEngine;
using System;

public class GameModel : MonoBehaviour
{
    public static GameModel I;
    void Awake(){ if (I){ Destroy(gameObject); return; } I=this; DontDestroyOnLoad(gameObject); }

    public int seeds = 3, coins = 0, wood = 0, stone = 0;
    public bool hasCan = false, hasAxe = false, hasPickaxe = false;

    public event Action OnChange;

    public void AddSeeds(int n){ seeds+=n; OnChange?.Invoke(); }
    public void AddCoins(int n){ coins+=n; OnChange?.Invoke(); }
    public void AddWood(int n){ wood+=n; OnChange?.Invoke(); }
    public void AddStone(int n){ stone+=n; OnChange?.Invoke(); }

    // ✅ NEU: von außen aufrufbar
    public void NotifyChange() => OnChange?.Invoke();
}