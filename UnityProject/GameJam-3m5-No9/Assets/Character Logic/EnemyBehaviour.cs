public class EnemyBehaviour : EntityBehaviour
{
    protected override void Awake()
    {
        base.Awake();

        var id = gameObject.GetInstanceID();
        Entity = new MapMangler.Entities.NPC(id);
    }
}
