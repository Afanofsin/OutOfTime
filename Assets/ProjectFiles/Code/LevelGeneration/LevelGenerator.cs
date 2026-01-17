using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using NaughtyAttributes;
using NUnit.Framework.Interfaces;
using ProjectFiles.Code.LevelGeneration;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Room settings")]
    [SerializeField] private List<Room> rooms;
    [SerializeField] private int roomCount;

    [Header("Grid")]
    [SerializeField] private Grid grid;
    [SerializeField] private int size = 300;

    private Dictionary<Vector2Int, int> occupiedTiles;
    private List<ConnectionPoint> availableConnections;
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

        Room firstRoom = rooms[0];
        PlaceRoomAt(middleCoord, firstRoom);
        placedRooms++;

        for (int i = 0; i < roomCount; i++)
        {
            if (availableConnections.Count == 0)
            {
                Debug.LogWarning("No More Available Connections");
                return;
            }
            
            
            Room room = rooms[random.Next(0, rooms.Count)];
            Vector2Int? placePos = TryPlaceRoom(room);

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

    private Vector2Int? TryPlaceRoom(Room room)
    {
        List<int> indices = new List<int>();
        for (int i = 0; i < availableConnections.Count; i++)
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
            var globalPoint = availableConnections[index];
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
                Debug.Log($"    Checking room connection: {roomPoint.direction}");
                if (AreConnectionsCompatible(globalPoint.direction, roomPoint.direction))
                {
                    Debug.Log($"      Compatible! Trying to place...");
                    Vector2Int offset = GetConnectionOffset(globalPoint.direction);
                    //Vector2Int potentialPos = globalPoint.localPosition + offset - roomPoint.localPosition;
                    Vector2Int potentialPos = globalPoint.localPosition - roomPoint.localPosition;
                    
                    if (CheckIfRoomFitsInGrid(potentialPos, room))
                    {
                        Debug.Log($"      SUCCESS! Placed at {potentialPos}");
                        // Mark this connection as used
                        globalPoint.isConnected = true;
                        return potentialPos;
                    }
                    else
                    {
                        Debug.Log($"      Failed fit check at {potentialPos}");
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
    
    private Vector2Int GetConnectionOffset(Direction direction)
    {
        return direction switch
        {
            Direction.North => new Vector2Int(0, 1),
            Direction.South => new Vector2Int(0, -1),
            Direction.East => new Vector2Int(1, 0),
            Direction.West => new Vector2Int(-1, 0),
            _ => Vector2Int.zero
        };
    }
    
    private bool CheckIfRoomFitsInGrid(Vector2Int pos, Room room)
    {
        int h = room.GetHeight;
        int w = room.GetWidth;
        int x = pos.x + w;
        int y = pos.y + h;

        if (x < 0 || y < 0 || x > size || y > size)
            return false;
        
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

        foreach (ConnectionPoint connectionPoint in room.ConnectionPoints)
        {
            var worldPosConnectionPoint = new ConnectionPoint
            {
                localPosition = position + connectionPoint.localPosition,
                direction = connectionPoint.direction,
                isConnected = false
            };
            availableConnections.Add(worldPosConnectionPoint);
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
    
}
