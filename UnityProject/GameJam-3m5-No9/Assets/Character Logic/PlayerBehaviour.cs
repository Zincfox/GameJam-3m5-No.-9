using UnityEngine;

[ExecuteAlways]
public class PlayerBehaviour : EntityBehaviour
{
    public MapMangler.Entities.Player Entity { get; private set; }

    protected override void Start()
    {
        base.Start();

        var id = gameObject.GetInstanceID();
        Entity = new MapMangler.Entities.Player(id);
    }
}
