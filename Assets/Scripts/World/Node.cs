using System.ComponentModel;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Node
{
    public bool walkable;           // Проходима ли клетка
    public Vector3 worldPosition;   // Мировая позиция клетки
    public int gridX, gridY;        // Координаты клетки в сетке

    public float gCost;             // Расстояние от стартовой ноды
    public float hCost;             // Расстояние до конечной ноды
    public float fCost => gCost + hCost; // Общий вес
    public Node parent;             // Родительская клетка (для восстановления пути)


    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        this.gCost = 0;
        this.hCost = 0;
        this.parent = null;
    }
    public override string ToString()
    {
        return "x= " + worldPosition.x + "; y=" + worldPosition.y;
    }

}
