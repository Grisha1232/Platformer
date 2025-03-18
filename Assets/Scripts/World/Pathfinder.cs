using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    [HideInInspector]
    public static Pathfinder instance;
    private Tilemap map;
    private Tilemap platforms;

    public GameObject playerTarget;
    private Vector3Int targetPosition;
    private HashSet<Vector3Int> availableTilesPosition;
    private Dictionary<Vector3Int, Color> availableForGizmos;

    private Dictionary<Vector3Int, Direction> path;

    private Vector3Int targetTilePosition;

    private void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
        // setMap();
    }

    private static Tilemap FindTilemapByTag(string tag)
    {
        Tilemap[] allTilemaps = GameObject.FindObjectsOfType<Tilemap>();

        foreach (Tilemap tilemap in allTilemaps)
        {
            if (tilemap.tag == tag)
            {
                return tilemap;
            }
        }

        return null;
    }
   
    public void setMap() {
        map = FindTilemapByTag("Map");
        platforms = FindTilemapByTag("Platforms");
        path = new Dictionary<Vector3Int, Direction>();
        availableForGizmos = new Dictionary<Vector3Int, Color>();
        availableTilesPosition = new HashSet<Vector3Int>();
        BoundsInt.PositionEnumerator positions = map.cellBounds.allPositionsWithin;
        foreach (var position in positions) {
            if (map.GetTile(position) == null && map.GetTile(position - new Vector3Int(0, (int)map.cellSize.y)) != null) {
                availableTilesPosition.Add(position); 
            } else if (platforms != null && map.GetTile(position) == null && platforms.GetTile(position - new Vector3Int(0, (int)platforms.cellSize.y)) != null) {
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
            } 
            if ( availableTilesPosition.Contains(position + bias2) && !availableTilesPosition.Contains(position - bias2) 
            && map.GetTile(position - bias2) == null) {
                availableForGizmos[position - bias2] = Color.magenta;
                toAdd.Add(position - bias2);
            }
            if ( availableTilesPosition.Contains(position - bias2) && !availableTilesPosition.Contains(position + bias2) 
            && map.GetTile(position + bias2) == null) {
                availableForGizmos[position + bias2] = Color.magenta;
                toAdd.Add(position + bias2);
            }

            if (availableForGizmos.ContainsKey(position - bias2) && availableForGizmos[position - bias2] == Color.magenta) {
                int count = 0;
                Vector3Int position1 = position - bias2 - bias3;
                while (map.GetTile(position1) == null && count < 10) {
                    availableForGizmos[position1] = Color.blue;
                    toAdd.Add(position1);
                    position1 -= bias3;
                    count++;
                }
            }
            if (availableForGizmos.ContainsKey(position + bias2) && availableForGizmos[position + bias2] == Color.magenta) {
                int count = 0;
                Vector3Int position1 = position + bias2 - bias3;
                while (map.GetTile(position1) == null && count < 10) {
                    availableForGizmos[position1] = Color.blue;
                    toAdd.Add(position1);
                    position1 -= bias3;
                    count++;
                }
            }
                
        }
        availableTilesPosition = availableTilesPosition.Concat(toAdd).ToHashSet();
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.instance.sceneName == "Main Menu") {
            return;
        }
        if (map == null) {
            map = FindTilemapByTag("Map");
        }
        if (playerTarget.transform == null) {
            Debug.LogWarning("playerTarget.transform is null");
        }
        targetPosition = map.WorldToCell(playerTarget.transform.position);
        if (targetTilePosition == null || availableTilesPosition.Contains(targetPosition) && targetTilePosition != targetPosition) {
            findPathForAllTiles();
        }
    }

#region Methods to find path

    private void findPathForAllTiles() {
        
        Vector3Int biasX = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int biasY = new Vector3Int(0, (int)map.cellSize.y);


        Queue<(Vector3Int point, int upCount)> queue = new();
        Dictionary<Vector3Int, bool> visited = new();

        Vector3Int[] deltas = { biasY, -biasY, -biasX, biasX };
        Direction[] dirSymbols = { Direction.Down, Direction.Up, Direction.Right, Direction.Left };

        queue.Enqueue((targetPosition, 0));
        visited[targetPosition] = true;
        path[targetPosition] = Direction.None;

        while (queue.Count > 0)
        {
            var (current, upCount) = queue.Dequeue();

            for (int d = 0; d < deltas.GetLength(0); d++)
            {
                Vector3Int newTile = current + deltas[d];
                int newUpCount = dirSymbols[d] == Direction.Up ? upCount + 1 : 0;
                if (availableTilesPosition.Contains(newTile) && !visited.ContainsKey(newTile) && newUpCount <= 6)
                {
                    queue.Enqueue((newTile, newUpCount));
                    visited[newTile] = true;
                    path[newTile] = dirSymbols[d];
                }
            }
        }
    }

    public int getPathLength(Vector3 from) {
        return getPath2(from).Count;
    }

    public List<Vector3Int> getPath2(Vector3 from) {
        Vector3Int start = map.WorldToCell(from);

        Vector3Int biasX = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int biasY = new Vector3Int(0, (int)map.cellSize.y);

        if ( !availableTilesPosition.Contains(start) ) {
            if (availableTilesPosition.Contains(start - biasX)) {
                start -= biasX;
            } else if (availableTilesPosition.Contains(start + biasX)) {
                start += biasX;
            } else if  (availableTilesPosition.Contains(start - biasY)) {
                start -= biasY;
            } else if  (availableTilesPosition.Contains(start + biasY)) {
                start += biasY;
            }
        }
        if ( !availableTilesPosition.Contains(start) ) {
            return new();
        }


        List<Vector3Int> rv = new();
        var current = start;
        while (path.ContainsKey(current) && path[current] != Direction.None) {
            if (path[current] == Direction.Up) {
                current += biasY;
            } else if (path[current] == Direction.Down) {
                current -= biasY;
            } else if (path[current] == Direction.Right) {
                current += biasX;
            } else if (path[current] == Direction.Left) {
                current -= biasX;
            }
            rv.Add(current);
        }
        return rv;
    }

    public List<Vector3Int> getNextThreeTiles(Vector3 from) {
        Vector3Int start = map.WorldToCell(from);

        if (!availableTilesPosition.Contains(start)) {
            return new();
        }
        
        Vector3Int biasX = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int biasY = new Vector3Int(0, (int)map.cellSize.y);
        
        List<Vector3Int> rv = new();
        var current = start;
        while (rv.Count != 4) {
            if (path[current] == Direction.Up) {
                current += biasY;
            } else if (path[current] == Direction.Down) {
                current -= biasY;
            } else if (path[current] == Direction.Right) {
                current += biasX;
            } else if (path[current] == Direction.Left) {
                current -= biasX;
            }
            if (rv.Count == 0) {
                rv.Add(current);
            } else if (path[current] != Direction.Down && path[current] != Direction.Up) {
                rv.Add(current);
            } else if (path[current] == Direction.Up && path[rv[^1]] != Direction.Up) {
                rv.Add(current);
            } else if (path[current] == Direction.Down && path[rv[^1]] != Direction.Down) {
                rv.Add(current);
            }
        }
        return rv;
    }

    private Vector3Int GetCellWithLowestFScore(List<Vector3Int> openSet, Dictionary<Vector3Int, int> fScore)
    {
        return openSet.OrderBy(cell => fScore[cell]).First();
    }

    private List<Vector3Int> GetNeighbors(Vector3Int cell, Dictionary<Vector3Int, Vector3Int> cameFrom)
    {
        
        Vector3Int biasX = new Vector3Int((int)map.cellSize.x, 0);
        Vector3Int biasY = new Vector3Int(0, (int)map.cellSize.y);

        List<Vector3Int> neighbors = new List<Vector3Int>();

        // Проверка соседей по горизонтали
        Vector3Int leftNeighbor = cell - biasX;
        Vector3Int rightNeighbor = cell + biasX;

        if (availableTilesPosition.Contains(leftNeighbor))
            neighbors.Add(leftNeighbor);
        if (availableTilesPosition.Contains(rightNeighbor))
            neighbors.Add(rightNeighbor);

        // Проверка прыжков вверх
        for (int i = 1; i <= 6; i++) // Максимальная высота прыжка 6 клеток
        {
            Vector3Int jumpNeighbor = cell + new Vector3Int(0, i * (int)map.cellSize.y);
            if (availableTilesPosition.Contains(jumpNeighbor))
            {
                neighbors.Add(jumpNeighbor);
                break; // Прерываем цикл, если нашли доступную клетку для прыжка
            } else {
            }
        }
        return neighbors;
    }

    private int Heuristic(Vector3Int a, Vector3Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return dx + dy; // Манхэттенское расстояние
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> path = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

#endregion

#region GIZMOS
    void OnDrawGizmos() {
        if (map == null) {
            return;
        }
        BoundsInt.PositionEnumerator positions = map.cellBounds.allPositionsWithin;

        Vector3 bias = new Vector3(map.cellSize.x / 2, map.cellSize.y / 2);
        // foreach (var position in positions) {

        //     if (map.GetTile(position) != null){
        //         Gizmos.color = Color.red;
        //     } else {
        //         continue;
        //     }
            
        //     Gizmos.DrawWireCube(position + bias, map.cellSize);
        // }

        // if (platforms != null) {
        //     BoundsInt.PositionEnumerator positions1 = platforms.cellBounds.allPositionsWithin;
        //     foreach (var position in positions1) {

        //         if (platforms.GetTile(position) != null){
        //             Gizmos.color = Color.red;
        //         } else {
        //             continue;
        //         }
                
        //         Gizmos.DrawWireCube(position + bias, map.cellSize);
        //     }
        // }

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
        Gizmos.DrawWireCube(map.WorldToCell(playerTarget.transform.position) + bias, map.cellSize);

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

    public void DrawPath(Vector3 from) {
        Vector3 bias = new Vector3(map.cellSize.x / 2, map.cellSize.y / 2);
        var listDiretions = getNextThreeTiles(from);
        if (listDiretions == null || listDiretions.Count == 0) {
            return;
        }

        Gizmos.color = Color.green;
        for (int i = 0; i < listDiretions.Count - 1; i++) {
            Gizmos.DrawWireCube(listDiretions[i] + bias, map.cellSize);
        }
    }
#endregion
}


