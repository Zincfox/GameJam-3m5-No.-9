using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public class RoomSegment
    {
        public readonly int id;
        public readonly Room parentRoom;

        public IReadOnlyList<RoomSegment> NeighbouringSegments { get => neighbours; }

        private readonly List<RoomSegment> neighbours = new List<RoomSegment>();

        public static void ConnectSegments(RoomSegment first, RoomSegment second)
        {
            if (!first.neighbours.Contains(second))
                first.neighbours.Add(second);
            if (!second.neighbours.Contains(first))
                second.neighbours.Add(first);
            if (first.parentRoom != second.parentRoom)
                Room.ConnectRooms(first.parentRoom, second.parentRoom);
        }


        internal RoomSegment(int id, Room parentRoom)
        {
            this.id = id;
            this.parentRoom = parentRoom;
        }
    }
}
