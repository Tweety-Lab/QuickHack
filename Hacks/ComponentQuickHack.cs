using ThunderRoad;
using UnityEngine;

namespace QuickHack.Hacks;

/// <summary>
/// Base Class for a <see cref="BaseQuickHack"/> that automatically targets a specific <see cref="MonoBehaviour"/>.
/// </summary>
public abstract class ComponentQuickHack<T> : BaseQuickHack where T : MonoBehaviour
{
    /// <inheritdoc/>
    public override bool CanHack(GameObject target)
    {
        T? component = target.GetComponent<T>() ?? target.GetComponentInParent<T>();
        return component != null;
    }

    /// <inheritdoc/>
    public override void Hack(GameObject target)
    {
        T component = target.GetComponent<T>() ?? target.GetComponentInParent<T>();
        Hack(component);
    }

    /// <summary> Apply the Quick Hack to the <typeparamref name="T"/>. </summary>
    public abstract void Hack(T target);

    /// <inheritdoc cref="CanHack"/>
    public virtual bool CanHack(T target) => true;
}
