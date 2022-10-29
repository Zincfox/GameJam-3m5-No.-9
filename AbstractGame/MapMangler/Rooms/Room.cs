using System;
using System.Collections.Generic;
using System.Text;

namespace MapMangler.Rooms
{
    public class Room
    {
        public int RoomID { get; set; }

        public IList<Room> Rooms = new List<Room>();
    }
}
