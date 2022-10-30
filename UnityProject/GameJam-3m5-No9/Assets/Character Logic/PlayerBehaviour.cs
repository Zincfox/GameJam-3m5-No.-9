using UnityEngine;

public class PlayerBehaviour : EntityBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        var id = gameObject.GetInstanceID();
        Entity = new MapMangler.Entities.Player(id);
    }
}
