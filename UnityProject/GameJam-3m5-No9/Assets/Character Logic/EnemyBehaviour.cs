using UnityEngine;

public class EnemyBehaviour : EntityBehaviour
{
    private MapMangler.Difficulty.NPC_AI ai;

    protected override void Awake()
    {
        base.Awake();

        var id = gameObject.GetInstanceID();
        Entity = new MapMangler.Entities.NPC(id);
        GameMaster gm = FindObjectOfType<GameMaster>();
        UnityEngine.Debug.Log("Awake NPC", gm);
        if (gm == null) throw new System.Exception("Could not locate GameMaster in scene");
        gm.GameStateReadyEvent += (_, _) =>
        {
            ai = new MapMangler.Difficulty.NPC_AI(gm.GameState, (MapMangler.Entities.NPC)Entity);
        };
    }

    public System.Collections.IEnumerator MakeNPCTurn()
    {
        ai.StartTurn();
        do
        {
            yield return null;
            var nextAction = ai.ObtainNextAction();
            Debug.Log(nextAction);
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
