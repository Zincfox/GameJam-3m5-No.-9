using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct PlayerStats
{
    public Button rerollButton;
    public TMPro.TMP_Text remainingActionsLabel;
    public TMPro.TMP_Text remainingHealthLabel;
}

public class GameMaster : MonoBehaviour
{
    [SerializeField]
    private LevelClickHandler levelClickHandler;

    [SerializeField]
    private PlayerBehaviour[] players;

    [SerializeField]
    private EnemyBehaviour[] enemies;

    [SerializeField]
    private RoomSegment startRoomSegment;

    [SerializeField]
    private RoomSegment sampleTargetSegment;

    [SerializeField]
    private PlayerStats[] playerStats;

    private const float SecondsToMove = 2.0f;

    private List<EntityBehaviour> entities = new List<EntityBehaviour>();

    private MapMangler.GameState gameState = new MapMangler.GameState(MapMangler.Difficulty.DifficultyParameters.fromLevel(MapMangler.Difficulty.DifficultyLevel.EASY, 4));

    public PlayerBehaviour ActivePlayer { get; set; }

    private void OnEnable()
    {
        levelClickHandler.AreaClicked += LevelClickHandler_AreaClicked;

        var counter = 0;
        foreach(var player in playerStats)
        {
            int playerId = counter++;
            player.rerollButton.onClick.AddListener(() =>
            {
                OnRerollButtonClick(playerId);
            });
        }
    }

    private void OnDisable()
    {
        levelClickHandler.AreaClicked -= LevelClickHandler_AreaClicked;
        foreach (var player in playerStats)
        {
            player.rerollButton.onClick.RemoveAllListeners();
        }
    }

    private void Start()
    {
        //MapMangler.GameState.LOGGER = Debug.Log;
        foreach (var p in players)
        {
            p.Entity.Location = startRoomSegment.Segment;
            p.Entity.LocationChangeEvent += Entity_LocationChangeEvent;
            entities.Add(p);
            var space = startRoomSegment.GetNextFreeSpace();
            space.Owner = p;
        }

        foreach (var e in enemies)
        {
            e.Entity.Location = startRoomSegment.Segment;
            e.Entity.LocationChangeEvent += Entity_LocationChangeEvent;
            entities.Add(e);
        }
        gameState.RunSetup();

        //StartCoroutine(Test());
        //SelectPlayer(0);
    }

    private void OnDestroy()
    {
        foreach(var p in players)
        {
            p.Entity.LocationChangeEvent -= Entity_LocationChangeEvent;
            Debug.Log(p.Entity.Location);
        }

        foreach (var e in enemies)
        {
            e.Entity.LocationChangeEvent -= Entity_LocationChangeEvent;
        }
    }

    private void Entity_LocationChangeEvent(object sender, MapMangler.Entities.Entity.EntityValueChangeEventArgs<MapMangler.Rooms.RoomSegment> e)
    {
        /*var entity = e.entity;
        var from = e.from;
        var to = e.to;
        var script = entities.Find(script => script.Entity.Equals(e.entity));
        MoveAvatarToTargetLocation(script, from, to);*/
    }

    private IEnumerator Test()
    {
        yield return null;
        players[0].Entity.Location = sampleTargetSegment.Segment;
        yield return null;
        players[1].Entity.Location = sampleTargetSegment.Segment;
        yield return null;
        players[2].Entity.Location = sampleTargetSegment.Segment;
        yield return null;

        var entity = (MapMangler.Entities.Player)players[3].Entity;
        //entity.StartTurn(5);
        //var action = entity.AttemptMoveTo(sampleTargetSegment.Segment);
        //action.Perform();
        entity.Actions = 3;
        var moveAction = entity.AttemptMoveTo(sampleTargetSegment.Segment);
        var stepper = moveAction.GetStepper();
        Debug.Log(stepper.Invoke());
        Debug.Log(stepper.Invoke());
        Debug.Log(stepper.Invoke());
        Debug.Log(stepper.Invoke());
        Debug.Log(stepper.Invoke());
    }

    public void MoveAvatarToTargetLocation(EntityBehaviour entity, MapMangler.Rooms.RoomSegment from, MapMangler.Rooms.RoomSegment to)
    {
        Debug.Log(from + " [ " + to);
        if (RoomSegment.Lookup.TryGetValue(from, out var fromSegment)
            && RoomSegment.Lookup.TryGetValue(to, out var toSegment))
        {
            var start = fromSegment.FindOccupiedSpace(entity);
            if (start) start.Owner = null;

            var end = toSegment.GetNextFreeSpace();
            end.Owner = entity;

            StartCoroutine(MoveEntity(entity, start.transform.position, end.transform.position));
        }
    }

    private IEnumerator MoveEntity(EntityBehaviour entity, Vector3 startPos, Vector3 targetPos)
    {
        void SetEntityPosition(EntityBehaviour entity, Vector3 pos)
        {
            entity.transform.position = pos;
        }
        Debug.Log("MoveEntity " + startPos +" "+targetPos);
        var time = 0.0f;
        Vector3 pos;
        while (time < SecondsToMove)
        {
            time += Time.deltaTime;
            pos = Vector3.Lerp(startPos, targetPos, time / SecondsToMove);
            SetEntityPosition(entity, pos);
            yield return null;
        }
        pos = Vector3.Slerp(startPos, targetPos, time / SecondsToMove);
        SetEntityPosition(entity, pos);
    }

    public void SelectPlayer(int index)
    {
        if (index < 0 || index >= players.Length)
        {
            ActivePlayer = null;
            return;
        }

        ActivePlayer = players[index];
        //ActivePlayer.Entity.Actions = 3; // TODO
    }

    private void LevelClickHandler_AreaClicked()
    {
        var area = levelClickHandler.LastClicked;
        var segment = area.parent.GetComponent<RoomSegment>();

        if(ActivePlayer == null)
        {
            return;
        }

        StartCoroutine(MovePlayerToSelectedTarget(segment));
    }

    private IEnumerator MovePlayerToSelectedTarget(RoomSegment clickedSegment)
    {
        var player = ActivePlayer;
        var steps = player.Entity.Actions;
        Debug.Log(steps);

        var entity = ActivePlayer.Entity;
        var segment = clickedSegment.Segment;
        var moveAction = entity.AttemptMoveTo(segment);
        if (moveAction == null)
        {
            yield break;
        }

        var stepper = moveAction.GetStepper();
        if (stepper == null)
        {
            yield break;
        }


        var remainingSteps = Mathf.Max(steps - player.Entity.Actions, 0);
        Debug.Log(remainingSteps);
        var segmentList = moveAction.path.Elements;
        var pathCount = segmentList.Count -1;

        var limit = steps - remainingSteps;
        for (var index = 0; index < limit; ++index)
        {
            if (stepper.Invoke() == false)
            {
                yield break;
            }
            Debug.Log(index+" "+ pathCount);

            var from = segmentList[index];
            var to = segmentList[index + 1];

            if (RoomSegment.Lookup.TryGetValue(from, out var fromSegment)
                && RoomSegment.Lookup.TryGetValue(to, out var toSegment))
            {
                var start = fromSegment.FindOccupiedSpace(player);
                if (start) start.Owner = null;

                var end = toSegment.GetNextFreeSpace();
                end.Owner = player;

                Debug.Log(fromSegment.name + " " + toSegment.name);

                yield return MoveEntity(player, start.transform.position, end.transform.position);
                yield return new WaitForSeconds(0.5f);

                Debug.Log(player.Entity.Actions);
            }
        }

        Debug.Log(player.Entity.Actions +" --- "+ remainingSteps + " "+ (player.Entity.Actions == 1 && remainingSteps == 0));
        // Fix issues with non decreasing counter
        if (player.Entity.Actions == 1 && remainingSteps == 0)
        {
            //player.Entity.Actions = 0;
            (player.Entity as MapMangler.Entities.Player).EndTurn();
        }
    }

    private void OnRerollButtonClick(int playerIndex)
    {
        SelectPlayer(playerIndex);
        Debug.Log(
            players[0].Entity.Actions + " " +
            players[1].Entity.Actions + " " +
            players[2].Entity.Actions + " " +
            players[3].Entity.Actions);

        if (
             players[0].Entity.Actions > 0
             || players[1].Entity.Actions > 0
             || players[2].Entity.Actions > 0
             || players[3].Entity.Actions > 0)
        {
            return;
        }

        var actionCount = UnityEngine.Random.Range(1, 5);
        players[playerIndex].Entity.Actions = actionCount;
        playerStats[playerIndex].remainingActionsLabel.text = actionCount.ToString();
    }
}
