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

    public class Navigator<T, P> where P : GamePath<T>, new() where T : notnull
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
        public P? FindShortestPath(T source, T target)
        {
            return FindShortestPaths(source, target).FirstOrDefault();
        }
        public IList<P> FindShortestPaths(T source, T target)
        {
            return FindShortestPaths(source, e => e.Equals(target));
        }

        public P? FindShortestPath(T source, Predicate<T> predicate)
        {
            return FindShortestPaths(source, predicate).FirstOrDefault();
        }

        public IList<P> FindShortestPaths(T source, Predicate<T> predicate)
        {
            ISet<T> visited = new HashSet<T>();
            List<T> fringe = new List<T>() {
                source
            };
            Dictionary<T, Tuple<int, IList<T>>> predecessors = new Dictionary<T, Tuple<int, IList<T>>>();
            predecessors.Add(source, new Tuple<int, IList<T>>(0, new List<T>() { source }));
            int? foundAtCost = null;
            List<T> matchingTs = new List<T>();
            while (fringe.Count > 0)
            {
                T currentT = fringe[0];
                GameState.LOGGER?.Invoke($"Visiting {currentT}");
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
                    int newCost = currentCost + cost;
                    if (visited.Contains(neighbour)) continue;
                    if (!fringe.Contains(neighbour)) fringe.Add(neighbour);
                    if (predecessors.TryGetValue(neighbour, out var oldInformation))
                    {
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
                    else
                    {
                        predecessors[neighbour] = new Tuple<int, IList<T>>(newCost, new List<T>() { currentT });
                    }
                }

                fringe.Sort((a, b) =>
                {
                    return predecessors[a].Item1 - predecessors[b].Item1;
                });
            }
            GameState.LOGGER?.Invoke($"Predecessors: {string.Join(',', predecessors.Select(e => $"{e.Key}:({e.Value.Item1})#[{string.Join(',', e.Value.Item2)}]"))}");
            GameState.LOGGER?.Invoke($"Traversing backwards: [{string.Join(',', matchingTs)}]");
            IList<P> result = matchingTs.SelectMany(target => TraversePredecessors(predecessors, target)).ToList();
            GameState.LOGGER?.Invoke($"Result: [{string.Join(',', result)}]");
            return result;
        }

        private IEnumerable<P> TraversePredecessors(IDictionary<T, Tuple<int, IList<T>>> predecessors, T target)
        {
            GameState.LOGGER?.Invoke($"Path-Element: {target}");
            (int cost, IList<T> previousTs) = predecessors[target];
            GameState.LOGGER?.Invoke($"Predecessor: {cost}#[{string.Join(',', previousTs)}]");
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
    }
}
