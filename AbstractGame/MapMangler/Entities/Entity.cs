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
                if (oldHealth == value)
                    return;
                health = value;
                HealthChangeEvent?.Invoke(this, new EntityValueChangeEventArgs<int>(this, oldHealth, value));
                if (health == 0)
                    DeathEvent?.Invoke(this, new EntityDeathEventArgs(this));
            }
        }


        protected int actions;
        public int Actions
        {
            get => actions;
            set
            {
                int oldActions = health;
                if (oldActions == value)
                    return;
                actions = value;
                ActionsChangeEvent?.Invoke(this, new EntityValueChangeEventArgs<int>(this, oldActions, value));
            }
        }

        public readonly StatBlock stats = new StatBlock();


        public readonly int entityID;

        private RoomSegment? location = null;
        public RoomSegment? Location
        {
            get => location;
            set
            {
                var oldLocation = location;
                if (oldLocation == value)
                    return;
                location = value;
                oldLocation?.RemoveEntity(this, value);
                value?.AddEntity(this, oldLocation);
                LocationChangeEvent?.Invoke(this, new EntityValueChangeEventArgs<RoomSegment?>(this, oldLocation, value));
            }
        }

        public void ReceiveBlockableDamage(int damage)
        {
            damage -= stats.Armor;
            if(damage > 0)
            {
                Health -= damage;
            }
        }

        public event EventHandler<EntityValueChangeEventArgs<RoomSegment?>>? LocationChangeEvent;
        public event EventHandler<EntityValueChangeEventArgs<int>>? HealthChangeEvent;
        public event EventHandler<EntityDeathEventArgs>? DeathEvent;
        public event EventHandler<EntityValueChangeEventArgs<int>>? ActionsChangeEvent;

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
