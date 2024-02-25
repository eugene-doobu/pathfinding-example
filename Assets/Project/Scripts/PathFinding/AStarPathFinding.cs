using System;
using System.Collections;
using System.Collections.Generic;
using FTG.HexMap;
using UnityEngine;
using Utils;

namespace FTG.PathFinding
{
    public class AStarPathFinding : IPathFinding
    {
        private HexMapController _hexMapController;
        
        private class PQNode : IComparable<PQNode>
        {
            public HexCell Cell;
            public int     F;
            public int     G;

            public int CompareTo(PQNode other)
            {
                if (F == other.F)
                    return 0;
                return F < other.F ? 1 : -1;
            }
        }
        
        public void SetHexMapController(HexMapController hexMapController)
        {
            _hexMapController = hexMapController;
        }

        public List<HexCell> FindPath(HexCell start, HexCell end)
        {
            var pq       = new PriorityQueue<HexCell, int>();
            var cameFrom = new Dictionary<HexCell, HexCell>();
            var openList = new Dictionary<HexCell, int>();
            var distance = new Dictionary<HexCell, int>();
            var closed   = new HashSet<HexCell>();
            
            foreach (var cell in _hexMapController.HexGrid.Cells)
                openList[cell] = int.MaxValue;
            
            var startG = 0;
            var startH = Heuristic(start, end);
            var startF = startG + startH;
            
            pq.Enqueue(start, startF);
            distance[start] = startG;
            openList[start] = startH;
            cameFrom[start] = start;
            
            while (pq.Count > 0)
            {
                var current = pq.Dequeue();
                if (current == end)
                    break;
                
                if (closed.Contains(current))
                    continue;
                    
                closed.Add(current);
                
                for (var d = HexDirection.NE; d <= HexDirection.NW; d++)
                {
                    var neighbor = current.GetNeighbor(d);
                    if (neighbor == null || cameFrom.ContainsKey(neighbor))
                        continue;
                    if (closed.Contains(neighbor))
                        continue;
                    if (neighbor.Color == _hexMapController.Colors[3])
                        continue;
                    
                    var g = distance[current] + 1;
                    var h = Heuristic(neighbor, end);
                    var f = g + h;

                    if (openList.ContainsKey(neighbor) && g + h >= openList[neighbor]) 
                        continue;
                    
                    neighbor.Color = _hexMapController.Colors[2];
                    distance[neighbor] = g;
                    openList[neighbor] = f;
                    pq.Enqueue(neighbor, f);
                    cameFrom[neighbor] = current;
                }
            }
            
            var path = new List<HexCell>();
            if (!cameFrom.ContainsKey(end))
                return path;
            
            for (var c = end; c != start; c = cameFrom[c])
                path.Add(c);
            
            path.Add(start);
            path.Reverse();
            return path;
        }
        
        private int Heuristic(HexCell a, HexCell b)
        {
            return a.DistanceTo(b);
        }

        public void ClearCellState(HexCell cell)
        {
            if (cell.Color != _hexMapController.Colors[3])
                cell.Color = Color.white;
        }
    }
}
