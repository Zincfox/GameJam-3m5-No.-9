using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;

namespace MapMangler.Rooms
{
    public class Room
    {
        public int RoomID { get; set; }

        public IList<Room> Neighbours = new List<Room>();

        private List<Entity> entities = new List<Entity>();

        public IReadOnlyList<Entity> Entities { get => entities; }

        internal void addEntity(Entity entity)
        {
            entities.Add(entity);
            EntitiesChangeEvent?.Invoke(this, new RoomContentChangeEventArgs(this, entity, false));
        }

        internal void removeEntity(Entity entity)
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
