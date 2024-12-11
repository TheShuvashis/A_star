using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{
    public Transform start, target;
    NodeGrid grid;

    void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    void Update()
    {
        FindPath(start.position, target.position);
        Debug.Log("Pathfinding complete");
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!startNode.isWalkable || !targetNode.isWalkable)
        {
            Debug.LogWarning("Start or Target Node is unwalkable!");
            return;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                    continue;

                float heightDifference = Mathf.Abs(currentNode.worldPosition.y - neighbour.worldPosition.y);

                // Skip neighbors if step height is too large
                if (heightDifference > grid.maxStepHeight)
                    continue;

                // Skip neighbors if slope angle is too steep
                float distance = Vector3.Distance(
                    new Vector3(currentNode.worldPosition.x, 0, currentNode.worldPosition.z),
                    new Vector3(neighbour.worldPosition.x, 0, neighbour.worldPosition.z)
                );

                float slopeAngle = Mathf.Atan2(heightDifference, distance) * Mathf.Rad2Deg;
                if (slopeAngle > grid.maxSlopeAngle)
                    continue;

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + Mathf.RoundToInt(heightDifference * 10);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        grid.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
