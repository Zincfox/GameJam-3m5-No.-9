using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomSegment : MonoBehaviour
{
    public static Dictionary<MapMangler.Rooms.RoomSegment, RoomSegment> Lookup = new Dictionary<MapMangler.Rooms.RoomSegment, RoomSegment>();

    [SerializeField]
    private RoomSegment[] neighbors;

    [SerializeField]
    private AvatarLocation[] avatarLocations;

    [SerializeField]
    private Room room;

    private MapMangler.Rooms.RoomSegment segment;

    public MapMangler.Rooms.RoomSegment Segment { get => segment; }

    private void Awake()
    {
        var id = gameObject.GetInstanceID();
        segment = room.RoomArea.CreateSegment(id);
        Lookup.Add(segment, this);
    }

    private void Reset()
    {
        avatarLocations = GetComponentsInChildren<AvatarLocation>();
    }

    private void Start()
    {
        foreach(var n in neighbors)
        {
            MapMangler.Rooms.RoomSegment.ConnectSegments(segment, n.segment);
        }
    }

    private void OnDrawGizmos()
    {
        foreach(var n in neighbors)
        {
            Gizmos.DrawLine(transform.position, n.transform.position);
        }
    }

    public AvatarLocation GetNextFreeSpace()
    {
        return Array.Find(avatarLocations, it => it.Owner == null);
    }

    public AvatarLocation FindOccupiedSpace(EntityBehaviour enity)
    {
        return Array.Find(avatarLocations, it => it.Owner == enity);
    }
}
