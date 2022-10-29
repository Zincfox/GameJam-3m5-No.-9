using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MapMangler.Rooms
{
    public static class Navigators
    {
        public readonly static Navigator<RoomSegment, RoomSegmentPath> DefaultSegmentNavigator = new Navigator<RoomSegment, RoomSegmentPath>(s => s.NeighbouringSegments);
        public readonly static Navigator<Room, RoomPath> DefaultRoomNavigator = new Navigator<Room, RoomPath>(r => r.NeighbouringRooms);
    }

    public class Navigator<T, P> where P : notnull, IPath<T>, new() where T:notnull
    {

        public delegate IEnumerable<(int cost, T neighbour)> NeighbourCostFunction(T source);

        public readonly NeighbourCostFunction neighbours;
        public Navigator(NeighbourCostFunction neighbours)
        {
            this.neighbours = neighbours;
        }

        public Navigator(Func<T, IEnumerable<T>> neighbours) : this(
            e => neighbours(e).Select(
                n => (1, n)))
        {

        }

        public IList<P> FindShortestPaths(T source, Predicate<T> predicate)
        {
            ISet<T> visited = new HashSet<T>();
            List<T> fringe = new List<T>() {
                source
            };
            Dictionary<T, Tuple<int, IList<T>>> predecessors = new Dictionary<T, Tuple<int, IList<T>>>();
            predecessors.Add(source, new Tuple<int, IList<T>>(0, new List<T>()));
            int? foundAtCost = null;
            List<T> matchingTs = new List<T>();
            while (fringe.Count > 0)
            {
                T currentT = fringe[0];
                (int currentCost, _) = predecessors[currentT];
                if (foundAtCost != null && currentCost > foundAtCost) break;
                fringe.RemoveAt(0);
                visited.Add(currentT);

                if (predicate(currentT))
                {
                    matchingTs.Add(currentT);
                    if (foundAtCost == null)
                        foundAtCost = currentCost;
                }

                foreach ((int cost, T neighbour) in neighbours(currentT))
                {
                    if (visited.Contains(neighbour)) continue;
                    if (predecessors.TryGetValue(neighbour, out var oldInformation))
                    {
                        int newCost = currentCost + cost;
                        (int oldCost, IList<T> oldTs) = oldInformation;
                        if (newCost > oldCost) continue;
                        if (newCost == oldCost)
                        {
                            if (!oldTs.Contains(currentT))
                                oldTs.Add(currentT);
                        }
                        else //newCost < oldCost
                        {
                            predecessors[neighbour] = new Tuple<int, IList<T>>(newCost, new List<T>() { currentT });
                        }
                    }
                }

                fringe.Sort((a, b) => predecessors[a].Item1 - predecessors[b].Item1);
            }
            return matchingTs.SelectMany(target => TraversePredecessors(predecessors, target)).ToList();
        }

        private IEnumerable<P> TraversePredecessors(IDictionary<T, Tuple<int, IList<T>>> predecessors, T target)
        {
            (_, IList<T> previousTs) = predecessors[target];
            return previousTs.SelectMany(r =>
            {
                if (target.Equals(r))
                {
                    P path = new P();
                    path.Elements.Add(target);
                    return new List<P>() { path };
                }
                IList<P> paths = TraversePredecessors(predecessors, r).ToList();
                foreach (var path in paths)
                {
                    path.Elements.Add(target);
                }
                return paths;
            });
        }

        public IList<P> FindShortestPaths(T source, T target)
        {
            return FindShortestPaths(source, e => e.Equals(target));
        }
    }
}
