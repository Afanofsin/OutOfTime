using System;
using System.Collections.Generic;
using NaughtyAttributes;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room settings")]
    [SerializeField] private List<Room> rooms;

    [SerializeField] private Room startRoom;
    [SerializeField] private List<Room> specialRooms;
    [SerializeField] private List<Room> bossRoom;
    [SerializeField] private int roomCount;

    [Header("Grid")]
    [SerializeField] private Grid grid;
    [SerializeField] private int size = 300;

    private Dictionary<Vector2Int, int> occupiedTiles;
    private List<ConnectionPoint> availableConnections;
    private List<ConnectionPoint> lastRoomConnections;
    private Vector2Int middleCoord;
    private System.Random random;

    void Awake()
    {
        occupiedTiles = new Dictionary<Vector2Int, int>();
        availableConnections = new List<ConnectionPoint>();
        middleCoord = new Vector2Int(size / 2, size / 2);
    }

    [Button]
    public void GenerateLevel()
    {
        ClearLevel();
        occupiedTiles = new();
        availableConnections = new();

        int seed = CreateGenerationSeed();
        random = new System.Random(seed);

        int tries = 0;
        int placedRooms = 0;
        int mainBranch = (int)(roomCount * 0.6f);

        Room firstRoom = startRoom;
        PlaceRoomAt(middleCoord, firstRoom);
        placedRooms++;

        for (int i = 0; i < roomCount; i++)
        {
            if (availableConnections.Count == 0)
            {
                Debug.LogWarning("No More Available Connections");
                return;
            }

            Vector2Int? placePos;
            Room room;
            if (mainBranch != placedRooms)
            {
                room = rooms[random.Next(0, rooms.Count)];
                placePos = TryPlaceRoom(room, lastRoomConnections);
            }
            else
            {
                room = rooms[random.Next(0, rooms.Count)];
                placePos = TryPlaceRoom(room, availableConnections);
            }

            if (placePos.HasValue)
            {
                PlaceRoomAt(placePos.Value, room);
                placedRooms++;
            }
            else
            {
                tries++;
                i--;
            }

            if (tries > 100)
            {
                Debug.LogWarning("Run out of tries");
                return;
            }
        }
        Debug.Log($"Generation complete tries : {tries}, rooms : {placedRooms}");
    }

    private Vector2Int? TryPlaceRoom(Room room, List<ConnectionPoint> connectionsList)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < connectionsList.Count; i++)
        {
            indices.Add(i);
        }
    
        for (int i = 0; i < indices.Count; i++)
        {
            int randIndex = random.Next(i, indices.Count);
            (indices[i], indices[randIndex]) = (indices[randIndex], indices[i]);
        }
        
        // Try to place at each connection point available

        foreach (var index in indices)
        {
            var globalPoint = connectionsList[index];
            if (globalPoint.isConnected)
                continue;

            List<ConnectionPoint> shuffledRoomConnections = new List<ConnectionPoint>(room.ConnectionPoints);
            for (int i = 0; i < shuffledRoomConnections.Count; i++)
            {
                int randIndex = random.Next(i, shuffledRoomConnections.Count);
                (shuffledRoomConnections[i], shuffledRoomConnections[randIndex]) 
                    = (shuffledRoomConnections[randIndex], shuffledRoomConnections[i]);
            }
            
            foreach (var roomPoint in shuffledRoomConnections)
            {
                if (AreConnectionsCompatible(globalPoint.direction, roomPoint.direction))
                {
                    Vector2Int potentialPos = globalPoint.localPosition - roomPoint.localPosition;
                    
                    if (CheckIfRoomFitsInGrid(potentialPos, room))
                    {
                        // Mark this connection as used
                        globalPoint.isConnected = true;
                        return potentialPos;
                    }
                }
            }
        }

        return null;
    }

    private bool AreConnectionsCompatible(Direction dir1, Direction dir2)
    {
        return (dir1 == Direction.East && dir2 == Direction.West) ||
               (dir1 == Direction.North && dir2 == Direction.South) ||
               (dir1 == Direction.West && dir2 == Direction.East) ||
               (dir1 == Direction.South && dir2 == Direction.North);
    }

    private bool CheckIfRoomFitsInGrid(Vector2Int pos, Room room)
    {
        int h = room.GetHeight;
        int w = room.GetWidth;
        int maxX = pos.x + w;
        int maxY = pos.y + h;

        if (pos.x < 0 || pos.y < 0 || maxX > size || maxY > size)
        {
            return false;
        }

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                int tileOccupied = room.occupiedTiles[(i * w) + j];
                if (tileOccupied == 0)
                    continue;

                Vector2Int checkPos = new Vector2Int(pos.x + j, pos.y + i);
                if (occupiedTiles.ContainsKey(checkPos))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void PlaceRoomAt(Vector2Int position, Room room)
    {
        PlaceRoom(position, room);
        Vector3 worldPos = grid.CellToWorld(new Vector3Int(position.x, position.y, 0));
        Instantiate(room, worldPos, Quaternion.identity, this.transform);

        lastRoomConnections.Clear();
        foreach (ConnectionPoint connectionPoint in room.ConnectionPoints)
        {
            var worldPosConnectionPoint = new ConnectionPoint
            {
                localPosition = position + connectionPoint.localPosition,
                direction = connectionPoint.direction,
                isConnected = false
            };
            availableConnections.Add(worldPosConnectionPoint);
            lastRoomConnections.Add(worldPosConnectionPoint);
        }
    }

    private void PlaceRoom(Vector2Int coords, Room room)
    {
        int height = room.GetHeight;
        int width = room.GetWidth;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int tileType = room.occupiedTiles[(y * width) + x];
                if (tileType == 0)
                    continue;
                
                Vector2Int pos =  new Vector2Int(coords.x + x, coords.y + y);
                
                occupiedTiles[pos] = tileType;
            }
        }
    }

    [Button]
    private void ClearLevel()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        occupiedTiles?.Clear();
        availableConnections?.Clear();
    }

    private int CreateGenerationSeed()
    {
        string timeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        int hashedTime = timeNow.GetHashCode();
        return Math.Abs(hashedTime % 100000000);
    }
    
    [Button]
    public void VisualizeOccupancyOnGrid()
    {
        if (occupiedTiles == null || occupiedTiles.Count == 0)
        {
            Debug.LogError("No occupancy data!");
            return;
        }

        Grid grid = GetComponentInChildren<Grid>();
        if (grid == null)
        {
            Debug.LogError("No Grid found in children!");
            return;
        }

        // Remove old visualization if it exists
        ClearOccupancyVisualization();

        GameObject vizParent = new GameObject($"{name}_Grid_Occupancy_Visualization");
        vizParent.transform.SetParent(transform);
        vizParent.transform.localPosition = Vector3.zero;

        foreach (var kvp in occupiedTiles)
        {
            Vector2Int cell = kvp.Key;
            int tileType = kvp.Value;

            if (tileType == 0)
                continue;

            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.SetParent(vizParent.transform);

            Vector3Int cellPos = new Vector3Int(cell.x, cell.y, 0);
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPos);

            quad.transform.position = cellCenter;
            quad.transform.rotation = Quaternion.identity;

            Vector3 cellSize = grid.cellSize;
            quad.transform.localScale = new Vector3(
                cellSize.x * 0.9f,
                cellSize.y * 0.9f,
                1f
            );

            var renderer = quad.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = new Color(1f, 0f, 0f, 0.35f);
            renderer.material = mat;

            // Push forward so it renders above tiles
            quad.transform.position += Vector3.back * 0.1f;

            DestroyImmediate(quad.GetComponent<Collider>());
        }

        Debug.Log($"Visualized {occupiedTiles.Count} occupied grid cells.");
    }

    [Button]
    public void ClearOccupancyVisualization()
    {
        var viz = transform.Find($"{name}_Grid_Occupancy_Visualization");
        if (viz != null)
        {
            DestroyImmediate(viz.gameObject);
            Debug.Log("Cleared occupancy visualization.");
        }
    }
    
}