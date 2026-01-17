using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace ProjectFiles.Code.LevelGeneration
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private TextAsset tmxLevel;
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] public List<int> occupiedTiles;
        
        [SerializeField] public List<ConnectionPoint> ConnectionPoints;
        public int GetHeight => height;
        public int GetWidth => width;
        
        public Vector2Int GetWorldConnectionPoint(Vector2Int roomPosition, ConnectionPoint point)
        {
            return roomPosition + point.localPosition;
        }
        
        [Button]
        public void ParseTmxDimensions()
        {
            if (tmxLevel == null)
            {
                Debug.LogError("TMX file is not assigned!");
                return;
            }
        
            if (string.IsNullOrEmpty(tmxLevel.text))
            {
                Debug.LogError("TMX file is empty!");
                return;
            }
        
            Debug.Log($"TMX Content length: {tmxLevel.text.Length}");
            
            TmxParser.TiledMap map = TmxParser.ParseLevelTMX(tmxLevel.text);
            
            if (map != null)
            {
                Debug.Log($"Map parsed successfully: {map.width}x{map.height}");
            }
            else
            {
                Debug.LogError("Failed to parse map");
            }
            
            width = map.width;
            height = map.height;
            occupiedTiles.Clear();
            occupiedTiles.Capacity = map.width * map.height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    occupiedTiles.Add(map.occupied[x, y]);
                }
            }
            
        }

        [Button]
        public void ShowRoom()
        {
            char[] blocks = new char[width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (occupiedTiles[(y * width) + x] == 1)
                    {
                        blocks[x] = '#';
                    }
                    else
                    {
                        blocks[x] = '.';
                    }
                }

                string level = string.Join(" ", blocks);
                Debug.Log(level);
            }
        }
    
    }
}

[System.Serializable]
public class ConnectionPoint
{
    public Vector2Int localPosition;
    public Direction direction;
    public bool isConnected;
}

public enum Direction
{
    North,
    South,
    East,
    West
}