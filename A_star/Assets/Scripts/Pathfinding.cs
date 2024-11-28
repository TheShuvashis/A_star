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

    private void FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        Node startNode = grid.GetNodeAtWorldPosition(startPosition);
        Node targetNode = grid.GetNodeAtWorldPosition(endPosition);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
    }
}
