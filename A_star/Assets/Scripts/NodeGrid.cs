using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public GameObject nodePrefab;
    public Vector2 gridSize;
    public float nodeSpacing = 1f;
    public LayerMask obstacleLayer;

    private Node[,] nodeGrid;

    private void Start()
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
                GameObject nodeObject = Instantiate(nodePrefab, nodePosition, Quaternion.identity, transform);

                Node node = nodeObject.GetComponent<Node>();
                node.isWalkable = !Physics.CheckSphere(nodePosition, nodeSpacing / 2f, obstacleLayer);
                nodeGrid[x, y] = node;
            }
        }

        ConnectNodes();
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void ConnectNodes()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Node currentNode = nodeGrid[x, y];
                if (!currentNode.isWalkable) continue;

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
                            if (neighborNode.isWalkable)
                                currentNode.neighbors.Add(neighborNode);
                        }
                    }
                }
            }
        }
    }
}
