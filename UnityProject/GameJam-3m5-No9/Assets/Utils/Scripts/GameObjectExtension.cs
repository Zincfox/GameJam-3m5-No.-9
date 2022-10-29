using UnityEngine;
using System.Collections;

public static class GameObjectExtension
{
    public static GameObject FindChildByName(this GameObject parent, string childName)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }
        }

        return null;
    }
}
