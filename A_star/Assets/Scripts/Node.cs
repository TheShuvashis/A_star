using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 position;
    public List<Node> neighbors = new List<Node>();
    public bool isWalkable = false;

    private void Awake()
    {
        position = transform.position;
    }
}
