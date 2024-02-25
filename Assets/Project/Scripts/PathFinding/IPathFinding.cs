using System.Collections;
using System.Collections.Generic;
using FTG.HexMap;
using UnityEngine;

namespace FTG.PathFinding
{
    public interface IPathFinding
    {
        List<HexCell> FindPath(HexCell start, HexCell end);
    }
}
