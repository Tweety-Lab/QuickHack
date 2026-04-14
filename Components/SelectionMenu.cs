using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ThunderRoad;
using QuickHack.Abilities;
using AliLib.Core.GC;

namespace QuickHack.Components;

/// <summary>
/// Component responsible for management of the Quick Hack selection menu.
/// </summary>
public class SelectionMenu : MonoBehaviour
{
    public class QuickHackInfo
    {
        public string Name { get; set; } = string.Empty;
        public string IconAddress { get; set; } = string.Empty;
    }

    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("UI", 2)]
    [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HighlightColorR = 0.40f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("UI", 2)]
    [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HighlightColorG = 0.75f;

    [ModOption(interactionType = ModOption.InteractionType.Slider)]
    [ModOptionCategory("UI", 2)]
    [ModOptionFloatValues(0f, 1f, 0.05f)]
    public static float HighlightColorB = 0.95f;

    /// <summary> All available quickhacks in the menu. </summary>
    public List<QuickHackInfo> Entries { get; set; } = new List<QuickHackInfo>();

    /// <summary> All QuickHack entries and their associated GameObject in the menu. </summary>
    public IReadOnlyDictionary<QuickHackInfo, GameObject> EntryInstances => entryInstances;

    private Dictionary<QuickHackInfo, GameObject> entryInstances = new Dictionary<QuickHackInfo, GameObject>();

    /// <summary> Index of the currently selected quickhack. </summary>
    public int SelectedIndex
    {
        get => field;
        set
        {
            field = value;
            UpdateSelection();
        }
    }

    public void Awake()
    {
        GameObject template = transform.Find("EntryTemplate").gameObject;
        SetInstanceVisibility(template, false);
    }

    /// <summary> Populates the menu with the contents of the <see cref="Entries"/> list. </summary>
    public void Populate()
    {
        GameObject template = transform.Find("EntryTemplate").gameObject;

        foreach (Transform child in transform)
            if (child.gameObject != template)
                GameObject.Destroy(child.gameObject);

        entryInstances.Clear();

        for (int i = 0; i < Entries.Count; i++)
        {
            GameObject entry = GameObject.Instantiate(template, transform);
            SetInstanceVisibility(entry, true);
            entry.transform.localPosition = template.transform.localPosition + Vector3.down * (i * 1f);
            entry.GetComponent<TextMeshPro>().text = Entries[i].Name;

            GameObject? icon = entry.transform.Find("Icon").gameObject;
            Catalog.LoadAssetAsync<Texture2D>(Entries[i].IconAddress, (texture) =>
            {
                if (icon == null)
                {
                    Catalog.ReleaseAsset(texture);
                    return;
                }

                MeshRenderer? renderer = icon.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    Catalog.ReleaseAsset(texture);
                    return;
                }

                SmartObject<Material> mat = renderer.material;
                mat.OnDisposed += () => Catalog.ReleaseAsset(texture);
                mat.Object?.mainTexture = texture;
            }, "QuickHack");

            entryInstances.Add(Entries[i], entry);
        }

        UpdateSelection();
    }

    /// <summary> Updates only the highlight. Call when selection changes. </summary>
    public void UpdateSelection()
    {
        if (SelectedIndex == -1)
            return;

        int i = 0;
        foreach (var kvp in entryInstances)
        {
            var highlight = kvp.Value.transform.Find("BackgroundHighlight").gameObject;
            highlight.GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f, 0.1f);
            if (i == SelectedIndex)
                highlight.GetComponent<SpriteRenderer>().color = new Color(HighlightColorR, HighlightColorG, HighlightColorB);
            i++;
        }
    }

    public void SetInstanceVisibility(GameObject instance, bool visible)
    {
        instance.GetComponent<MeshRenderer>()?.enabled = visible;
        foreach (Renderer r in instance.GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }
}
