using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    NodeGrid grid;

    private void Awake()
    {
        grid = GetComponent<NodeGrid>();
    }

    private void ConstructPath(Vector3 startPosition, Vector3 endPosition)
    {
        //Set start position and end position
        Node startNode = grid.GetNodeAtWorldPosition(startPosition);
        Node targetNode = grid.GetNodeAtWorldPosition(endPosition);

        //Initialize open set and closed set
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++) {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                return;
            }

            foreach (Node neighbor in grid.GetNieghbors(currentNode))
            {
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                {
                    continue;
                }
            }
        }
    }

    
}
