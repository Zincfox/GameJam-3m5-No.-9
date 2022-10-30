using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private PlayerBtnHandler[] handler;

    [SerializeField]
    private MapMangler.Difficulty.DifficultyLevel difficultyLevel;

    private const float SecondsToMove = 2.0f;

    private List<EntityBehaviour> entities = new List<EntityBehaviour>();

    public MapMangler.GameState GameState { get; private set; }

    private void Awake()
    {
        GameState = new MapMangler.GameState(MapMangler.Difficulty.DifficultyParameters.fromLevel(difficultyLevel, 4));
    }

    public PlayerBehaviour ActivePlayer { get; set; }

    private void OnEnable()
    {
        levelClickHandler.AreaClicked += LevelClickHandler_AreaClicked;
    }

    private void OnDisable()
    {
        levelClickHandler.AreaClicked -= LevelClickHandler_AreaClicked;
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
        GameState.RunSetup();

        //StartCoroutine(Test());
        SelectPlayer(0);
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

    private void SetupGameState()
    {
        GameStateReadyEvent?.Invoke(this, System.EventArgs.Empty);
    }

    private void Entity_LocationChangeEvent(object sender, MapMangler.Entities.Entity.EntityValueChangeEventArgs<MapMangler.Rooms.RoomSegment> e)
    {
        /*var entity = e.entity;
        var from = e.from;
        var to = e.to;
        var script = entities.Find(script => script.Entity.Equals(e.entity));
        MoveAvatarToTargetLocation(script, from, to);*/
    }

    public event System.EventHandler? GameStateReadyEvent;

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
        ActivePlayer.Entity.Actions = 3; // TODO
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
            }
        }

    }
}
