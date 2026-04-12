using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ThunderRoad;

namespace QuickHack.Components;

/// <summary>
/// Component responsible for management of the Quick Hack selection menu.
/// </summary>
public class SelectionMenu : MonoBehaviour
{
    public class QuickHackInfo
    {
        public string Name {  get; set; }
        public string IconAddress { get; set; }
    }

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
            entry.transform.localPosition = template.transform.localPosition + Vector3.down * (i * 0.65f);
            entry.GetComponent<TextMeshPro>().text = Entries[i].Name;

            GameObject icon = entry.transform.Find("Icon").gameObject;
            Catalog.LoadAssetAsync<Texture2D>(Entries[i].IconAddress, (texture) =>
            {
                MeshRenderer renderer = icon?.GetComponent<MeshRenderer>();
                if (renderer == null) return;
                renderer.material.mainTexture = texture;
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
            var tmp = kvp.Value.GetComponent<TextMeshPro>();
            tmp.color = (i == SelectedIndex) ? Color.white : Color.gray;
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
