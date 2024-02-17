using RH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;



namespace Assets.Game.Scripts.MyWFC
{
    public class WFC2 : MonoBehaviour
    {
        public int Width = 5;
        public int Height = 5;
        public ConfigTile ConfigTile;


        public Tilemap tilemap;
        public Tile[] tiles;
        public Tile errorTile;


        public int startSeed = -1;
        public int seed = -1;
        public bool autorun = true;

        private RH.TileDataModel _tileDataModel;
        public int lastSeed = -1;

        public WFC wfc;

        public void Start()
        {
            _tileDataModel = ConfigTile.Convert();
            wfc = new WFC(Width, Height, _tileDataModel);

        }


        public void DrawAll(bool all)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!wfc.Tiles[x, y].Collapsed)
                    {
                        if (all)
                        {
                            Debug.LogError($"{x}, {y} PossibleTiles.Count {wfc.Tiles[x, y].PossibleTiles.Count}");
                            tilemap.SetTile(new Vector3Int(x, y, 0), errorTile);
                        }
                        foundError = true;
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tiles[wfc.Tiles[x, y].PossibleTiles[0].Id]);
                    }
                }
            }
        }

        // 15560

        bool initialized = false;
        public bool foundError = false;
        public bool done = false;

        public bool generationFinished = true;
        public Coroutine coroutineGeneration;

        public bool DoStep = true;

        IEnumerator Generate(int seed)
        {
            wfc.Init(seed);
            done = false;
            while (!done)
            {
                done = wfc.Iterate();
            }
            done = true;
            DrawAll(done);
            yield return null;
        }
        public int i = 0;
        public void GenerateAction()
        {
            UnityEngine.Random.InitState(i);
            i++;
            coroutineGeneration = StartCoroutine(Generate(i));
        }

        bool wasInitialized = false;

        public void UpdateNonRandom()
        {
            done = false;
            while (!done)
            {
                done = wfc.Iterate();
            }
            done = true;
            DrawAll(done);
        }


        public int IteractionCount = 0;


        public void Update()
        {
            if (!wasInitialized)
            {
                UnityEngine.Random.InitState(15560);
                i++;
                wfc.Init(seed);
                wasInitialized = true;
            }
            else 
            {
                if (!done)
                {
                    if (IteractionCount == 12)
                    {
                        Debug.Log("Debug");
                    }

                    done = wfc.Iterate();
                    IteractionCount++;
                    DrawAll(done);
                }
            }

            //if (startSeed != -1 && !wasInitialized)
            //{
            //    wasInitialized = true;
            //    UnityEngine.Random.seed = startSeed;
            //    wfc.Init(seed);
            //}
            //else if (startSeed != -1 && wasInitialized)
            //{
            //    if (Input.GetKeyDown(KeyCode.Space))
            //    {
            //        done = wfc.Iterate();
            //        DrawAll();
            //    }
            //}
            //if (startSeed != -1 && coroutineGeneration == null)
            //{
            //    UnityEngine.Random.seed = startSeed;
            //    coroutineGeneration = StartCoroutine(Generate(UnityEngine.Random.seed));
            //}

            // 15560
            //if (done && !foundError)
            //{
            //    Debug.Log($"Using seed {i}");
            //    UnityEngine.Random.InitState(15560);
            //    i++;
            //    coroutineGeneration = StartCoroutine(Generate(UnityEngine.Random.seed));
            //}
            //else if (foundError)
            //{
            //    lastSeed = UnityEngine.Random.seed;
            //}


                /*if (startSeed != -1 && coroutineGeneration == null)
                {
                    UnityEngine.Random.seed = startSeed;
                    //coroutineGeneration = StartCoroutine(Generate(UnityEngine.Random.seed));
                    wfc.Init(seed);
                    done = false;
                    while (!done)
                    {
                        done = wfc.Iterate();
                    }
                    done = true;
                    DrawAll();
                }
                else if (done && !foundError)
                {
                    Debug.Log($"Using seed {i}");
                    UnityEngine.Random.seed = i;
                    i++;
                    coroutineGeneration = StartCoroutine(Generate(UnityEngine.Random.seed));
                }
                else if (foundError)
                {
                    lastSeed = UnityEngine.Random.seed;
                }*/

            }
        }

}