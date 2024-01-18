using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

namespace TilemapWorldGenerator
{
    public class TilemapRuleExtractor : MonoBehaviour
    {
        // The tilemap to analyze and extract rules
        [SerializeField] private Tilemap Tilemap;
        // Extracted width
        [SerializeField] private int Width;
        // Extracted height
        [SerializeField] private int Height;

        private class TilemapDef
        {
            public Tilemap Tilemap;
            public int Width;
            public int Height;
            public TileBase[] Tiles;
        }

        private TilemapDef _tilemapDef;
        private TileProperties[] TilesProperties;

        public void ExtractRules()
        {
            Debug.Log("[TilemapRuleExtractor] Extracting rules from tilemap " + Tilemap.name);
            SetupTilemapDefObject(Tilemap);
            TilesProperties = RetrieveTilesTypes(_tilemapDef.Tiles);
            GetRules(_tilemapDef, TilesProperties);
            CreateScriptableObjects(TilesProperties);
            Debug.Log("[TilemapRuleExtractor] Rules extracted from tilemap " + Tilemap.name);
        }

        private void CheckTilemap()
        {
            // Check bounds and if no blank cell
        }

        private void SetupTilemapDefObject(Tilemap tilemap)
        {
            _tilemapDef = new TilemapDef();
            _tilemapDef.Tilemap = tilemap;
            _tilemapDef.Tilemap.CompressBounds();
            _tilemapDef.Width = tilemap.cellBounds.size.x;
            _tilemapDef.Height = tilemap.cellBounds.size.y;
            _tilemapDef.Tiles = _tilemapDef.Tilemap.GetTilesBlock(_tilemapDef.Tilemap.cellBounds);
        }

        private TileProperties[] RetrieveTilesTypes(TileBase[] tiles)
        {
            List<TileProperties> tilesTypes = new List<TileProperties>();
            for (int i = 0; i < tiles.Length; i++)
            {
                TileBase tile = tiles[i];
                TileProperties t = tilesTypes.FirstOrDefault(tileType => tileType.TileBase == tile);
                if (t == null)
                    tilesTypes.Add(new TileProperties(tile, tile.name, 1));
                else
                    t.IncreaseWeight(1);
            }

            return tilesTypes.ToArray();
        }

        private void GetRules(TilemapDef tilemapDef, TileProperties[] TileProperties)
        {
            // For each tiles OF THE TILEMAP
            for (int i = 0; i < tilemapDef.Tiles.Length; i++)
            {
                TileBase tileBase = tilemapDef.Tiles[i];
                TileProperties concernedTile = GetTileDefFromTileBase(TileProperties, tileBase);
                TileBase[] neighbours = GetNeighbours(tilemapDef, i);

                TileBase neighbourLeft = neighbours[0];
                if (neighbourLeft && !concernedTile.Left.Any(o => o.TileBase == neighbourLeft))
                {
                    concernedTile.Left.Add(GetTileDefFromTileBase(TileProperties, neighbourLeft));
                }

                TileBase neighbourTop = neighbours[1];
                if (neighbourTop && !concernedTile.Top.Any(o => o.TileBase == neighbourTop))
                {
                    concernedTile.Top.Add(GetTileDefFromTileBase(TileProperties, neighbourTop));
                }

                TileBase neighbourRight = neighbours[2];
                if (neighbourRight && !concernedTile.Right.Any(o => o.TileBase == neighbourRight))
                {
                    concernedTile.Right.Add(GetTileDefFromTileBase(TileProperties, neighbourRight));
                }

                TileBase neighbourBottom = neighbours[3];
                if (neighbourBottom && !concernedTile.Bottom.Any(o => o.TileBase == neighbourBottom))
                {
                    concernedTile.Bottom.Add(GetTileDefFromTileBase(TileProperties, neighbourBottom));
                }
            }
        }

        private TileBase GetTile(TilemapDef tilemapDef, int x, int y)
        {
            return (x >= 0 && x < Width && y >= 0 && y < Height) ? tilemapDef.Tiles[x + y * tilemapDef.Width] : null;
        }

        private TileBase[] GetNeighbours(TilemapDef tilemapDef, int i)
        {
            return new TileBase[]
            {
                GetTile(tilemapDef, (i % tilemapDef.Width) - 1, i / tilemapDef.Width),
                GetTile(tilemapDef, (i % tilemapDef.Width), (i  / tilemapDef.Width) + 1),
                GetTile(tilemapDef, (i % tilemapDef.Width) + 1, i / tilemapDef.Width),
                GetTile(tilemapDef, (i % tilemapDef.Width), (i  / tilemapDef.Width) - 1)
            };
        }

        TileProperties GetTileDefFromTileBase(TileProperties[] TilesDefinition, TileBase t)
        {
            for (int i = 0; i < TilesDefinition.Length; i++)
            {
                if (TilesDefinition[i].TileBase == t)
                {
                    return TilesDefinition[i];
                }
            }
            return null;
        }


        private Node GetNodeFromTileDefinition(Node[] nodes, TileProperties tileDefinition)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].Tile == tileDefinition.TileBase as UnityEngine.Tilemaps.Tile)
                {
                    return nodes[i];
                }
            }
            Debug.Log($"Couldn't find node correspondance for {tileDefinition.Name}");
            return null;
        }


        private void CreateScriptableObjects(TileProperties[] TileProperties)
        {
            Node[] nodesSO = new Node[TileProperties.Length];

            for (int i = 0; i < TileProperties.Length; i++)
            {
                TileProperties TileProperty = TileProperties[i];
                Node nodeSO = ScriptableObject.CreateInstance<Node>();
                nodeSO.Tile = TileProperty.TileBase as UnityEngine.Tilemaps.Tile;
                nodeSO.Name = TileProperty.Name;
                nodeSO.Weight = TileProperty.Weight;
                nodesSO[i] = nodeSO;
            }

            for (int i = 0; i < TileProperties.Length; i++)
            {
                TileProperties TileProperty = TileProperties[i];
                for (int j = 0; j < TileProperty.Left.Count; j++)
                {
                    Node nLeft = GetNodeFromTileDefinition(nodesSO, TileProperty.Left[j]);
                    nodesSO[i].Left.CompatibleNodes.Add(nLeft);
                }

                for (int j = 0; j < TileProperty.Top.Count; j++)
                {
                    Node nTop = GetNodeFromTileDefinition(nodesSO, TileProperty.Top[j]);
                    nodesSO[i].Top.CompatibleNodes.Add(nTop);
                }

                for (int j = 0; j < TileProperty.Right.Count; j++)
                {
                    Node nRight = GetNodeFromTileDefinition(nodesSO, TileProperty.Right[j]);
                    nodesSO[i].Right.CompatibleNodes.Add(nRight);
                }

                for (int j = 0; j < TileProperty.Bottom.Count; j++)
                {
                    Node nBottom = GetNodeFromTileDefinition(nodesSO, TileProperty.Bottom[j]);
                    nodesSO[i].Bottom.CompatibleNodes.Add(nBottom);
                }

                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Game/ScriptableObjects/Generated/ " + typeof(Node).Name + "_" + TileProperty.Name + ".asset");

                AssetDatabase.CreateAsset(nodesSO[i], assetPathAndName);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
            }
        }
    }
}