using UnityEngine;
using System;
using System.Linq;

public enum Icon
{
    // Players
    CatWarrior1,
    CatWarrior2,
    CatWarrior3,
    CatWarrior4,
    Cowboy1,
    Military1,
    Military2,
    Worker1,
    Worker2,
    // Enemies
    Demon1,
    Demon2,
    // Other
    Door1,
    Action1,
}

[Serializable]
public struct Pair
{
    public Icon icon;
    public Sprite sprite;
}

[CreateAssetMenu(fileName = nameof(AvatarIcons), menuName = "ScriptableObjects/AvatarIcons", order = 1)]
public class AvatarIcons : ScriptableObject
{
    [SerializeField]
    private Pair[] data;

    public Sprite SelectSprite(Icon icon)
    {
        return data.First(it => it.icon == icon).sprite;
    }
}
