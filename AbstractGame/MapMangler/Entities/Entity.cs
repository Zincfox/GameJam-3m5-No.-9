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

        protected int health;

        public int Health
        {
            get => health;
            set
            {
                int oldHealth = health;
                if (value < 0) value = 0;
                if (oldHealth == health)
                    return;
                health = value;
                HealthChangeEvent?.Invoke(this, new EntityValueChangeEventArgs<int>(this, oldHealth, value));
                if (health == 0)
                    DeathEvent?.Invoke(this, new EntityDeathEventArgs(this));
            }
        }


        public readonly int entityID;

        private Room? location = null;
        public Room? Location
        {
            get => location;
            set
            {
                Room? oldLocation = location;
                if (oldLocation == location)
                    return;
                location = value;
                oldLocation?.RemoveEntity(this);
                value?.AddEntity(this);
                LocationChangeEvent?.Invoke(this, new EntityValueChangeEventArgs<Room?>(this, oldLocation, value));
            }
        }

        public event EventHandler<EntityValueChangeEventArgs<Room?>>? LocationChangeEvent;
        public event EventHandler<EntityValueChangeEventArgs<int>>? HealthChangeEvent;
        public event EventHandler<EntityDeathEventArgs>? DeathEvent;

        public class EntityValueChangeEventArgs<T> : EventArgs
        {
            public readonly T from;
            public readonly T to;
            public readonly Entity entity;
            public EntityValueChangeEventArgs(Entity entity, T from, T to)
            {
                this.from = from;
                this.to = to;
                this.entity = entity;
            }
        }

        public class EntityDeathEventArgs : EventArgs
        {
            public readonly Entity entity;
            public EntityDeathEventArgs(Entity entity)
            {
                this.entity = entity;
            }
        }
    }
}
