using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public GameObject nodePrefab;
    public Vector2 gridSize;
    public float nodeSpacing = 1f;
    public LayerMask obstacleLayer;
    public float nodeRadius = 0.5f;

    private Node[,] nodeGrid;

    void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        ClearGrid();

        nodeGrid = new Node[Mathf.RoundToInt(gridSize.x), Mathf.RoundToInt(gridSize.y)];
        Vector3 gridOrigin = transform.position;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 nodePosition = gridOrigin + new Vector3(x * nodeSpacing, 0, y * nodeSpacing);

                if (Physics.Raycast(nodePosition + Vector3.up * 10f, Vector3.down, out RaycastHit hit, Mathf.Infinity, ~obstacleLayer))
                {
                    nodePosition = hit.point;

                    GameObject nodeObject = Instantiate(nodePrefab, nodePosition, Quaternion.identity, transform);
                    Node node = nodeObject.GetComponent<Node>();

                    node.isWalkable = !Physics.CheckSphere(nodePosition, nodeRadius, obstacleLayer);

                    node.position = nodePosition;
                    nodeGrid[x, y] = node;
                }
                else
                {
                    nodeGrid[x, y] = null;
                }
            }
        }

        ConnectNodes();
    }

    private void ConnectNodes()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Node currentNode = nodeGrid[x, y];
                if (currentNode == null || !currentNode.isWalkable) continue;

                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int neighborX = x + dx;
                        int neighborY = y + dy;

                        if (neighborX >= 0 && neighborX < gridSize.x && neighborY >= 0 && neighborY < gridSize.y)
                        {
                            Node neighborNode = nodeGrid[neighborX, neighborY];
                            if (neighborNode == null || !neighborNode.isWalkable) continue;

                            currentNode.neighbors.Add(neighborNode);
                        }
                    }
                }
            }
        }
    }

    public void UpdateGrid()
    {
        if (nodeGrid == null) return;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Node node = nodeGrid[x, y];
                if (node == null) continue;

                node.isWalkable = !Physics.CheckSphere(node.position, nodeRadius, obstacleLayer);
            }
        }

        ConnectNodes();
    }

    private void Update()
    {
        UpdateGrid();
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}
