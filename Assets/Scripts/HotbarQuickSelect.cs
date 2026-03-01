using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // neues Input System
#endif

public class HotbarQuickSelect : MonoBehaviour
{
    public ToolManager tools;

    [Header("Controls")]
    public bool useNumberKeys = true;   // 1..7 direkt wählen
    public bool useScroll = true;       // Maus/Trackpad
    public bool useTabCycle = true;     // Tab=next, Shift+Tab=prev (falls du willst)
    public bool wrap = true;            // am Ende wieder zum Anfang
    public bool invertScroll = false;   // falls Scroll-Richtung „falsch“ ist
    public float scrollThreshold = 0.1f; // setz auf 0.01 bei Mac-Trackpad

    void Awake()
    {
        if (!tools) tools = FindFirstObjectByType<ToolManager>();
    }

    void Update()
    {
        if (!tools) return;
        int count = tools.HotbarEntries?.Count ?? 0;
        if (count == 0) return;
        int max = Mathf.Min(7, count);

        // 1..7
        if (useNumberKeys)
        {
            for (int i = 0; i < max; i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { tools.index = i; return; }
        }

        // Tab / Shift+Tab (E bleibt frei!)
        if (useTabCycle && Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) Prev(count);
            else Next(count);
            return;
        }

        // Scroll (altes + neues Input)
        if (useScroll)
        {
            float d = 0f;
            #if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) d = Mouse.current.scroll.ReadValue().y;
            #endif
            #if ENABLE_LEGACY_INPUT_MANAGER
            if (Mathf.Abs(d) < 0.001f) d = Input.mouseScrollDelta.y;
            #endif

            if (invertScroll) d = -d;

            if (d > scrollThreshold)      Prev(count);
            else if (d < -scrollThreshold) Next(count);
        }
    }

    void Next(int count)
    {
        if (wrap) tools.index = (tools.index + 1) % count;
        else tools.index = Mathf.Min(count - 1, tools.index + 1);
    }

    void Prev(int count)
    {
        if (wrap) tools.index = (tools.index - 1 + count) % count;
        else tools.index = Mathf.Max(0, tools.index - 1);
    }
}