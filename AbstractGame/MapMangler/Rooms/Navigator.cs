using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public static class Navigator
    {
        public static RoomPath findShortestPath(Room source, Room target)
        {
            RoomPath path = new RoomPath();
            path.rooms.Add(source);
            path.rooms.Add(target);
            return path;
        }
    }
}
