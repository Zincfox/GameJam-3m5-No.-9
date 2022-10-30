using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VictoryRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject victoryObject;


    [SerializeField]
    private int requiredPlayerCount = 4;

    private Room room;
    // Start is called before the first frame update
    void Start()
    {
        room = GetComponent<Room>();
        foreach (var segment in room.RoomArea.Segments)
        {
            segment.EntitiesChangeEvent += Segment_EntitiesChangeEvent;
        }
    }

    private bool victorySet = false;

    private void Segment_EntitiesChangeEvent(object sender, MapMangler.Rooms.RoomSegment.RoomSegmentContentChangeEventArgs e)
    {
        if (victorySet) return;
        if(room.RoomArea.GetRoomEntities().Count(e=>e is MapMangler.Entities.Player) >= requiredPlayerCount)
        {
            victorySet = true;
            Victory();
        }
    }

    void Victory()
    {
        gameObject.SetActive(true);
    }
}
