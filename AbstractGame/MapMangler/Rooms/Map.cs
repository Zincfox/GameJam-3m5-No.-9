using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public class RoomMap
    {
        public IReadOnlyList<Room> Rooms => rooms;
        private readonly List<Room> rooms = new List<Room>();

        public void AddRoom(Room room)
        {
            rooms.Add(room);
        }
    }
}
