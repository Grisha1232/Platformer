using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    protected Transform seeker;   // Объект, который будет искать путь (моб)
    protected Transform target;   // Цель, к которой идет моб (игрок)

    protected NavigationGrid grid;
    private List<Node> foundPath;

    private List<Node> openList;    // Список нод для проверки
    private HashSet<Node> closedList;  // Список проверенных нод
    private float nodeSize;
    private Vector2 gridSize;

    void Awake() {
        grid = NavigationGrid.instance;
        nodeSize = grid.GetNodeSize();
        gridSize = grid.GetGridSize();
    }
#region Algorithm A*
    protected void FindPath(float jumpForce)
    {
        Node startNode = grid.GetNodeFromWorldPoint(seeker.position);
        Node endNode = grid.GetNodeFromWorldPoint(target.position);

        openList = new List<Node> { startNode };
        closedList = new HashSet<Node>();

        Node last = new Node(true, new Vector3(0, 0, 0), 0, 0);

        while (openList.Count > 0) {
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode) {
                RetracePath(startNode, endNode);
                return;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<Node> neighbors = grid.GetNeighbors(currentNode, jumpForce);
            foreach (Node neighbor in neighbors) {
                if (closedList.Contains(neighbor) || !neighbor.walkable) {
                    continue;
                }

                float newGCost = currentNode.gCost + Vector2.Distance(currentNode.worldPosition, neighbor.worldPosition);
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor)) {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Vector2.Distance(neighbor.worldPosition, endNode.worldPosition);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor)) {
                        openList.Add(neighbor);
                    }
                }
            }
            last = currentNode;
        }
        print("Path doesnt found");
        RetracePath(startNode, last);
    }

    protected void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        foundPath = path; // Передаем путь в скрипт движения моба
    }
#endregion

#region Helper functions for A*
    private Node GetLowestFCostNode(List<Node> nodes) {
        Node lowestFCostNode = nodes[0];
        foreach (Node node in nodes) {
            if (node.fCost < lowestFCostNode.fCost || 
                (node.fCost == lowestFCostNode.fCost && node.hCost < lowestFCostNode.hCost)) {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }
#endregion

#region Setters / Getters
    protected List<Node> GetPath() {
        return foundPath;
    }

    protected void SetTarget(Transform target) {
        this.target = target;
    }

    protected void SetSeeker(Transform seeker) {
        this.seeker = seeker;
    }
#endregion
}
