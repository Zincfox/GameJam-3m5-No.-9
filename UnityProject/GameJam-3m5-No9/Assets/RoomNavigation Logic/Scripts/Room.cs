using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField]
    private RoomSegment[] roomSegements;

    [SerializeField]
    private Room[] roomNeighbors;

    public MapMangler.Rooms.Room RoomArea { get; } = new MapMangler.Rooms.Room();

    private void Awake()
    {
        GameMaster gm = FindObjectOfType<GameMaster>();
        Debug.Log("Awake Room", gm);
        if (gm == null) throw new System.Exception("Could not locate GameMaster in scene");
        gm.GameStateReadyEvent += (_, _) =>
        {
            gm.GameState.map.AddRoom(RoomArea);
        };
    }

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
