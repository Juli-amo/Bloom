using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HotbarBootstrap : MonoBehaviour
{
    [Header("References")]
    public ToolManager tools;   // im Inspector auf den ToolManager des Players setzen

    [Header("Layout")]
    public Vector2 referenceResolution = new(1920, 1080);
    public Vector2 size = new(560, 96);
    public float yOffset = 18f;
    public float slotSize = 64f;
    public float spacing = 6f;

    [System.Serializable]
    public class ToolIcon { public Tool tool; public Sprite sprite; }

    [Header("Icons")]
    public List<ToolIcon> iconSet;

    RectTransform slotContainer;
    readonly List<Image> highlights = new();
    readonly List<TextMeshProUGUI> countLabels = new();
    Tool[] lastTools = new Tool[0];
    float poll;

    void Start()
    {
        if (!tools) tools = FindFirstObjectByType<ToolManager>();
        BuildCanvasIfNeeded();
        Rebuild();
        if (tools) tools.OnChanged += Rebuild;
    }

    void OnDestroy() { if (tools) tools.OnChanged -= Rebuild; }

    void Update()
    {
        // Live-Highlight
        for (int i = 0; i < highlights.Count; i++)
            if (highlights[i]) highlights[i].gameObject.SetActive(i == tools.index);

        // Counts regelmäßig ziehen
        poll += Time.unscaledDeltaTime;
        if (poll >= 0.2f) { poll = 0f; UpdateCounts(); }
    }

    void BuildCanvasIfNeeded()
    {
        var canvas = FindFirstObjectByType<Canvas>();
        if (!canvas)
        {
            var go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.matchWidthOrHeight = 0.5f;
        }
        if (!FindFirstObjectByType<EventSystem>())
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        // alten Root entfernen
        var old = canvas.transform.Find("HotbarRoot");
        if (old) Destroy(old.gameObject);

        // Root
        var root = new GameObject("HotbarRoot", typeof(RectTransform));
        root.transform.SetParent(canvas.transform, false);
        var rt = root.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.sizeDelta = size;
        rt.anchoredPosition = new Vector2(0f, yOffset);

        // Hintergrund (RaycastTarget AN ist ok)
        var bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(root.transform, false);
        var bgImg = bg.GetComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.40f);
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;

        // Slots-Container
        var cont = new GameObject("SlotContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        cont.transform.SetParent(root.transform, false);
        slotContainer = cont.GetComponent<RectTransform>();
        slotContainer.anchorMin = Vector2.zero; slotContainer.anchorMax = Vector2.one;
        slotContainer.offsetMin = new Vector2(16, 12); slotContainer.offsetMax = new Vector2(-16, -12);

        var h = cont.GetComponent<HorizontalLayoutGroup>();
        h.spacing = spacing;
        h.childAlignment = TextAnchor.MiddleCenter;
        h.childControlWidth = true;  h.childControlHeight = true;
        h.childForceExpandWidth = false; h.childForceExpandHeight = false;
    }

    ToolIcon FindIcon(Tool t) => iconSet?.Find(ic => ic != null && ic.tool == t);

    public void Rebuild()
    {
        if (!tools || slotContainer == null) return;

        // Tool-Liste zum Vergleich
        var entries = tools.HotbarEntries;
        var arr = new Tool[entries.Count];
        for (int i = 0; i < entries.Count; i++) arr[i] = entries[i].tool;
        if (AreSame(arr, lastTools))
        {
            UpdateCounts();
            return;
        }
        lastTools = arr;

        // clear
        highlights.Clear(); countLabels.Clear();
        for (int i = slotContainer.childCount - 1; i >= 0; i--)
            Destroy(slotContainer.GetChild(i).gameObject);

        // build
        for (int i = 0; i < entries.Count; i++)
            CreateSlot(i, entries[i]);

        UpdateCounts();
    }

    bool AreSame(Tool[] a, Tool[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
        return true;
    }

    void CreateSlot(int index, ToolManager.Entry entry)
    {
        // Button-Root
        var slot = new GameObject($"Slot_{index}", typeof(Image), typeof(Button), typeof(LayoutElement));
        slot.transform.SetParent(slotContainer, false);

        var le = slot.GetComponent<LayoutElement>();
        le.preferredWidth = slotSize; le.preferredHeight = slotSize;

        var bg = slot.GetComponent<Image>();
        bg.color = new Color(1, 1, 1, 0.15f);
        bg.raycastTarget = true; // muss Events fangen

        var btn = slot.GetComponent<Button>();
        btn.targetGraphic = bg;  // wichtig für UI-States
        int captured = index;
        btn.onClick.AddListener(() => tools.index = captured);

        // Highlight (darf NICHT raycasten!)
        var hi = new GameObject("Highlight", typeof(Image));
        hi.transform.SetParent(slot.transform, false);
        var hiRt = hi.GetComponent<RectTransform>();
        hiRt.anchorMin = Vector2.zero; hiRt.anchorMax = Vector2.one;
        hiRt.offsetMin = Vector2.zero; hiRt.offsetMax = Vector2.zero;
        var hiImg = hi.GetComponent<Image>();
        hiImg.color = new Color(1, 1, 1, 0.35f);
        hiImg.raycastTarget = false; // <<< fix
        hi.SetActive(index == tools.index);
        highlights.Add(hiImg);

        // Icon (darf NICHT raycasten!)
        var iconGO = new GameObject("Icon", typeof(Image));
        iconGO.transform.SetParent(slot.transform, false);
        var iconRt = iconGO.GetComponent<RectTransform>();
        iconRt.anchorMin = Vector2.zero; iconRt.anchorMax = Vector2.one;
        iconRt.offsetMin = new Vector2(6, 6); iconRt.offsetMax = new Vector2(-6, -6);
        var iconImg = iconGO.GetComponent<Image>();
        iconImg.preserveAspect = true;
        iconImg.raycastTarget = false; // <<< fix
        var ic = FindIcon(entry.tool);
        if (ic != null && ic.sprite != null) iconImg.sprite = ic.sprite;

        // Count-Label (auch nicht raycasten)
        var numGO = new GameObject("Count", typeof(TextMeshProUGUI));
        numGO.transform.SetParent(slot.transform, false);
        var nrt = numGO.GetComponent<RectTransform>();
        nrt.anchorMin = new Vector2(1, 0); nrt.anchorMax = new Vector2(1, 0);
        nrt.pivot = new Vector2(1, 0);
        nrt.anchoredPosition = new Vector2(-4, 4); nrt.sizeDelta = new Vector2(44, 26);
        var num = numGO.GetComponent<TextMeshProUGUI>();
        num.alignment = TextAlignmentOptions.BottomRight;
        num.fontSize = 18; num.color = Color.white;
        num.raycastTarget = false; // <<< fix
        countLabels.Add(num);
    }

    void UpdateCounts()
    {
        var entries = tools?.HotbarEntries;
        if (entries == null) return;
        for (int i = 0; i < countLabels.Count && i < entries.Count; i++)
        {
            var e = entries[i];
            countLabels[i].text = (e.stackable && e.count > 0)
                ? Mathf.Min(e.count, 99).ToString()
                : "";
        }
    }
}