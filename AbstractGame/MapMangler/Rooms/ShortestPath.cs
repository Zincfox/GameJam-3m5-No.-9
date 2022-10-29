using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public class RoomPath
    {
        public Room? Start
        {
            get => rooms.Count > 0 ? rooms[0] : null;
        }
        public Room? End
        {
            get => rooms.Count > 0 ? rooms[rooms.Count - 1] : null;
        }
        public IList<Room> rooms = new List<Room>();

        public int Cost
        {
            get => rooms.Count;
        }
    }
}
