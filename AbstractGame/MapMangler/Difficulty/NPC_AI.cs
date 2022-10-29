using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MapMangler.Entities;
using MapMangler.Rooms;

namespace MapMangler.Difficulty
{
    public class NPC_AI
    {

        public readonly GameState gameState;
        public readonly NPC npc;

        public NPC_AI(GameState gameState, NPC npc)
        {
            this.gameState = gameState;
            this.npc = npc;
        }

        public void StartTurn()
        {
            npc.Actions = gameState.difficulty.EnemyActions;
        }

        public abstract class GameAction
        {
            public Entity Source { get; }

            public GameAction(Entity source)
            {
                Source = source;
            }

            public abstract void Perform();
        }

        public class AttackAction : GameAction
        {
            public AttackAction(Entity source, Entity target, int damage) : base(source)
            {
                this.target = target;
                this.damage = damage;
            }

            public readonly Entity target;
            public readonly int damage;

            public override void Perform()
            {
                if (Source.Actions < 0)
                {
                    return;
                }
                Source.Actions--;
                target.ReceiveBlockableDamage(damage);
            }
        }

        public class MoveAction : GameAction
        {
            public readonly RoomSegmentPath path;

            public MoveAction(Entity source, RoomSegmentPath path) : base(source)
            {
                this.path = path;
            }

            public Func<bool> GetStepper()
            {
                IEnumerator<RoomSegment> enumerator = path.roomSegments.Skip(1).GetEnumerator();
                bool stepper()
                {
                    if (!enumerator.MoveNext()) return false;
                    if (Source.Actions <= 0) return false;
                    Source.Actions--;
                    Source.Location = enumerator.Current;
                    return true;
                }
                return stepper;
            }

            public override void Perform()
            {
                Func<bool> stepper = GetStepper();
                while (stepper()) { }
            }
        }

        public GameAction? ObtainNextAction()
        {
            if (npc.Actions <= 0) return null;
            var location = npc.Location;
            if (location == null) return null;
            IList<Player> playersInAttackRange = (npc.stats.Ranged ? location.parentRoom.GetRoomEntities() : location.Entities).OfType<Player>().ToList();
            var target = ChooseMinBy(playersInAttackRange, p => p.Health);
            if (target != null)
            {
                return new AttackAction(npc, target, npc.stats.Damage);
            }
            ISet<Room> vision = gameState.enemyVisionTracker.GetEntityVisibleRooms(npc);
            IList<RoomSegmentPath> shortestPathsToPlayers = Navigators.DefaultSegmentNavigator.FindShortestPaths(location, s =>
            {
                return (!vision.Contains(s.parentRoom));
            });

            var pathToLowestPlayer = ChooseMinBy(shortestPathsToPlayers, p => p.EndSegment!.Entities.Min(e => e.Health));
            if(pathToLowestPlayer != null)
            {
                return new MoveAction(npc, pathToLowestPlayer);
            }
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
                if(newValue.CompareTo(minValue)<0)
                {
                    minValue = newValue;
                    minElement = element;
                }
            }
            return minElement;
        }
    }
}
