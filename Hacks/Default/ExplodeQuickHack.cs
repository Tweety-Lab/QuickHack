using ThunderRoad;
using UnityEngine;
using static ThunderRoad.CreatureData;

namespace QuickHack.Hacks.Default;

/// <summary>
/// Explode a breakable object.
/// </summary>
public class ExplodeQuickHack : ComponentQuickHack<Item>
{
    /// <inheritdoc/>
    public override string Name { get; } = "Explode";

    /// <inheritdoc/>
    public override string Icon { get; } = "QuickHack.Icons.Explode";

    /// <inheritdoc/>
    public override bool CanHack(Item target) => target.breakable != null && target.breakable.CanBeBroken();

    /// <inheritdoc/>
    public override void Hack(Item target)
    {
        Vector3 center = target.Center;
        float radius = 3f;
        float force = 2f;

        target.breakable.Explode(force * 3f, center, radius, 0f, ForceMode.Impulse);

        EffectInstance? effect = Catalog.GetData<EffectData>("MeteorExplosion").Spawn(center, Quaternion.identity, target.transform, null, false);
        effect?.Play();

        foreach (Collider col in Physics.OverlapSphere(center, radius))
        {
            float distance = Vector3.Distance(center, col.transform.position);
            float falloff = 1f - (distance / radius);

            if (col.attachedRigidbody != null)
                col.attachedRigidbody.AddExplosionForce(force, center, radius, 1f, ForceMode.Impulse);

            Creature? creature = col.GetComponentInParent<Creature>();
            if (creature != null && creature.ragdoll != null)
            {
                if (!creature.isPlayer)
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized);

                creature.Damage(new CollisionInstance()
                {
                    damageStruct = new DamageStruct(DamageType.Energy, 10f * falloff)
                });

                foreach (RagdollPart part in creature.ragdoll.parts)
                    part.physicBody.AddExplosionForce(force, center, radius, 1f, ForceMode.Impulse);
            }
        }
    }
}