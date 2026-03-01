using UnityEngine;
using UnityEngine.UI; 

public class SettingsMenu : MonoBehaviour
{
    [Header("Zieh hier das 'SettingsOverlay' rein")]
    public GameObject settingsPanel; 

    [Header("Zieh hier den Toggle aus dem Fenster rein")]
    public Toggle soundToggle;       

    void Start()
    {
        // Sicherstellen, dass Menü beim Start zu ist
        if(settingsPanel) settingsPanel.SetActive(false);

        // Sound Einstellung laden (falls Toggle zugewiesen)
        if(soundToggle)
        {
            soundToggle.isOn = AudioListener.volume > 0.5f;
            soundToggle.onValueChanged.AddListener(OnSoundChanged);
        }
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; // Spiel pausieren
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // Spiel weiterlaufen lassen
    }

    public void OnSoundChanged(bool isOn)
    {
        AudioListener.volume = isOn ? 1f : 0f;
    }
}