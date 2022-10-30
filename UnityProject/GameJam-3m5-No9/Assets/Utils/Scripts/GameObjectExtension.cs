using System;
using UnityEngine;
using UnityEngine.Assertions;

public static class GameObjectExtension
{
    public static GameObject FindChildByName(this GameObject parent, string childName)
    {
        foreach (Transform childTransform in parent.transform)
        {
            if (childTransform.name == childName)
            {
                return childTransform.gameObject;
            }
        }
        throw new Exception($"Could not find child by name: {childName}");
    }

    public static void BindComponent<TComponent>(this GameObject go, out TComponent component)
        where TComponent : Component
    {
        component = go.GetComponentInChildren(typeof(TComponent)) as TComponent;
        Assert.IsNotNull(component, $"Could not find component of type ${typeof(TComponent)} on {go.name}");
    }
}
