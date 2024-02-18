using System.Collections.Generic;
using UnityEngine;

namespace FTG.HexMap
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        private List<Color> colors;
        private Mesh        hexMesh;

        private MeshCollider  meshCollider;
        private List<int>     triangles;
        private List<Vector3> vertices;

        private void Awake()
        {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            meshCollider                    = gameObject.AddComponent<MeshCollider>();

            hexMesh.name = "Hex Mesh";
            vertices     = new List<Vector3>();
            colors       = new List<Color>();
            triangles    = new List<int>();
        }

        public void Triangulate(HexCell[] cells)
        {
            hexMesh.Clear();
            vertices.Clear();
            colors.Clear();
            triangles.Clear();

            foreach (var cell in cells)
                Triangulate(cell);

            hexMesh.vertices  = vertices.ToArray();
            hexMesh.colors    = colors.ToArray();
            hexMesh.triangles = triangles.ToArray();
            hexMesh.RecalculateNormals();

            meshCollider.sharedMesh = hexMesh;
        }

        private void Triangulate(HexCell cell)
        {
            for (var d = HexDirection.NE; d <= HexDirection.NW; d++) Triangulate(d, cell);
        }

        private void Triangulate(HexDirection direction, HexCell cell)
        {
            var center = cell.transform.localPosition;
            var v1     = center + HexMetrics.GetFirstSolidCorner(direction);
            var v2     = center + HexMetrics.GetSecondSolidCorner(direction);

            AddTriangle(center, v1, v2);
            AddTriangleColor(cell.Color);

            if (direction <= HexDirection.SE) TriangulateConnection(direction, cell, v1, v2);
        }

        private void TriangulateConnection(HexDirection direction, HexCell cell, Vector3 v1, Vector3 v2)
        {
            var neighbor = cell.GetNeighbor(direction);
            if (neighbor == null) return;

            var bridge  = HexMetrics.GetBridge(direction);
            var v3      = v1 + bridge;
            var v4      = v2 + bridge;
            v3.y = v4.y = neighbor.Elevation * HexMetrics.elevationStep;

            if (cell.GetEdgeType(direction) == HexEdgeType.Slope)
            {
                TriangulateEdgeTerraces(v1, v2, cell, v3, v4, neighbor);
            }
            else
            {
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(cell.Color, neighbor.Color);
            }

            var nextNeighbor = cell.GetNeighbor(direction.Next());
            if (direction <= HexDirection.E && nextNeighbor != null)
            {
                var v5 = v2 + HexMetrics.GetBridge(direction.Next());
                v5.y = nextNeighbor.Elevation * HexMetrics.elevationStep;

                if (cell.Elevation <= neighbor.Elevation)
                {
                    if (cell.Elevation <= nextNeighbor.Elevation)
                        TriangulateCorner(v2, cell, v4, neighbor, v5, nextNeighbor);
                    else
                        TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
                }
                else if (neighbor.Elevation <= nextNeighbor.Elevation)
                {
                    TriangulateCorner(v4, neighbor, v5, nextNeighbor, v2, cell);
                }
                else
                {
                    TriangulateCorner(v5, nextNeighbor, v2, cell, v4, neighbor);
                }
            }
        }

        private void TriangulateEdgeTerraces(
            Vector3 beginLeft, Vector3 beginRight, HexCell beginCell,
            Vector3 endLeft, Vector3 endRight, HexCell endCell
        )
        {
            var v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, 1);
            var v4 = HexMetrics.TerraceLerp(beginRight, endRight, 1);
            var c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, 1);

            AddQuad(beginLeft, beginRight, v3, v4);
            AddQuadColor(beginCell.Color, c2);

            for (var i = 2; i < HexMetrics.terraceSteps; i++)
            {
                var v1 = v3;
                var v2 = v4;
                var c1 = c2;
                v3 = HexMetrics.TerraceLerp(beginLeft, endLeft, i);
                v4 = HexMetrics.TerraceLerp(beginRight, endRight, i);
                c2 = HexMetrics.TerraceLerp(beginCell.Color, endCell.Color, i);
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2);
            }

            AddQuad(v3, v4, endLeft, endRight);
            AddQuadColor(c2, endCell.Color);
        }

        private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        private void AddTriangleColor(Color color)
        {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        private void AddTriangleColor(Color c1, Color c2, Color c3)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
        }

        private void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            var vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        private void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
        {
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c3);
            colors.Add(c4);
        }

        private void AddQuadColor(Color c1, Color c2)
        {
            colors.Add(c1);
            colors.Add(c1);
            colors.Add(c2);
            colors.Add(c2);
        }

#region TriangulateCorner

        private void TriangulateCorner(
            Vector3 bottom, HexCell bottomCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var leftEdgeType  = bottomCell.GetEdgeType(leftCell);
            var rightEdgeType = bottomCell.GetEdgeType(rightCell);

            if (leftEdgeType == HexEdgeType.Slope)
            {
                if (rightEdgeType == HexEdgeType.Slope)
                    TriangulateCornerTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
                else if (rightEdgeType == HexEdgeType.Flat)
                    TriangulateCornerTerraces(left, leftCell, right, rightCell, bottom, bottomCell);
                else
                    TriangulateCornerTerracesCliff(bottom, bottomCell, left, leftCell, right, rightCell);
            }
            else if (rightEdgeType == HexEdgeType.Slope)
            {
                if (leftEdgeType == HexEdgeType.Flat)
                    TriangulateCornerTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                else
                    TriangulateCornerCliffTerraces(bottom, bottomCell, left, leftCell, right, rightCell);
            }
            else if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                if (leftCell.Elevation < rightCell.Elevation)
                    TriangulateCornerCliffTerraces(right, rightCell, bottom, bottomCell, left, leftCell);
                else
                    TriangulateCornerTerracesCliff(left, leftCell, right, rightCell, bottom, bottomCell);
            }
            else
            {
                AddTriangle(bottom, left, right);
                AddTriangleColor(bottomCell.Color, leftCell.Color, rightCell.Color);
            }
        }

        private void TriangulateCornerTerraces(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell)
        {
            var v3 = HexMetrics.TerraceLerp(begin, left, 1);
            var v4 = HexMetrics.TerraceLerp(begin, right, 1);
            var c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);
            var c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, 1);

            AddTriangle(begin, v3, v4);
            AddTriangleColor(beginCell.Color, c3, c4);

            for (var i = 2; i < HexMetrics.terraceSteps; i++)
            {
                var v1 = v3;
                var v2 = v4;
                var c1 = c3;
                var c2 = c4;
                v3 = HexMetrics.TerraceLerp(begin, left, i);
                v4 = HexMetrics.TerraceLerp(begin, right, i);
                c3 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                c4 = HexMetrics.TerraceLerp(beginCell.Color, rightCell.Color, i);
                AddQuad(v1, v2, v3, v4);
                AddQuadColor(c1, c2, c3, c4);
            }

            AddQuad(v3, v4, left, right);
            AddQuadColor(c3, c4, leftCell.Color, rightCell.Color);
        }

        private void TriangulateCornerTerracesCliff(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var b             = 1f / (rightCell.Elevation - beginCell.Elevation);
            if (b < 0) b      = -b;
            var boundary      = Vector3.Lerp(begin, right, b);
            var boundaryColor = Color.Lerp(beginCell.Color, rightCell.Color, b);

            TriangulateBoundaryTriangle(begin, beginCell, left, leftCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangle(left, right, boundary);
                AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        private void TriangulateCornerCliffTerraces(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 right, HexCell rightCell
        )
        {
            var b             = 1f / (leftCell.Elevation - beginCell.Elevation);
            if (b < 0) b      = -b;
            var boundary      = Vector3.Lerp(begin, left, b);
            var boundaryColor = Color.Lerp(beginCell.Color, leftCell.Color, b);

            TriangulateBoundaryTriangle(right, rightCell, begin, beginCell, boundary, boundaryColor);

            if (leftCell.GetEdgeType(rightCell) == HexEdgeType.Slope)
            {
                TriangulateBoundaryTriangle(left, leftCell, right, rightCell, boundary, boundaryColor);
            }
            else
            {
                AddTriangle(left, right, boundary);
                AddTriangleColor(leftCell.Color, rightCell.Color, boundaryColor);
            }
        }

        private void TriangulateBoundaryTriangle(
            Vector3 begin, HexCell beginCell,
            Vector3 left, HexCell leftCell,
            Vector3 boundary, Color boundaryColor
        )
        {
            var v2 = HexMetrics.TerraceLerp(begin, left, 1);
            var c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, 1);

            AddTriangle(begin, v2, boundary);
            AddTriangleColor(beginCell.Color, c2, boundaryColor);

            for (var i = 2; i < HexMetrics.terraceSteps; i++)
            {
                var v1 = v2;
                var c1 = c2;
                v2 = HexMetrics.TerraceLerp(begin, left, i);
                c2 = HexMetrics.TerraceLerp(beginCell.Color, leftCell.Color, i);
                AddTriangle(v1, v2, boundary);
                AddTriangleColor(c1, c2, boundaryColor);
            }

            AddTriangle(v2, left, boundary);
            AddTriangleColor(c2, leftCell.Color, boundaryColor);
        }

#endregion TriangulateCorner
    }
}