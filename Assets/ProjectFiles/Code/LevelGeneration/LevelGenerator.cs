using System;
using System.Collections.Generic;
using System.Linq;
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
    private Vector2Int middleCoord;
    private System.Random random;

    [SerializeField] private float BASIC_PROBABILITY = 0.10f;
    [SerializeField] private float MAIN_BRANCH_LENGTH = 0.5f;
    private float probabilityToCloseConnection;
    
    private List<ConnectionPoint> allConnections;
    private List<ConnectionPoint> lastPlacedRoomConnections;
    private List<ConnectionPoint> branchStartConnections;
    private HashSet<Vector2Int> specialRoomPositions = new HashSet<Vector2Int>();
    private IEnumerable<ConnectionPoint> OpenConnections =>
        allConnections.Where(c => c.connectionState == ConnectionState.Open);
    private IEnumerable<ConnectionPoint> OpenLastConnections =>
        lastPlacedRoomConnections.Where(c => c.connectionState == ConnectionState.Open);
    private IEnumerable<ConnectionPoint> RandomlyClosedConnections =>
        allConnections.Where(c => c.connectionState == ConnectionState.ClosedRandomly);
    private IEnumerable<ConnectionPoint> RandomlyClosedLastConnections =>
        lastPlacedRoomConnections.Where(c => c.connectionState == ConnectionState.ClosedRandomly);
    
    // Flags
    public bool IsBossRoomGenerated { get; private set; }
    public bool IsAtLeastOneSpecialRoomGenerated { get; private set; }

    private int index = 0;

    void Awake()
    {
        occupiedTiles = new Dictionary<Vector2Int, int>();
        allConnections = new List<ConnectionPoint>();
        middleCoord = new Vector2Int(size / 2, size / 2);
        probabilityToCloseConnection = BASIC_PROBABILITY;
        lastPlacedRoomConnections = new();
        branchStartConnections = new();
        specialRoomPositions = new HashSet<Vector2Int>();
        index = 0;
    }

    [Button]
    public void GenerateLevel()
    {
        ClearLevel();
        occupiedTiles = new();
        probabilityToCloseConnection = BASIC_PROBABILITY;
        allConnections?.Clear();
        lastPlacedRoomConnections?.Clear();
        branchStartConnections?.Clear();
        specialRoomPositions?.Clear();
        IsBossRoomGenerated = false;
        IsAtLeastOneSpecialRoomGenerated = false;
        index = 0;

        int seed = CreateGenerationSeed();
        random = new System.Random(seed);

        int tries = 0;
        int placedRooms = 0;
        int mainBranch = (int)(roomCount * MAIN_BRANCH_LENGTH);
        int firstBranch = (int)(roomCount * (MAIN_BRANCH_LENGTH / 2));
        int secondBranch = roomCount - mainBranch - firstBranch;

        Debug.Log("###Start of Main Branch###");
        Room firstRoom = startRoom;
        PlaceRoomAt(middleCoord, firstRoom, out var firstInstance);
        placedRooms++;

        for (int i = 0; i < mainBranch; i++)
        {
            if (allConnections.Count == 0)
            {
                Debug.LogWarning("No More Available Connections");
                return;
            }

            Vector2Int? placePos;
            Room room;
            bool isBossRoom = false;
            
            if (i == mainBranch - 1 && bossRoom.Count > 0)
            {
                room = bossRoom[random.Next(0, bossRoom.Count)];
                IsBossRoomGenerated = true;
                isBossRoom = true;
                Debug.Log("Boss placed");
            }
            else
            {
                room = rooms[random.Next(0, rooms.Count)];                
            }
            
            var connectionPoints = OpenLastConnections.Any()
                ? OpenLastConnections
                : RandomlyClosedLastConnections;
                
            placePos = TryPlaceRoom(room, connectionPoints);

            if (i > 2 && i < mainBranch - 2 && !isBossRoom) StorePossibleBranchStart(); 
            
            TryToCloseConnection();
            
            if (placePos.HasValue)
            {
                PlaceRoomAt(placePos.Value, room, out var instance);
                if (isBossRoom)
                {
                    specialRoomPositions.Add(placePos.Value);
                    foreach (var connection in lastPlacedRoomConnections.ToList())
                    {
                        connection.connectionState = ConnectionState.Used;
                    }
        
                    lastPlacedRoomConnections.Clear();
                }

                instance.Index = index;
                index++;
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
        Debug.Log("###MainBranchComplete###");
        
        Debug.Log("###Start 1 Branch###");
        SpawnBranch(firstBranch, ref placedRooms, ref tries);
        
        Debug.Log("###Start 2 Branch###");
        SpawnBranch(secondBranch, ref placedRooms, ref tries);
        
        Debug.Log($"Generation complete tries : {tries}, rooms : {placedRooms}");
    }

    private void SpawnBranch(int branchLength, ref int placedRooms, ref int tries)
    {
        int initialTries = tries;
        if (StartNewBranch())
        {
            for (int i = 0; i < branchLength; i++)
            {
                Room room;
                Vector2Int? placePos;
                bool isSpecialRoom = false;
                if (i == branchLength - 1 && specialRooms.Count > 0)
                {
                    room = specialRooms[random.Next(0, specialRooms.Count)];
                    isSpecialRoom = true;
                    IsAtLeastOneSpecialRoomGenerated = true;
                }
                else
                {
                    room = rooms[random.Next(0, rooms.Count)];
                }
                
                var connectionPoints = OpenConnections.Any()
                    ? OpenLastConnections
                    : RandomlyClosedConnections;
                
                placePos = TryPlaceRoom(room, connectionPoints);
                TryToCloseConnection();
            
                if (placePos.HasValue)
                {
                    PlaceRoomAt(placePos.Value, room, out var instance);
                    if (isSpecialRoom)
                    {
                        specialRoomPositions.Add(placePos.Value);
                        foreach (var connection in lastPlacedRoomConnections.ToList())
                        {
                            connection.connectionState = ConnectionState.Used;
                        }
        
                        lastPlacedRoomConnections.Clear();
                    }
                    placedRooms++;
                }
                else
                {
                    tries++;
                    i--;
                }

                if (tries > initialTries + 100)
                {
                    break;
                }
            }
        }
    }
    
    private void StorePossibleBranchStart()
    {
        foreach (var connection in lastPlacedRoomConnections.Where(c => c.connectionState == ConnectionState.Open))
        {
            foreach (var specialPos in specialRoomPositions)
            {
                // Check if the connection is close to any special room position
                // (connections will be within the room's bounds)
                if (Vector2Int.Distance(connection.localPosition, specialPos) < 15) // Adjust distance as needed
                {
                    connection.connectionState = ConnectionState.Used;
                    break;
                }
            }
            
            if (!branchStartConnections.Contains(connection))
            {
                branchStartConnections.Add(connection);
            }
        }
    }

    private bool StartNewBranch()
    {
        var availableStarts = branchStartConnections
            .Where(c => c.connectionState == ConnectionState.Open)
            .ToList();
        
        if (availableStarts.Count == 0)
        {
            availableStarts = branchStartConnections
                .Where(c => c.connectionState == ConnectionState.ClosedRandomly)
                .ToList();
            if (availableStarts.Count == 0) return false;
        }

        var startConnection = availableStarts[random.Next(0, availableStarts.Count)];
        if (startConnection.connectionState == ConnectionState.ClosedRandomly)
        {
            startConnection.connectionState = ConnectionState.Open;
        }
        lastPlacedRoomConnections.Clear();
        lastPlacedRoomConnections.Add(startConnection);
        Debug.Log($"Starting new branch from connection at {startConnection.localPosition}");
        return true;
    }

    private Vector2Int? TryPlaceRoom(Room room, IEnumerable<ConnectionPoint> connections)
    {
        List<int> indices = new List<int>();
        var connectionPoints = connections.ToList();
        for (int i = 0; i < connectionPoints.Count; i++)
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
            var globalPoint = connectionPoints[index];
            if (globalPoint.connectionState == ConnectionState.Used)
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
                        globalPoint.connectionState = ConnectionState.Used;
                        //EliminateConnection(globalPoint);
                        return potentialPos;
                    }
                }
            }
        }

        return null;
    }

    private void TryToCloseConnection()
    {
        probabilityToCloseConnection = BASIC_PROBABILITY;
        if (allConnections.Count < 5) return;
        foreach (var connection in OpenConnections.ToList())
        {
            if (random.NextDouble() < probabilityToCloseConnection)
            {
                connection.connectionState = ConnectionState.ClosedRandomly;
                probabilityToCloseConnection = Mathf.Max(0f, probabilityToCloseConnection - 0.3f);
            }
        }
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

    private void PlaceRoomAt(Vector2Int position, Room room, out Room instance)
    {
        PlaceRoom(position, room);
        Vector3 worldPos = grid.CellToWorld(new Vector3Int(position.x, position.y, 0));
        instance = Instantiate(room, worldPos, Quaternion.identity, this.transform);

        lastPlacedRoomConnections.Clear();
        foreach (ConnectionPoint connectionPoint in room.ConnectionPoints)
        {
            var worldPosConnectionPoint = new ConnectionPoint
            {
                localPosition = position + connectionPoint.localPosition,
                direction = connectionPoint.direction,
                connectionState = ConnectionState.Open
            };
            allConnections.Add(worldPosConnectionPoint);
            lastPlacedRoomConnections.Add(worldPosConnectionPoint);
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
        allConnections?.Clear();
        branchStartConnections?.Clear();
        specialRoomPositions?.Clear();
        probabilityToCloseConnection = BASIC_PROBABILITY;
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