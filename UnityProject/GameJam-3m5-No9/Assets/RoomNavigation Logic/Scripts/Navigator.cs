using UnityEngine;
using System.Collections;

public class Navigator : MonoBehaviour
{
    public static Navigator Instance;

    private const float SecondsToMove = 2.0f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
    }

    public void MoveAvatarToTargetLocation(EntityBehaviour entity, MapMangler.Rooms.RoomSegment from, MapMangler.Rooms.RoomSegment to)
    {
        Debug.Log(from+ " [ "+to);
        if(RoomSegment.Lookup.TryGetValue(from, out var fromSegment)
            && RoomSegment.Lookup.TryGetValue(to, out var toSegment))
        {
            var start = fromSegment.FindOccupiedSpace(entity);
            if(start) start.Owner = null;

            var end = toSegment.GetNextFreeSpace();
            end.Owner = entity;

            Debug.Log(from.id+" "+fromSegment);
            Debug.Log(to.id+ " "+toSegment);
            Debug.Log(entity);
            Debug.Log(gameObject);
            var coroutine = MoveEntity(entity, start.transform.position, end.transform.position);
            StartCoroutine(coroutine);
        }
    }

    private IEnumerator MoveEntity(EntityBehaviour entity, Vector3 startPos, Vector3 targetPos)
    {
        yield return null;
        /*void SetEntityPosition (EntityBehaviour entity, Vector3 pos) {
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
        SetEntityPosition(entity, pos);*/
    }
}
