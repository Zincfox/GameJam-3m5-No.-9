using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour[] players;

    [SerializeField]
    private EnemyBehaviour[] enemies;

    [SerializeField]
    private RoomSegment startRoomSegment;

    [SerializeField]
    private RoomSegment sampleTargetSegment;

    private const float SecondsToMove = 2.0f;

    private List<EntityBehaviour> entities = new List<EntityBehaviour>();

    private void Start()
    {
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

        StartCoroutine(Test());
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
        var entity = e.entity;
        var from = e.from;
        var to = e.to;
        var script = entities.Find(script => script.Entity.Equals(e.entity));
        MoveAvatarToTargetLocation(script, from, to);
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

        var entity = players[3].Entity;

        entity.Actions = 3;
        //players[3].Entity.Location = sampleTargetSegment.Segment;
        var moveAction = entity.AttemptMoveTo(sampleTargetSegment.Segment);
        Debug.Log(moveAction);
        var stepper = moveAction.GetStepper();
        Debug.Log(stepper);
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
        Debug.Log("MoveEntity");
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
}
