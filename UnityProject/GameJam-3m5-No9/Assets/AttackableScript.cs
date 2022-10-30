using UnityEngine;
using UnityEngine.EventSystems;

public class AttackableScript: MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField]
    private GameObject target;

    [SerializeField]
    private EntityBehaviour entityBehaviour;

    private GameMaster gameMaster;

    private MapMangler.Entities.Entity targetEntity;

    public void OnPointerClick(PointerEventData eventData)
    {
        // DO NOT REMOVE
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnMouseDown");
        var attempt = gameMaster.ActivePlayer.Entity.AttemptAttack(targetEntity);
        if (attempt != null)
        {
            attempt.Perform();
        }
    }

    private void Awake()
    {
        gameMaster = GameObject.FindObjectOfType<GameMaster>();
        if (gameMaster == null) throw new System.Exception("Could not find GameMaster in Scene");

        gameMaster.GameStateReadyEvent += (_, _) =>
        {
            targetEntity = entityBehaviour.Entity;
        };
    }
}
