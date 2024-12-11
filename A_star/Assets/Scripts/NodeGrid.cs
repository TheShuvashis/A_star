using UnityEngine;
using System.Collections.Generic;

public class NodeGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public LayerMask groundMask; // Layer for detecting ground
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float maxSlopeAngle = 45f; // Maximum slope angle in degrees
    public float maxStepHeight = 0.5f; // Maximum step height
    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public List<Node> path;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

                // Raycast to determine ground height
                if (Physics.Raycast(worldPoint + Vector3.up * 10, Vector3.down, out RaycastHit hit, 20f, groundMask))
                {
                    worldPoint = hit.point; // Set node height
                }

                // Check walkability
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }

        CheckNodeSlopesAndSteps();
    }

    void CheckNodeSlopesAndSteps()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = grid[x, y];
                if (!node.isWalkable) continue;

                foreach (Node neighbour in GetNeighbours(node))
                {
                    float heightDifference = Mathf.Abs(node.worldPosition.y - neighbour.worldPosition.y);

                    // Check step height
                    if (heightDifference > maxStepHeight)
                    {
                        node.isWalkable = false;
                        break;
                    }

                    // Check slope
                    float distance = Vector3.Distance(
                        new Vector3(node.worldPosition.x, 0, node.worldPosition.z),
                        new Vector3(neighbour.worldPosition.x, 0, neighbour.worldPosition.z)
                    );

                    float slopeAngle = Mathf.Atan2(heightDifference, distance) * Mathf.Rad2Deg;
                    if (slopeAngle > maxSlopeAngle)
                    {
                        node.isWalkable = false;
                        break;
                    }
                }
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    void UpdateObstacles()
    {
        foreach (Node node in grid)
        {
            bool walkable = !(Physics.CheckSphere(node.worldPosition, nodeRadius, unwalkableMask));
            node.isWalkable = walkable;
        }
    }

    private void Update()
    {
        UpdateObstacles();
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.isWalkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, new Vector3(.1f, .1f, .1f));
            }

            if (path != null)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i].worldPosition, path[i + 1].worldPosition);
                }
            }
        }
    }
}
