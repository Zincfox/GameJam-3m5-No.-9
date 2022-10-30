public class EnemyBehaviour : EntityBehaviour
{
    private MapMangler.Difficulty.NPC_AI ai;

    protected override void Awake()
    {
        base.Awake();

        var id = gameObject.GetInstanceID();
        Entity = new MapMangler.Entities.NPC(id);
    }

    protected override void Start()
    {
        base.Start();

        GameMaster gm = FindObjectOfType<GameMaster>();
        if (gm == null) throw new System.Exception("Could not locate GameMaster in scene");
        gm.GameStateReadyEvent += (gs, _) =>
        {
            var gameState = (MapMangler.GameState)gs;

            ai = new MapMangler.Difficulty.NPC_AI(gameState, (MapMangler.Entities.NPC)Entity);
        };
    }

    public System.Collections.IEnumerator MakeNPCTurn()
    {
        ai.StartTurn();
        do
        {
            yield return null;
            var nextAction = ai.ObtainNextAction();
            if (nextAction == null) break;
            if (nextAction is MapMangler.Actions.MoveAction moveAction)
            {
                var stepper = moveAction.GetStepper();
                while (stepper())
                {
                    yield return null;
                }
            } else
            {
                nextAction.Perform();
            }
        } while (true);
    }


}
