using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MapMangler.Entities;

namespace MapMangler.Rooms
{
    public class Room
    {
        public int RoomID { get; set; }

        public IReadOnlyList<Room> NeighbouringRooms { get => segments.Select(s => s.parentRoom).Distinct().Except(new Room[] { this }).ToList(); }

        private readonly List<RoomSegment> segments = new List<RoomSegment>();

        public IReadOnlyList<RoomSegment> Segments { get => segments; }

        public IEnumerable<Entity> GetRoomEntities()
        {
            return Segments.SelectMany(s => s.Entities);
        }

        public RoomSegment CreateSegment(int id)
        {
            RoomSegment segment = new RoomSegment(id, this);
            segments.Add(segment);
            return segment;
        }

        public override string ToString()
        {
            return $"Room({RoomID}, segments:[{string.Join(',', segments.Select(s=>s.id))}])";
        }
    }
}
