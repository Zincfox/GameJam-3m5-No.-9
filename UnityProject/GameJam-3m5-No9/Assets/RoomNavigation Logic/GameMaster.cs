using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    private MapMangler.Difficulty.DifficultyLevel difficultyLevel;

    private const float SecondsToMove = 2.0f;

    private List<EntityBehaviour> entities = new List<EntityBehaviour>();

    public MapMangler.GameState GameState { get; private set; }

    private int activePlayerIndex;

    private TurnController turnController = new TurnController();

    private void Awake()
    {
        GameState = new MapMangler.GameState(MapMangler.Difficulty.DifficultyParameters.fromLevel(difficultyLevel, 4));
    }

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

        turnController.AllPlayerTurnsFinished += TurnController_AllPlayerTurnsFinished;
        turnController.EnemyTurnFinshed += TurnController_EnemyTurnFinshed;
    }

    private void OnDisable()
    {
        levelClickHandler.AreaClicked -= LevelClickHandler_AreaClicked;
        foreach (var player in playerStats)
        {
            player.rerollButton.onClick.RemoveAllListeners();
        }

        turnController.AllPlayerTurnsFinished -= TurnController_AllPlayerTurnsFinished;
        turnController.EnemyTurnFinshed -= TurnController_EnemyTurnFinshed;
    }

    private void Start()
    {
        //MapMangler.GameState.LOGGER = Debug.Log;
        foreach (var p in players)
        {
            p.Entity.Location = startRoomSegment.Segment;
            //p.Entity.LocationChangeEvent += Entity_LocationChangeEvent;
            p.Entity.HealthChangeEvent += Player_HealthChangeEvent;
            p.Entity.ActionsChangeEvent += Player_ActionsChangeEvent;
            entities.Add(p);
            var space = startRoomSegment.GetNextFreeSpace();
            space.Owner = p;
        }

        foreach (var e in enemies)
        {
            e.Entity.Location = startRoomSegment.Segment;
            e.Entity.LocationChangeEvent += Enemy_LocationChangeEvent;
            entities.Add(e);
        }
        SetupGameState();
        //StartCoroutine(Test());
        //SelectPlayer(0);
    }

    private ref PlayerStats GetPlayerStats(MapMangler.Entities.Player player)
    {
        for (int i = 0; i < playerStats.Length;i++)
        {
            if (players[i].Entity == player)
            {
                return ref playerStats[i];
            }
        }
        throw new Exception($"Could not find GameObject-Player for Logic-Player {player.entityID}");
    }

    private int GetPlayerStatsIndex(MapMangler.Entities.Player player)
    {
        for (int i = 0; i < playerStats.Length; i++)
        {
            if (players[i].Entity == player)
            {
                return i;
            }
        }
        throw new Exception($"Could not find GameObject-Player for Logic-Player {player.entityID}");
    }

    private void Player_HealthChangeEvent(object sender, MapMangler.Entities.Entity.EntityValueChangeEventArgs<int> e)
    {
        GetPlayerStats((MapMangler.Entities.Player)e.entity).remainingHealthLabel.text = e.to.ToString();
    }

    private void Player_ActionsChangeEvent(object sender, MapMangler.Entities.Entity.EntityValueChangeEventArgs<int> e)
    {
        GetPlayerStats((MapMangler.Entities.Player)e.entity).remainingActionsLabel.text = e.to.ToString();

        if (e.entity.Actions == 0)
        {
            var index = GetPlayerStatsIndex((MapMangler.Entities.Player)e.entity);
            turnController.SetPlayerTurnToFinish(index);
            playerStats[index].rerollButton.interactable = false;
        }
    }

    private void OnDestroy()
    {
        /*foreach(var p in players)
        {
            p.Entity.LocationChangeEvent -= Entity_LocationChangeEvent;
            Debug.Log(p.Entity.Location);
        }*/

        foreach (var e in enemies)
        {
            e.Entity.LocationChangeEvent -= Enemy_LocationChangeEvent;
        }
    }

    private void SetupGameState()
    {
        GameStateReadyEvent?.Invoke(this, EventArgs.Empty);
        GameState.RunSetup();
    }

    private void Enemy_LocationChangeEvent(object sender, MapMangler.Entities.Entity.EntityValueChangeEventArgs<MapMangler.Rooms.RoomSegment> e)
    {
        var entity = e.entity;
        var from = e.from;
        var to = e.to;
        var script = entities.Find(script => script.Entity.Equals(e.entity));
        MoveAvatarToTargetLocation(script, from, to);
    }

    public event EventHandler? GameStateReadyEvent;


    public IEnumerator ProcessNPCTurns()
    {
        foreach(EnemyBehaviour enemy in entities)
        {
            var iterator = enemy.MakeNPCTurn();
            bool again;
            do
            {
                again = iterator.MoveNext();
                yield return null;
            } while (again);
        }
    }

    public void MoveAvatarToTargetLocation(EntityBehaviour entity, MapMangler.Rooms.RoomSegment from, MapMangler.Rooms.RoomSegment to)
    {
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

        activePlayerIndex = index;
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
        var segmentList = moveAction.path.Elements;
        var pathCount = segmentList.Count -1;

        var limit = steps - remainingSteps;
        for (var index = 0; index < limit; ++index)
        {
            var result = stepper.Invoke();

            if (result == false)
            {
                yield break;
            }

            var from = segmentList[index];
            var to = segmentList[index + 1];

            if (RoomSegment.Lookup.TryGetValue(from, out var fromSegment)
                && RoomSegment.Lookup.TryGetValue(to, out var toSegment))
            {
                var start = fromSegment.FindOccupiedSpace(player);
                if (start) start.Owner = null;

                var end = toSegment.GetNextFreeSpace();
                end.Owner = player;

                yield return MoveEntity(player, start.transform.position, end.transform.position);
                yield return new WaitForSeconds(0.5f);
            }
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
        //playerStats[playerIndex].remainingActionsLabel.text = actionCount.ToString();
    }

    private void TurnController_AllPlayerTurnsFinished()
    {
        //ProcessNPCTurns();
        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        var enemies = entities.Where(it => it is EnemyBehaviour);
        foreach (EnemyBehaviour enemy in enemies)
        {
            var iterator = enemy.MakeNPCTurn();
            bool again;
            do
            {
                again = iterator.MoveNext();
                yield return null;
            } while (again);
        }
    }

    private void TurnController_EnemyTurnFinshed()
    {
        Debug.Log("TurnController_EnemyTurnFinshed");
        playerStats[0].rerollButton.interactable = true;
        playerStats[1].rerollButton.interactable = true;
        playerStats[2].rerollButton.interactable = true;
        playerStats[3].rerollButton.interactable = true;
    }
}
