using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationGrid : MonoBehaviour
{
    
    public Vector3 bottomLeft;
    public Vector3 topLeft;
    public Vector3 topRight;
    public Vector3 bottomRight;
    public float nodeSize;          // Размер одной клетки
    public LayerMask unwalkableLayer; // Слой для проверки препятствий

    private Node[,] grid;            // Двумерный массив узлов
    private int gridSizeX, gridSizeY; // Размеры сетки в узлах (кол-во клеток по X и Y)

    public static NavigationGrid instance;
    private void Awake()
    {
        if ( instance == null ) {
            instance = this;
            CreateGrid();
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void CreateGrid()
    {
        // Вычисляем размеры сетки
        gridSizeX = Mathf.RoundToInt(Vector3.Distance(bottomLeft, bottomRight) / nodeSize);
        gridSizeY = Mathf.RoundToInt(Vector3.Distance(bottomLeft, topLeft) / nodeSize);

        // Инициализация сетки
        grid = new Node[gridSizeX, gridSizeY];

        // Создание каждой клетки в сетке
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Мировая позиция каждой клетки
                Vector3 worldPoint = GetWorldPosition(x, y);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeSize / 2, unwalkableLayer);
                
                // Создаём узел
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Получение мировой позиции клетки с учётом её координат в сетке
    public Vector3 GetWorldPosition(int x, int y)
    {
        // Для этого можно просто сдвигать нижнюю точку по размерам клеток
        return new Vector3(bottomLeft.x + x * nodeSize, bottomLeft.y + y * nodeSize, 0);
    }


    // Получение узла из мировых координат
    public Node GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        // Рассчитываем координаты клетки в сетке, деля координаты на размер клетки
        float percentX = (worldPosition.x - bottomLeft.x) / (topRight.x - bottomLeft.x);
        float percentY = (worldPosition.y - bottomLeft.y) / (topLeft.y - bottomLeft.y);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);


        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbors(Node node, float jumpForce)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    Node neighbor = grid[checkX, checkY];
                    if (neighbor != null && neighbor.walkable) {
                        // Учитываем ограничения по прыжкам и горизонтальной досягаемости
                        float verticalDistance = Mathf.Abs(neighbor.worldPosition.y - node.worldPosition.y);

                        if (verticalDistance > jumpForce) {
                            continue; // Если враг не может добраться
                        }

                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    void OnDrawGizmosSelected()
    {


        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red; // Проходимые клетки — белые, непроходимые — красные
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeSize - 0.1f));
            }
        }
    }

    public float GetNodeSize() {
        return nodeSize;
    }
    
    public Vector2 GetGridSize() {
        return new Vector2(gridSizeX, gridSizeY);
    }
}
