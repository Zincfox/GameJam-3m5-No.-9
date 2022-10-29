using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Rooms;

namespace MapMangler.Entities
{
    public abstract class Entity
    {
        public Entity(int entityID)
        {
            this.entityID = entityID;
        }

        public readonly int entityID;

        private Room? location = null;
        public Room? Location {
            get => location;
            set
            {
                Room? oldLocation = location;
                location = value;
                oldLocation?.removeEntity(this);
                value?.addEntity(this);
                LocationChangeEvent?.Invoke(this, new LocationChangeEventArgs(this, oldLocation, value));
            }
        }

        public event EventHandler<LocationChangeEventArgs>? LocationChangeEvent;

        public class LocationChangeEventArgs : EventArgs
        {
            public readonly Room? from;
            public readonly Room? to;
            public readonly Entity entity;
            public LocationChangeEventArgs(Entity entity, Room? from, Room? to)
            {
                this.from = from;
                this.to = to;
                this.entity = entity;
            }
        }
    }
}
