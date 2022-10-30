using UnityEngine;

public class EnemyBehaviour : EntityBehaviour
{
    public MapMangler.Entities.NPC Entity { get; private set; }

    protected override void Start()
    {
        base.Start();

        var id = gameObject.GetInstanceID();
        Entity = new MapMangler.Entities.NPC(id);
    }
}
