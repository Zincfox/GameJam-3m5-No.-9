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

        private readonly List<Entity> entities = new List<Entity>();

        public IReadOnlyList<Entity> Entities { get => entities; }

        internal void AddEntity(Entity entity)
        {
            entities.Add(entity);
            EntitiesChangeEvent?.Invoke(this, new RoomContentChangeEventArgs(this, entity, false));
        }

        internal void RemoveEntity(Entity entity)
        {
            if (!entities.Remove(entity)) return;
            EntitiesChangeEvent?.Invoke(this, new RoomContentChangeEventArgs(this, entity, true));
        }

        public event EventHandler<RoomContentChangeEventArgs>? EntitiesChangeEvent;

        public class RoomContentChangeEventArgs : EventArgs
        {
            public readonly Room room;
            public readonly Entity entity;
            private readonly bool isLeave;
            public RoomContentChangeEventArgs(Room room, Entity entity, bool isLeave)
            {
                this.room = room;
                this.entity = entity;
                this.isLeave = isLeave;
            }

            public bool IsLeave() => isLeave;
            public bool IsEnter() => !isLeave;
        }
    }
}
