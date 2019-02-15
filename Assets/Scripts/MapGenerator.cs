﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int size;

    public float height;
    public int gridSize;
    public int maxRun;

    public Texture2D buildable;
    public Texture2D path;

    public void Start() { Generate(); }
    public Terrain current;

    public GameObject[,] tiles;

    void Generate() {
        TerrainData data = new TerrainData();
        //do to Terrian Magic =( the heightmapResolution is always 2**n + 1 for some n
        //so we get a resolution as close to our input size as possible but the world unit size is hte same.
        data.heightmapResolution = size;
       
        data.size = new Vector3(size, height, size);
        size = data.heightmapResolution;
        print(data.alphamapResolution = size);
        tiles = new GameObject[gridSize, gridSize];
        int x = gridSize / 2;//Random.Range(1, gridSize - 2);
        int y = -1;
        int steps = gridSize;
        Vector2Int[] dirs = new Vector2Int[] { Vector2Int.right, Vector2Int.left };
        Vector2Int dir = Vector2Int.down;
        while (y < gridSize - 1) {
            steps = Random.Range(2, maxRun);
            while (steps > 0) {
                if (x + dir.x < 1 || x + dir.x > gridSize - 2 || y - dir.y > gridSize - 1) {
                    //x = Mathf.Clamp(x, 1, gridSize - 2);
                    break;
                }
                x += dir.x;
                y -= dir.y;
                tiles[x, y] = gameObject;
                steps--;
            }
            if (dir == Vector2Int.down) {
                dir = dirs[Random.Range(0,dirs.Length)];
            }
            else {
                dir = Vector2Int.down;
            }
        }
       

        float[,] heights = new float[size, size];
        float scale = size / (float)gridSize;
        for (x = 0; x < gridSize; x++)
        {
            for (y = 0; y < gridSize; y++)
            {
                if (tiles[x, y] != gameObject) continue;
                int sx = Mathf.RoundToInt(x * scale);
                int sy = Mathf.RoundToInt(y * scale);
                int ex = Mathf.RoundToInt((x + 1) * scale);
                int ey = Mathf.RoundToInt((y + 1) * scale);
                for (int cx = sx; cx < ex; cx++) {
                    for (int cy = sy; cy < ey; cy++) {
                        heights[cx, cy] = 1;
                    }
                }
            }
        }
        float[,,] textures = new float[size, size, 2];
        for (x = 0; x < size; x++) {
            for (y = 0; y < size; y++) {
                if (heights[x, y] == 0)
                {
                    heights[x, y] = 1;
                    textures[x, y, 0] = 0;
                    textures[x, y, 1] = 1;
                }
                else {
                    heights[x, y] = 0;
                    textures[x, y, 0] = 1;
                    textures[x, y, 1] = 0;
                }
            }
        }
        TerrainLayer pathLayer = new TerrainLayer();
        pathLayer.diffuseTexture = path;
        TerrainLayer buildableLayer = new TerrainLayer();
        buildableLayer.diffuseTexture = buildable;
        data.terrainLayers = new TerrainLayer[] { pathLayer, buildableLayer};
        data.SetHeights(0, 0, heights);
        data.SetAlphamaps(0, 0, textures);
        GameObject go = Terrain.CreateTerrainGameObject(data);
        current = go.GetComponent<Terrain>();
        go.transform.SetParent(transform, false);
        go.transform.localPosition = new Vector3(data.size.x / -2f, 0, data.size.z / -2f);
    }
}
