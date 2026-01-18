using System.Collections.Generic;
using System.Xml;

namespace ProjectFiles.Code.LevelGeneration
{
    public static class TmxParser
    {
        public class TiledMap
        {
            public int width;
            public int height;
            public int[,] occupied;
        }

        public static TiledMap ParseLevelTMX(string content)
        {
            TiledMap map = new TiledMap();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            
            XmlNode mapNode = xmlDoc.SelectSingleNode("map");
            map.width = int.Parse(mapNode.Attributes["width"].Value);
            map.height = int.Parse(mapNode.Attributes["height"].Value);
            map.occupied = new int[map.width, map.height];
            
            XmlNodeList layerNodes = mapNode.SelectNodes("//layer");
            foreach (XmlNode layerNode in layerNodes)
            {
                XmlNode dataNode = layerNode.SelectSingleNode("data");
                string csvData = dataNode.InnerText.Trim();
                string[] rows = csvData.Split('\n');

                for (int y = 0; y < rows.Length; y++)
                {
                    string[] values = rows[y].Split(',');
                    for (int x = 0; x < values.Length && x < map.width; x++)
                    {
                        if (int.TryParse(values[x].Trim(), out int tileId))
                        {
                            if (tileId != 0)
                            {
                                map.occupied[x, map.height - 1 - y] = 1;
                            }
                        }
                    }
                }
            }

            return map;
        }
    }
}