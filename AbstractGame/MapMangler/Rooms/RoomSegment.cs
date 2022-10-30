using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;

namespace MapMangler.Rooms
{
    public class RoomSegment
    {
        public readonly int id;
        public readonly Room parentRoom;

        public IReadOnlyList<RoomSegment> NeighbouringSegments { get => neighbours; }

        private readonly List<RoomSegment> neighbours = new List<RoomSegment>();

        public static void ConnectSegments(RoomSegment first, RoomSegment second)
        {
            if (!first.neighbours.Contains(second))
                first.neighbours.Add(second);
            if (!second.neighbours.Contains(first))
                second.neighbours.Add(first);
        }


        internal RoomSegment(int id, Room parentRoom)
        {
            this.id = id;
            this.parentRoom = parentRoom;
        }


        private readonly List<Entity> entities = new List<Entity>();

        public IReadOnlyList<Entity> Entities { get => entities; }

        internal void AddEntity(Entity entity, RoomSegment? previousLocation)
        {
            if (entities.Contains(entity)) return;
            entities.Add(entity);
            EntitiesChangeEvent?.Invoke(this, new RoomSegmentContentChangeEventArgs(this, entity, false, previousLocation));
        }

        internal void RemoveEntity(Entity entity, RoomSegment? newLocation)
        {
            if (!entities.Remove(entity)) return;
            EntitiesChangeEvent?.Invoke(this, new RoomSegmentContentChangeEventArgs(this, entity, true, newLocation));
        }

        public event EventHandler<RoomSegmentContentChangeEventArgs>? EntitiesChangeEvent;

        public class RoomSegmentContentChangeEventArgs : EventArgs
        {
            public readonly RoomSegment roomSegment;
            public readonly Entity entity;
            private readonly bool isLeave;
            public readonly RoomSegment? fromRoomSegment;
            public readonly RoomSegment? toRoomSegment;
            public RoomSegmentContentChangeEventArgs(RoomSegment roomSegment, Entity entity, bool isLeave, RoomSegment? other=null)
            {
                this.roomSegment = roomSegment;
                this.entity = entity;
                this.isLeave = isLeave;
                if (isLeave)
                {
                    fromRoomSegment = roomSegment;
                    toRoomSegment = other;
                }
                else
                {
                    fromRoomSegment = other;
                    toRoomSegment = roomSegment;
                }
            }

            public bool IsLeave() => isLeave;
            public bool IsEnter() => !isLeave;
        }
    }
}
