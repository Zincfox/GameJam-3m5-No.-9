using UnityEngine;

public class AtackableScript : MonoBehaviour
{
    [SerializeField]
    private GameObject target;

    private GameMaster gameMaster;

    private MapMangler.Entities.Entity targetEntity;

    private void Awake()
    {
        gameMaster = GameObject.FindObjectOfType<GameMaster>();
        if (gameMaster == null) throw new System.Exception("Could not find GameMaster in Scene");
        var entityBehaviour = target.GetComponent<EntityBehaviour>();
        if (entityBehaviour != null) throw new System.Exception($"Could not find EntityBehaviour on {targetEntity}");
        gameMaster.GameStateReadyEvent += (_, _) =>
        {
            targetEntity = entityBehaviour.Entity;
        };
    }

    private void OnMouseDown()
    {
        var attempt = gameMaster.ActivePlayer.Entity.AttemptAttack(targetEntity);
        if(attempt != null)
        {
            attempt.Perform();
        }
    }
}
