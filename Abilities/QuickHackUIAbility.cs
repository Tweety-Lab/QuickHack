using AliLib.Core;
using AliLib.Core.Abilities;
using AliLib.Core.Assets;
using QuickHack.Hacks;
using System.Collections;
using ThunderRoad;
using UnityEngine;

namespace QuickHack.Abilities;

/// <summary>
/// Handles the UI of the Quick Hack spell.
/// </summary>
public class QuickHackUIAbility : Ability
{
    [Addressable("QuickHack.HackBackground.Red")]
    public static GameObject? RedBackground { get; set; }

    [Addressable("QuickHack.Icons.BaseMat")]
    public static Material? BaseIconMaterial { get; set; }

    /// <inheritdoc/>
    public QuickHackUIAbility(AbilitySpell spell) : base(spell) { }


    /// <inheritdoc/>
    public override void Load()
    {
        base.Load();

         Spell.GetAbility<QuickHackLogicAbility>()?.OnQuickHackUsed += OnQuickHackUsed;
    }

    private void OnQuickHackUsed((BaseQuickHack QuickHack, GameObject Target) info) => SpawnQuickHackIcon(info.QuickHack.Icon, info.Target);

    public void SpawnQuickHackIcon(string iconAddress, GameObject target)
    {
        GameObject? instance = GameObject.Instantiate(RedBackground);

        if (BaseIconMaterial != null)
        {
            // TODO: Does this material properly get disposed?
            Material matInstance = GameObject.Instantiate(BaseIconMaterial);

            matInstance.SetColor("_Tint", new Color(1.0f, 0.3725f, 0.3725f));
            instance?.transform.Find("Icon").GetComponent<MeshRenderer>().material = matInstance;

            Catalog.LoadAssetAsync<Texture2D>(iconAddress, (text) => matInstance.SetTexture("_Sprite", text), "QuickHack");
        }

        // HACK: Ideally our billboard shader would handle rotation but since it doesn't we use a coroutine for positions instead of parenting
        if (target.TryGetComponent<Creature>(out Creature creature))
            CoroutineRunner.Instance.StartCoroutine(FollowPosition(instance!.transform, creature.ragdoll.GetPart(RagdollPart.Type.Torso).transform));
        else
            CoroutineRunner.Instance.StartCoroutine(FollowPosition(instance!.transform, target.transform));

        CoroutineRunner.Instance.PlayAfterDelay(() => GameObject.Destroy(instance), 3f);
    }

    private IEnumerator FollowPosition(Transform follower, Transform target, Vector3 localOffset = default)
    {
        while (follower != null && target != null)
        {
            follower.position = target.position + localOffset;
            follower.rotation = Quaternion.identity;
            yield return null;
        }
    }
}
