using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;

namespace MapMangler.Rooms
{
    public class Room
    {
        public int RoomID { get; set; }

        public IReadOnlyList<Room> NeighbouringRooms { get => neighbours; }

        private readonly List<Room> neighbours = new List<Room>();

        private readonly List<RoomSegment> segments = new List<RoomSegment>();

        public IReadOnlyList<RoomSegment> Segments { get => segments; }

        public RoomSegment CreateSegment(int id)
        {
            RoomSegment segment = new RoomSegment(id, this);
            segments.Add(segment);
            return segment;
        }

        public static void ConnectRooms(Room first, Room second)
        {
            if (!first.neighbours.Contains(second))
                first.neighbours.Add(second);
            if (!second.neighbours.Contains(first))
                second.neighbours.Add(first);
        }

    }
}
