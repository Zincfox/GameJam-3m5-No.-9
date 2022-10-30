using System;
using System.Collections.Generic;
using System.Text;
using MapMangler.Entities;
using System.Linq;

namespace MapMangler.Rooms.Visibility
{
    public abstract class VisionTracker
    {

        public virtual bool IsVisible(Room room)
        {
            return visibleRooms.Contains(room);
        }

        public virtual bool IsVisible(RoomSegment segment)
        {
            return IsVisible(segment.parentRoom);
        }

        public event EventHandler<VisionEventArgs>? GlobalVisionChangedEvent;
        public IReadOnlyList<Room> VisibleRooms => visibleRooms;
        protected readonly List<Room> visibleRooms = new List<Room>();

        public class VisionEventArgs : EventArgs
        {
            public readonly ISet<Room> GainedVision;
            public readonly ISet<Room> LostVision;

            public VisionEventArgs(ISet<Room> gainedVision, ISet<Room> lostVision)
            {
                GainedVision = gainedVision;
                LostVision = lostVision;
            }
        }

        public class UnveiledVisionTracker : VisionTracker
        {
            public override bool IsVisible(Room room)
            {
                return true;
            }

            public override bool IsVisible(RoomSegment segment)
            {
                return true;
            }

            public UnveiledVisionTracker(RoomMap map) : this(map.Rooms)
            {

            }

            public UnveiledVisionTracker(IEnumerable<Room> roomList)
            {
                visibleRooms.AddRange(roomList);
            }
        }

        public class EntityVisionTracker : VisionTracker
        {
            public readonly bool seeIntoAdjacientRooms;

            public EntityVisionTracker() : this(seeIntoAdjacientRooms: false)
            {

            }

            public EntityVisionTracker(bool seeIntoAdjacientRooms = false)
            {
                this.seeIntoAdjacientRooms = seeIntoAdjacientRooms;
            }

            protected readonly Dictionary<Entity, ISet<Room>> visibleEntityRooms = new Dictionary<Entity, ISet<Room>>();

            public bool IsVisible(Room room, Entity to)
            {
                return visibleEntityRooms.TryGetValue(to, out ISet<Room> rooms) && rooms.Contains(room);
            }

            public bool IsVisible(RoomSegment segment, Entity to)
            {
                return IsVisible(segment.parentRoom, to);
            }

            public ISet<Room> GetEntityVisibleRooms(Entity entity)
            {
                return visibleEntityRooms.GetValueOrDefault(entity) ?? new HashSet<Room>();
            }

            public ISet<Entity> GetVisibleBy(Room room)
            {
                return visibleEntityRooms.Where(entry => entry.Value.Contains(room)).Select(entry => entry.Key).ToHashSet();
            }
            public void AddVisionSource(Entity ent)
            {
                ent.LocationChangeEvent += Ent_LocationChangeEvent;
                visibleEntityRooms.Add(ent, new HashSet<Room>());
                UpdateEntityLocation(ent, null, ent.Location);
            }


            public void RemoveVisionSource(Entity ent)
            {
                ent.LocationChangeEvent -= Ent_LocationChangeEvent;
                UpdateEntityLocation(ent, ent.Location, null);
            }

            public event EventHandler<EntityVisionEventArgs>? EntityVisionChangedEvent;

            private ISet<Room> CalculateEntityVisibleRooms(Entity ent)
            {
                return CalculateEntityVisibleRooms(ent.Location);
            }


            private ISet<Room> CalculateEntityVisibleRooms(RoomSegment? location)
            {
                if (location == null)
                    return new HashSet<Room>();
                ISet<Room> visible = new HashSet<Room>
                {
                    location.parentRoom
                };
                if (seeIntoAdjacientRooms)
                    foreach (var segment in location.NeighbouringSegments)
                    {
                        visible.Add(segment.parentRoom);
                    }
                return visible;
            }

            private void Ent_LocationChangeEvent(object sender, Entity.EntityValueChangeEventArgs<RoomSegment?> e)
            {
                UpdateEntityLocation(e.entity, e.from, e.to);
            }

            private void UpdateEntityLocation(Entity ent, RoomSegment? from, RoomSegment? to)
            {
                var storedEntityVision = visibleEntityRooms[ent];
                var newEntityVision = CalculateEntityVisibleRooms(to);

                var lostEntityVisionSet = storedEntityVision.Except(newEntityVision).ToHashSet();
                var gainedEntityVisionSet = newEntityVision.Except(storedEntityVision).ToHashSet();

                var staticGlobalVisibilityPartialSet = CalculateVisibleRoomUnion(exceptEntity: ent);
                var lostGlobalVisionSet = lostEntityVisionSet.Except(staticGlobalVisibilityPartialSet).ToHashSet(); //gainedEntity- and lostEntity-VisionSet already exclude rooms unchanged by entity movement
                var gainedGlobalVisionSet = gainedEntityVisionSet.Except(staticGlobalVisibilityPartialSet).ToHashSet();

                visibleRooms.AddRange(gainedGlobalVisionSet);
                visibleRooms.RemoveAll(lostGlobalVisionSet.Contains);

                EntityVisionChangedEvent?.Invoke(this, new EntityVisionEventArgs(gainedEntityVisionSet, lostEntityVisionSet, ent));
                GlobalVisionChangedEvent?.Invoke(this, new VisionEventArgs(gainedGlobalVisionSet, lostGlobalVisionSet));
            }

            private ISet<Room> CalculateVisibleRoomUnion()
            {
                return visibleEntityRooms.Values.SelectMany(vision => vision).ToHashSet();
            }
            private ISet<Room> CalculateVisibleRoomUnion(Entity exceptEntity)
            {
                return visibleEntityRooms.SelectMany(entry => entry.Key == exceptEntity ? Enumerable.Empty<Room>() : entry.Value).ToHashSet();
            }

            public class EntityVisionEventArgs : VisionEventArgs
            {
                public readonly Entity entity;
                public EntityVisionEventArgs(ISet<Room> gainedVision, ISet<Room> lostVision, Entity entity) : base(gainedVision, lostVision)
                {
                    this.entity = entity;
                }
            }
        }
    }
}
