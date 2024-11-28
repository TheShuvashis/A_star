using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public GameObject nodePrefab;
    public Vector2 gridSize;
    public float nodeSpacing = 1f;
    public float nodeRadius = 0.5f;
    public LayerMask obstacleLayer;
    public float maxSlopeAngle = 45f;
    public float stepHeight = 0.5f;

    [Header("Debug")]
    public Transform playerTransform;
    private Node[,] nodeGrid;
    private int gridSizeX, gridSizeY;
    private const float raycastHeight = 10f;

    void Start()
    {
        // Calculate the number of nodes based on grid size and node radius
        gridSizeX = Mathf.RoundToInt(gridSize.x / (nodeRadius * 2));
        gridSizeY = Mathf.RoundToInt(gridSize.y / (nodeRadius * 2));

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearGrid();

        nodeGrid = new Node[gridSizeX, gridSizeY];
        Vector3 gridOrigin = transform.position;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 nodePosition = gridOrigin + new Vector3(x * nodeSpacing, 0, y * nodeSpacing);

                if (Physics.Raycast(nodePosition + Vector3.up * raycastHeight, Vector3.down, out RaycastHit hit, Mathf.Infinity, ~obstacleLayer))
                {
                    nodePosition = hit.point;

                    if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                    {
                        continue;
                    }

                    if (y > 0 && Mathf.Abs(nodePosition.y - nodeGrid[x, y - 1].position.y) > stepHeight)
                    {
                        continue;
                    }

                    GameObject nodeObject = Instantiate(nodePrefab, nodePosition, Quaternion.identity, transform);
                    Node node = nodeObject.GetComponent<Node>();

                    node.isWalkable = !Physics.CheckSphere(nodePosition, nodeRadius, obstacleLayer);
                    node.position = nodePosition;
                    nodeGrid[x, y] = node;
                }
            }
        }
        ConnectAllNodes();
    }

    private void ConnectAllNodes()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = nodeGrid[x, y];
                if (node != null && node.isWalkable)
                {
                    UpdateNodeConnections(node, x, y);
                }
            }
        }
    }

    private void UpdateNodeConnections(Node node, int x, int y)
    {
        node.neighbors.Clear();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int neighborX = x + dx;
                int neighborY = y + dy;

                if (neighborX >= 0 && neighborX < gridSizeX && neighborY >= 0 && neighborY < gridSizeY)
                {
                    Node neighborNode = nodeGrid[neighborX, neighborY];
                    if (neighborNode != null && neighborNode.isWalkable)
                    {
                        node.neighbors.Add(neighborNode);
                    }
                }
            }
        }
    }

    public void UpdateGrid()
    {
        if (nodeGrid == null) return;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node node = nodeGrid[x, y];
                if (node != null)
                {
                    node.isWalkable = !Physics.CheckSphere(node.position, nodeRadius, obstacleLayer);

                    if (Physics.Raycast(node.position + Vector3.up * raycastHeight, Vector3.down, out RaycastHit hit))
                    {
                        if (Vector3.Angle(hit.normal, Vector3.up) > maxSlopeAngle)
                        {
                            node.isWalkable = false;
                        }
                    }
                    UpdateNodeConnections(node, x, y);
                }
            }
        }
    }


    public Node GetNodeAtGridPosition(int x, int y)
    {
        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            return nodeGrid[x, y];
        }
        return null;
    }

    public Node GetNodeAtWorldPosition(Vector3 worldPosition)
    {
        //Convert world position to grid coordinates
        int x = Mathf.RoundToInt((worldPosition.x - transform.position.x) / nodeSpacing);
        int y = Mathf.RoundToInt((worldPosition.z - transform.position.z) / nodeSpacing);
        return GetNodeAtGridPosition(x, y);
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void Update()
    {

    }
}
