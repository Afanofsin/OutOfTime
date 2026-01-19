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
    private HashSet<ConnectionPoint> specialRoomConnections = new HashSet<ConnectionPoint>();
    private HashSet<ConnectionPoint> firstBranchConnections = new HashSet<ConnectionPoint>();
    private List<(Vector2Int position, Direction direction)> pendingUsedConnections = new List<(Vector2Int, Direction)>();
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
    public int SpecialRoomsGenerated { get; private set; }

    private int index = 1;
    public static LevelGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        
        occupiedTiles = new Dictionary<Vector2Int, int>();
        allConnections = new List<ConnectionPoint>();
        middleCoord = new Vector2Int(size / 2, size / 2);
        probabilityToCloseConnection = BASIC_PROBABILITY;
        lastPlacedRoomConnections = new();
        branchStartConnections = new();
        specialRoomConnections = new HashSet<ConnectionPoint>();
        firstBranchConnections = new HashSet<ConnectionPoint>();
        pendingUsedConnections = new List<(Vector2Int, Direction)>();
        index = 1;
    }

    [Button]
    public void GenerateLevel()
    {
        ClearLevel();
        occupiedTiles = new();
        probabilityToCloseConnection = BASIC_PROBABILITY;
        IsBossRoomGenerated = false;
        SpecialRoomsGenerated = 0;
        middleCoord = new Vector2Int(size / 2, size / 2);
        index = 1;

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
            
            if (placePos.HasValue)
            {
                PlaceRoomAt(placePos.Value, room, out var instance);
                if (!isBossRoom) StorePossibleBranchStart(); 
                if (isBossRoom)
                {
                    foreach (var connection in lastPlacedRoomConnections.ToList())
                    {
                        connection.connectionState = ConnectionState.Used;
                        specialRoomConnections.Add(connection);
                    }
        
                    lastPlacedRoomConnections.Clear();
                }
                //TryToCloseConnection();

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
        SpawnBranch(firstBranch, ref placedRooms, ref tries, true);
        
        branchStartConnections?.RemoveAll(c => firstBranchConnections.Contains(c));
        firstBranchConnections?.Clear();
        
        Debug.Log("###Start 2 Branch###");
        SpawnBranch(secondBranch, ref placedRooms, ref tries, false);
        
        Debug.Log($"Generation complete tries : {tries}, rooms : {placedRooms}");
        foreach (var connection in allConnections)
        {
            connection.OnStateFinalized?.Invoke();
        }
    }

    private void SpawnBranch(int branchLength, ref int placedRooms, ref int tries, bool isFirstBranch)
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
                }
                else
                {
                    room = rooms[random.Next(0, rooms.Count)];
                }
                
                var connectionPoints = OpenLastConnections.Any()
                    ? OpenLastConnections
                    : RandomlyClosedLastConnections;
                
                placePos = TryPlaceRoom(room, connectionPoints);
                TryToCloseConnection();
            
                if (placePos.HasValue)
                {
                    PlaceRoomAt(placePos.Value, room, out var instance);
                    
                    if (isFirstBranch && !isSpecialRoom)
                    {
                        foreach (var connection in lastPlacedRoomConnections.ToList())
                        {
                            firstBranchConnections.Add(connection);
                        }
                    }
                    
                    if (isSpecialRoom)
                    {
                        foreach (var connection in lastPlacedRoomConnections.ToList())
                        {
                            connection.connectionState = ConnectionState.Used;
                            specialRoomConnections.Add(connection);
                        }
                        SpecialRoomsGenerated++;
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
        foreach (var connection in lastPlacedRoomConnections)
        {
            if (connection.connectionState != ConnectionState.Open)
                continue;

            branchStartConnections.Add(connection);
        }
    }
    
    // private bool StartNewBranch()
    // {
    //     for (int attempt = 0; attempt < 5; attempt++)
    //     {
    //         var available = branchStartConnections
    //             .Where(c => c.connectionState == ConnectionState.Open)
    //             .ToList();
    //
    //         if (available.Count == 0)
    //         {
    //             available = branchStartConnections
    //                 .Where(c => c.connectionState == ConnectionState.ClosedRandomly)
    //                 .ToList();
    //
    //             if (available.Count == 0)
    //                 return false;
    //         }
    //
    //         var start = available[random.Next(available.Count)];
    //         if (startConnection.connectionState == ConnectionState.ClosedRandomly)
    //         {
    //             startConnection.connectionState = ConnectionState.Open;
    //         }
    //         lastPlacedRoomConnections.Clear();
    //         lastPlacedRoomConnections.Add(start);
    //         return true;
    //     }
    //     return false;
    // }
    
    private bool StartNewBranch()
    {
        // 1. Get all candidates (Open or ClosedRandomly)
        var candidates = branchStartConnections
            .Where(c => c.connectionState == ConnectionState.Open || c.connectionState == ConnectionState.ClosedRandomly)
            .ToList();

        if (candidates.Count == 0) return false;

        // 2. Shuffle candidates (Fisher-Yates) so we don't always pick the first one
        for (int i = 0; i < candidates.Count; i++)
        {
            var temp = candidates[i];
            int randomIndex = random.Next(i, candidates.Count);
            candidates[i] = candidates[randomIndex];
            candidates[randomIndex] = temp;
        }

        // 3. Find the first candidate where a room actually fits
        foreach (var startNode in candidates)
        {
            // Try a few times to find a room that fits this specific connection
            // We test with a "Hypothetical" check without actually placing it yet
            for (int k = 0; k < 5; k++) 
            {
                Room testRoom = rooms[random.Next(rooms.Count)];
            
                // Check all connection points on the room to see if one aligns
                foreach (var roomPoint in testRoom.ConnectionPoints)
                {
                    if (AreConnectionsCompatible(startNode.direction, roomPoint.direction))
                    {
                        Vector2Int potentialPos = startNode.localPosition - roomPoint.localPosition;
                    
                        if (CheckIfRoomFitsInGrid(potentialPos, testRoom))
                        {
                            // FOUND A VALID SPOT!
                            // Set this as our starting point
                            if (startNode.connectionState == ConnectionState.ClosedRandomly)
                            {
                                startNode.connectionState = ConnectionState.Open;
                            }
                        
                            lastPlacedRoomConnections.Clear();
                            lastPlacedRoomConnections.Add(startNode);
                            Debug.Log($"Starting new branch from {startNode.localPosition}");
                            return true;
                        }
                    }
                }
            }
        }
        // If we looped through ALL candidates and NO room fit anywhere:
        return false;
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
                        // Eliminate connection
                        globalPoint.connectionState = ConnectionState.Used;
                        // Store connection of this room to be Consumed later
                        pendingUsedConnections.Add((potentialPos + roomPoint.localPosition, roomPoint.direction));
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
            if (branchStartConnections.Contains(connection))
                continue;
            
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
        
        lastPlacedRoomConnections?.Clear();
        
        for (int i = 0; i < room.ConnectionPoints.Count; i++)
        {
            ConnectionPoint prefabConnection = room.ConnectionPoints[i];
            ConnectionPoint instanceConnection = instance.ConnectionPoints[i];
        
            var worldPosConnectionPoint = new ConnectionPoint
            {
                localPosition = position + prefabConnection.localPosition,
                direction = prefabConnection.direction,
                connectionState = ConnectionState.Open
            };
            
            bool wasUsed = pendingUsedConnections.Any(p => 
                p.position == worldPosConnectionPoint.localPosition && 
                p.direction == worldPosConnectionPoint.direction);
        
            if (wasUsed)
            {
                worldPosConnectionPoint.connectionState = ConnectionState.Used;
                pendingUsedConnections.RemoveAll(p => 
                    p.position == worldPosConnectionPoint.localPosition && 
                    p.direction == worldPosConnectionPoint.direction);
            }

            instanceConnection.WorldConnection = worldPosConnectionPoint;
            worldPosConnectionPoint.OnStateFinalized = instanceConnection.FinalizeCoverState;
            
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

    #region Debugging 
    
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
        specialRoomConnections?.Clear();
        firstBranchConnections?.Clear();
        pendingUsedConnections?.Clear();
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
    
    #endregion
}