using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{

    
    public enum Direction {
        Up,
        Down,
        Right,
        Left,
        None
    }

    Pathfinder instance;
    public Tilemap map;

    public Transform target;

    public Transform from;

    private Vector3Int targetPosition;
    private HashSet<Vector3Int> availableTilesPosition;
    private Dictionary<Vector3Int, Color> availableForGizmos;

    private Dictionary<Vector3Int, Direction> path;

    private float cooldownRefresh = 1f;
    private float cooldownCounter = 2f;

    void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        path = new Dictionary<Vector3Int, Direction>();
        availableForGizmos = new Dictionary<Vector3Int, Color>();
        availableTilesPosition = new HashSet<Vector3Int>();
        BoundsInt.PositionEnumerator positions = map.cellBounds.allPositionsWithin;
        foreach (var position in positions) {
            if (map.GetTile(position) == null && map.GetTile(position - new Vector3Int(0, (int)map.cellSize.y)) != null) {
                availableTilesPosition.Add(position); 
            } else {
                continue;
            }
        }
    
        Vector3Int bias2 = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int bias3 = new Vector3Int(0, (int)map.cellSize.y);
        HashSet<Vector3Int> toAdd = new HashSet<Vector3Int>();
        foreach (var position in availableTilesPosition) {
            availableForGizmos[position] = Color.green;
            if ( availableTilesPosition.Contains(position + bias2) && availableTilesPosition.Contains(position - bias2) ) {
                continue;
            } else if ( availableTilesPosition.Contains(position + bias2) && !availableTilesPosition.Contains(position - bias2) 
            && map.GetTile(position - bias2) == null ) {
                availableForGizmos[position - bias2] = Color.magenta;
                toAdd.Add(position - bias2);
            } else if ( availableTilesPosition.Contains(position - bias2) && !availableTilesPosition.Contains(position + bias2) 
            && map.GetTile(position + bias2) == null ) {
                availableForGizmos[position + bias2] = Color.magenta;
                toAdd.Add(position + bias2);
            }

            if (availableForGizmos.ContainsKey(position - bias2) && availableForGizmos[position - bias2] == Color.magenta) {

                Vector3Int position1 = position - bias2 - bias3;
                while (map.GetTile(position1) == null || availableForGizmos.ContainsKey(position1) && availableForGizmos[position1] != Color.green) {
                    availableForGizmos[position1] = Color.blue;
                    toAdd.Add(position1);
                    position1 -= bias3;
                }
            }
            if (availableForGizmos.ContainsKey(position + bias2) && availableForGizmos[position + bias2] == Color.magenta) {

                Vector3Int position1 = position + bias2 - bias3;
                while (map.GetTile(position1) == null || availableForGizmos.ContainsKey(position1) && availableForGizmos[position1] != Color.green) {
                    availableForGizmos[position1] = Color.blue;
                    toAdd.Add(position1);
                    position1 -= bias3;
                }
            }
                
        }
        availableTilesPosition.AddRange(toAdd);

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        cooldownCounter += Time.deltaTime;
        if (cooldownCounter > cooldownRefresh) {
            cooldownCounter = 0;
            
            targetPosition = map.WorldToCell(target.position);
            findPathForAllTiles();
        }
    }

#region Methods to find path

    private void findPathForAllTiles() {
        
        Vector3Int biasX = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int biasY = new Vector3Int(0, (int)map.cellSize.y);


        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, bool> visited = new();

        Vector3Int[] deltas = { biasY, -biasY, -biasX, biasX };
        Direction[] dirSymbols = { Direction.Down, Direction.Up, Direction.Right, Direction.Left };

        queue.Enqueue(targetPosition);
        visited[targetPosition] = true;
        path[targetPosition] = Direction.None;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            for (int d = 0; d < deltas.GetLength(0); d++)
            {
                Vector3Int newTile = current + deltas[d];
                if (availableTilesPosition.Contains(newTile) && !visited.ContainsKey(newTile))
                {
                    queue.Enqueue(newTile);
                    visited[newTile] = true;
                    path[newTile] = dirSymbols[d];
                }
            }
        }
    }

    public int getPathLength(Vector3 from) {
        return getPath(from).Count;
    }

    public List<Direction> getPath(Vector3 from) {
        List<Direction> uniquePath = new();

        Vector3Int biasX = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int biasY = new Vector3Int(0, (int)map.cellSize.y);

        Vector3Int fromPosition = map.WorldToCell(from);
        Vector3Int currentPosition = fromPosition;
        while (path[currentPosition] != Direction.None) {

        }
        return uniquePath;
    }
#endregion

#region GIZMOS
    void OnDrawGizmos() {
        BoundsInt.PositionEnumerator positions = map.cellBounds.allPositionsWithin;

        Vector3 bias = new Vector3(map.cellSize.x / 2, map.cellSize.y / 2);
        foreach (var position in positions) {

            if (map.GetTile(position) != null){
                Gizmos.color = Color.red;
            } else {
                continue;
            }
            
            Gizmos.DrawWireCube(position + bias, map.cellSize);
        }

        // if (availableForGizmos != null) {
        //     foreach (KeyValuePair<Vector3Int, Color> value in availableForGizmos) {
        //         Gizmos.color = value.Value;
        //         Gizmos.DrawWireCube(value.Key + bias, map.cellSize);
        //     }
        // }
        // if (availableTilesPosition != null) {
        //     foreach(var tile in availableTilesPosition) {
        //         Gizmos.color = Color.blue;
        //         Gizmos.DrawWireCube(tile + bias, map.cellSize);
        //     }
        // }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(map.WorldToCell(target.position) + bias, map.cellSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(map.WorldToCell(from.position) + bias, map.cellSize);

        Gizmos.color = Color.blue;
        if (path != null) {
            foreach (var tile in path) {
                DrawArrow(tile.Key + bias, map.cellSize, tile.Value);
            }
        }

    }

    private void DrawArrow(Vector3 center, Vector2 size, Direction direction)
    {
        // Центр квадрата
        Vector3 start = center;

        // Длина стрелки и ее размер относительно стороны квадрата
        float arrowLength = Mathf.Min(size.x, size.y) * 0.5f;
        float arrowWidth = arrowLength * 0.2f;

        // Направление стрелки
        Vector3 end = center;
        Vector3 leftWing, rightWing;

        switch (direction)
        {
            case Direction.Up:
                end += Vector3.up * arrowLength;
                leftWing = end + (Vector3.left + Vector3.down) * arrowWidth;
                rightWing = end + (Vector3.right + Vector3.down) * arrowWidth;
                break;

            case Direction.Down:
                end += Vector3.down * arrowLength;
                leftWing = end + (Vector3.left + Vector3.up) * arrowWidth;
                rightWing = end + (Vector3.right + Vector3.up) * arrowWidth;
                break;

            case Direction.Left:
                end += Vector3.left * arrowLength;
                leftWing = end + (Vector3.up + Vector3.right) * arrowWidth;
                rightWing = end + (Vector3.down + Vector3.right) * arrowWidth;
                break;

            case Direction.Right:
                end += Vector3.right * arrowLength;
                leftWing = end + (Vector3.up + Vector3.left) * arrowWidth;
                rightWing = end + (Vector3.down + Vector3.left) * arrowWidth;
                break;

            default:
                return;
        }

        // Рисуем стрелку
        Gizmos.DrawLine(start, end);
        Gizmos.DrawLine(end, leftWing);
        Gizmos.DrawLine(end, rightWing);
    }
#endregion
}


