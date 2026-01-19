using System;
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
        public int Index = 0;
        
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
            if (occupiedTiles == null || occupiedTiles.Count == 0)
            {
                Debug.LogError("No occupancy data. Parse TMX first!");
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
    
            // Add top border with column numbers
            sb.Append("    "); // Padding for row numbers
            for (int x = 0; x < width; x++)
            {
                sb.Append($"{x % 10} ");
            }
            sb.AppendLine();
    
            sb.Append("   ┌");
            for (int x = 0; x < width; x++)
            {
                sb.Append("──");
            }
            sb.AppendLine("┐");
    
            // Print rows from top to bottom with row numbers
            for (int y = height - 1; y >= 0; y--)
            {
                sb.Append($"{y,2} │"); // Row number with padding
        
                for (int x = 0; x < width; x++)
                {
                    if (occupiedTiles[(y * width) + x] == 1)
                    {
                        sb.Append("██"); // Filled block
                    }
                    else
                    {
                        sb.Append("  "); // Empty space
                    }
                }
                sb.AppendLine($"│ {y}");
            }
    
            // Bottom border
            sb.Append("   └");
            for (int x = 0; x < width; x++)
            {
                sb.Append("──");
            }
            sb.AppendLine("┘");
    
            // Bottom column numbers
            sb.Append("    ");
            for (int x = 0; x < width; x++)
            {
                sb.Append($"{x % 10} ");
            }
            sb.AppendLine();
    
            // Add connection points visualization
            if (ConnectionPoints != null && ConnectionPoints.Count > 0)
            {
                sb.AppendLine("\nConnection Points:");
                foreach (var point in ConnectionPoints)
                {
                    sb.AppendLine($"  {point.direction}: ({point.localPosition.x}, {point.localPosition.y})");
                }
            }
    
            Debug.Log($"Room: {width}x{height}\n{sb}");
        }
    
        [Button]
        void DebugConnectionPoints()
        {
            foreach (var point in ConnectionPoints)
            {
                Vector3 worldPos = transform.TransformPoint(new Vector3(point.localPosition.x, point.localPosition.y, 0));
                Debug.Log($"{point.direction}: Local {point.localPosition} -> World {worldPos}");
                Debug.DrawLine(transform.position, worldPos, Color.red, 5f);
            }
        }
        
        [Button]
        public void VisualizeOccupancy()
        {
            if (occupiedTiles == null || occupiedTiles.Count == 0)
            {
                Debug.LogError("No occupancy data!");
                return;
            }

            // Create a temporary GameObject to visualize
            GameObject vizParent = new GameObject($"{name}_Occupancy_Visualization");
            vizParent.transform.SetParent(transform);
            vizParent.transform.localPosition = Vector3.zero;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (occupiedTiles[(y * width) + x] == 1)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.SetParent(vizParent.transform);
                        cube.transform.localPosition = new Vector3(x, y, 0);
                        cube.transform.localScale = new Vector3(0.9f, 0.9f, 0.1f);
                
                        // Make it semi-transparent red
                        var renderer = cube.GetComponent<Renderer>();
                        var mat = new Material(Shader.Find("Sprites/Default"));
                        mat.color = new Color(1, 0, 0, 0.5f);
                        renderer.material = mat;
                
                        DestroyImmediate(cube.GetComponent<Collider>());
                    }
                }
            }
    
            Debug.Log($"Created occupancy visualization for {name}. Red cubes show where collision detection thinks tiles are.");
        }
    }
}

[System.Serializable]
public class ConnectionPoint
{
    public Vector2Int localPosition;
    public Direction direction;
    private ConnectionState _currentConnectionState = (ConnectionState)(-1);
    public GameObject Cover;
    public ConnectionPoint WorldConnection;
    public Action OnStateFinalized;
    
    public ConnectionState connectionState
    {
        get { return _currentConnectionState; }
        set
        {
            _currentConnectionState = value;
        }
    }

    public void FinalizeCoverState()
    {
        if (Cover == null) return;
        ConnectionState stateToUse = WorldConnection?.connectionState ?? connectionState;
        switch (stateToUse)
        {
            case ConnectionState.Open:
            case ConnectionState.ClosedRandomly:
                Cover?.SetActive(true);
                break;
            case ConnectionState.Used:
                Cover?.SetActive(false);
                break;
            default:
                Cover?.SetActive(false);
                break;
        }
    }
}

public enum Direction
{
    North,
    South,
    East,
    West
}

public enum ConnectionState
{
    Open,
    ClosedRandomly,
    Used
}