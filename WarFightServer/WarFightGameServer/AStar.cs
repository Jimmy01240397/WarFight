using System;
using System.Collections.Generic;

namespace WarFightGameServer
{
    public class AStar
    {
        private static readonly (int, int)[] directions =
        {
        ( 0, 1 ),  // right
        ( 1, 0 ),  // down
        ( 0, -1 ), // left
        ( -1, 0 ),  // up
        ( 1, 1 ),  // down-right
        ( 1, -1 ), // down-left
        ( -1, -1 ), // up-left
        ( -1, 1 )   // up-right
    };

        private byte[][] map;

        public byte this[int x, int y]
        {
            get
            {
                return map[y][x];
            }
            set
            {
                map[y][x] = value;
            }
        }

        public AStar(byte[][] map)
        {
            this.map = map;
        }

        public List<(int, int)> FindPath(int startX, int startY, int goalX, int goalY)
        {
            var openSet = new HashSet<(int, int)>();
            var closedSet = new HashSet<(int, int)>();
            var cameFrom = new Dictionary<(int, int), (int, int)>();
            var gScore = new Dictionary<(int, int), int>();
            var fScore = new Dictionary<(int, int), int>();

            openSet.Add((startX, startY));
            gScore[(startX, startY)] = 0;
            fScore[(startX, startY)] = HeuristicCostEstimate((startX, startY), (goalX, goalY));

            while (openSet.Count > 0)
            {
                var current = GetNodeWithLowestFScore(openSet, fScore);
                if (current == (goalX, goalY))
                {
                    return ReconstructPath(cameFrom, current);
                }

                openSet.Remove(current);
                closedSet.Add(current);

                foreach (var direction in directions)
                {
                    var neighbor = (current.Item1 + direction.Item1, current.Item2 + direction.Item2);
                    if (IsOutOfBounds(neighbor) || IsWall(neighbor) || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    var tentativeGScore = gScore[current] + 1; // Assume a cost of 1 to move between squares
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                    else if (tentativeGScore >= gScore[neighbor])
                    {
                        continue;
                    }

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, (goalX, goalY));
                }
            }

            // No path was found
            return new List<(int, int)>();
        }

        private (int, int) GetNodeWithLowestFScore(HashSet<(int, int)> openSet, Dictionary<(int, int), int> fScore)
        {
            (int, int) lowestNode = (-1, -1);
            int lowestScore = int.MaxValue;
            foreach (var node in openSet)
            {
                if (fScore[node] < lowestScore)
                {
                    lowestNode = node;
                    lowestScore = fScore[node];
                }
            }
            return lowestNode;
        }

        private List<(int, int)> ReconstructPath(Dictionary<(int, int), (int, int)> cameFrom, (int, int) current)
        {
            var path = new List<(int, int)>();
            path.Add(current);
            while (cameFrom.TryGetValue(current, out current))
            {
                path.Add(current);
            }
            path.Reverse();
            return path;
        }

        private bool IsOutOfBounds((int, int) square)
        {
            return square.Item1 < 0 || square.Item1 >= map[0].Length || square.Item2 < 0 || square.Item2 >= map.Length;
        }

        private bool IsWall((int, int) square)
        {
            return this[square.Item1, square.Item2] != 0; // 1 represents a wall
        }

        private int HeuristicCostEstimate((int, int) from, (int, int) to)
        {
            // Manhattan distance is a common heuristic for grid-based maps
            return Math.Abs(from.Item1 - to.Item1) + Math.Abs(from.Item2 - to.Item2);
        }

        public byte[][] ToArray()
        {
            byte[][] output = new byte[map.Length][];
            for(int i = 0; i < map.Length; i++)
            {
                output[i] = new byte[map[i].Length];
                map[i].CopyTo(output[i], 0);
            }
            return output;
        }
    }
}