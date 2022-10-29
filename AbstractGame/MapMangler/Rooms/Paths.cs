using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public interface IPath<T>
    {
        int Cost { get => Elements.Count - 1; }
        public IList<T> Elements { get; }
    }
    public class RoomPath : IPath<Room>
    {
        public IList<Room> rooms = new List<Room>();

        public IList<Room> Elements => rooms;
        public Room? Start
        {
            get => rooms.Count > 0 ? rooms[0] : null;
        }
        public Room? End
        {
            get => rooms.Count > 0 ? rooms[rooms.Count - 1] : null;
        }

    }

    public class RoomSegmentPath : IPath<RoomSegment>
    {
        public IList<RoomSegment> roomSegments = new List<RoomSegment>();
        public IList<RoomSegment> Elements => roomSegments;
        public RoomSegment? StartSegment => roomSegments.Count > 0 ? roomSegments[0] : null;
        public Room? StartRoom => StartSegment?.parentRoom;
        public RoomSegment? EndSegment => roomSegments.Count > 0 ? roomSegments[roomSegments.Count -1] : null;
        public Room? EndRoom => EndSegment?.parentRoom;
    }
}
