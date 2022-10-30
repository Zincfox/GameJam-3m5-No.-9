using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MapMangler.Entities;
using MapMangler.Rooms;
using MapMangler.Actions;

namespace MapMangler.Difficulty
{
    public class NPC_AI
    {

        public readonly GameState gameState;
        public readonly NPC npc;

        private readonly List<(int health, RoomSegment location)> assumedHealthLocations = new List<(int, RoomSegment)>();

        public NPC_AI(GameState gameState, NPC npc)
        {
            this.gameState = gameState;
            this.npc = npc;
            this.gameState.enemyVisionTracker.EntityVisionChangedEvent += EnemyVisionTracker_EntityVisionChangedEvent;
            foreach (var room in this.gameState.enemyVisionTracker.GetEntityVisibleRooms(npc))
            {
                GainedVision(room);
            }
        }

        private void EnemyVisionTracker_EntityVisionChangedEvent(object sender, Rooms.Visibility.VisionTracker.EntityVisionTracker.EntityVisionEventArgs e)
        {
            if (e.entity != npc) return;
            foreach (var lostVision in e.LostVision)
            {
                LostVision(lostVision);
            }
            foreach (var gainedVision in e.GainedVision)
            {
                GainedVision(gainedVision);
            }
        }

        private void GainedVision(Room room)
        {
            foreach (var segment in room.Segments)
            {
                segment.EntitiesChangeEvent += Segment_EntitiesChangeEvent;
                assumedHealthLocations.RemoveAll(assumedInfo => assumedInfo.location == segment);
            }
        }

        private void LostVision(Room room)
        {
            foreach (var segment in room.Segments)
            {
                segment.EntitiesChangeEvent -= Segment_EntitiesChangeEvent;
                foreach (var entity in segment.Entities)
                {
                    assumedHealthLocations.Add((entity.Health, segment));
                }
            }
        }

        private void Segment_EntitiesChangeEvent(object sender, RoomSegment.RoomSegmentContentChangeEventArgs e)
        {
            if (!(e.entity is Player)) return;
            var toSegment = e.toRoomSegment;
            if (e.IsLeave() && toSegment != null && !gameState.enemyVisionTracker.IsVisible(toSegment, npc))
            {
                assumedHealthLocations.Add((e.entity.Health, toSegment));
            }
        }


        public void StartTurn()
        {
            npc.Actions = npc.stats.BonusActions + GlobalRandom.random.Next(npc.stats.MinRollActions, npc.stats.MaxRollActions + 1);
        }




        public GameAction? ObtainNextAction()
        {
            if (npc.Actions <= 0) return EndTurn();
            var location = npc.Location;
            if (location == null) return EndTurn();
            IList<Player> playersInAttackRange = (npc.stats.Ranged ? location.parentRoom.GetRoomEntities() : location.Entities).OfType<Player>().ToList();
            var target = ChooseMinBy(playersInAttackRange, p => p.Health);
            if (target != null)
            {
                var attackAction = npc.AttemptAttack(target);
                if (attackAction != null)
                {
                    attackAction.ActionPerformedEvent += (_, _2) => assumedHealthLocations.Clear();
                    return attackAction;
                }
            }
            ISet<Room> vision = gameState.enemyVisionTracker.GetEntityVisibleRooms(npc);
            IList<RoomSegmentPath> shortestPathsToPlayers = Navigators.DefaultSegmentNavigator.FindShortestPaths(location, s =>
            {
                return !vision.Contains(s.parentRoom);
            });

            var pathToLowestPlayer = ChooseMinBy(shortestPathsToPlayers, p => p.EndSegment!.Entities.Min(e => e.Health));
            if (pathToLowestPlayer != null)
            {
                pathToLowestPlayer.LimitTo(1);
                var moveAction = npc.AttemptMove(pathToLowestPlayer, false);
                if (moveAction != null)
                {
                    moveAction.ActionPerformedEvent += (_, _2) => assumedHealthLocations.Clear();
                    return moveAction;
                }
            }
            IList<RoomSegmentPath> shortestPathsToAssumedPlayers = Navigators.DefaultSegmentNavigator.FindShortestPaths(location, s =>
            {
                return assumedHealthLocations.Any((assumedInfo) => assumedInfo.location == s);
            });

            var pathToLowestAssumedPlayer = ChooseMinBy(shortestPathsToAssumedPlayers,
                p => assumedHealthLocations
                .Where(assumedInfo => assumedInfo.location == p.EndSegment)
                .Min(assumedInfo => assumedInfo.health));
            if (pathToLowestAssumedPlayer != null)
            {
                pathToLowestAssumedPlayer.LimitTo(1);
                var moveAction = npc.AttemptMove(pathToLowestAssumedPlayer, false);
                return moveAction;
            }
            return EndTurn();
        }

        private GameAction? EndTurn()
        {
            return null;
        }

        private static T? ChooseMinBy<T, C>(IEnumerable<T> collection, Func<T, C> minBy) where C : IComparable<C> where T : class
        {
            List<T> list = collection.ToList();
            if (list.Count == 0) return null;
            T minElement = list[0];
            C minValue = minBy(minElement);
            foreach (T element in list.Skip(1))
            {
                C newValue = minBy(element);
                if (newValue.CompareTo(minValue) < 0)
                {
                    minValue = newValue;
                    minElement = element;
                }
            }
            return minElement;
        }
    }
}
