using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public abstract class GamePath<T>
    {
        public virtual int Cost { get => Elements.Count - 1; }
        public abstract IList<T> Elements { get; }

        public virtual void LimitTo(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            while (Elements.Count > count)
                Elements.RemoveAt(count);
        }
    }
    public class RoomPath : GamePath<Room>
    {
        public IList<Room> rooms = new List<Room>();

        public override IList<Room> Elements => rooms;
        public Room? Start
        {
            get => rooms.Count > 0 ? rooms[0] : null;
        }
        public Room? End
        {
            get => rooms.Count > 0 ? rooms[rooms.Count - 1] : null;
        }
    }

    public class RoomSegmentPath : GamePath<RoomSegment>
    {
        public IList<RoomSegment> roomSegments = new List<RoomSegment>();
        public override IList<RoomSegment> Elements => roomSegments;
        public RoomSegment? StartSegment => roomSegments.Count > 0 ? roomSegments[0] : null;
        public Room? StartRoom => StartSegment?.parentRoom;
        public RoomSegment? EndSegment => roomSegments.Count > 0 ? roomSegments[roomSegments.Count - 1] : null;
        public Room? EndRoom => EndSegment?.parentRoom;

        public override string ToString()
        {
            return $"SegmentPath({string.Join("->",roomSegments)})";
        }
    }
}
