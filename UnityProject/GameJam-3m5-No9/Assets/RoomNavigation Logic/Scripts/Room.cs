using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private RoomSegment[] roomSegements;

    [SerializeField]
    private Room[] roomNeighbors;

    public MapMangler.Rooms.Room RoomArea { get; } = new MapMangler.Rooms.Room();

    void Start()
    {
        //InitRoomSegments();
        //ConnectRooms();
    }

    /*private void InitRoomSegments ()
    {
        var room = RoomArea;
        foreach (var segment in roomSegements)
        {
            var id = segment.GetInstanceID();
            room.CreateSegment(id);
        }
        Debug.Log(room.Segments.Count);
    }

    private void ConnectRooms()
    {
        var room = RoomArea;
        foreach (var neigbor in roomNeighbors)
        {
            MapMangler.Rooms.Room.ConnectRooms(room, neigbor.RoomArea);
        }
        Debug.Log(room.NeighbouringRooms.Count);
    }*/
}