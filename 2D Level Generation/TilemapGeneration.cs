using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class TilemapGeneration : MonoBehaviour
{
    [Header("Walls")]
    public Tilemap map;
    public RuleTile ruleTile;
    public Vector2Int gridSize = new Vector2Int(60, 40);
    public int numberOfSteps = 120;
    public int brushSize = 4;
    private Vector2Int[] directions = new Vector2Int[]{new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,-1)};

    [Header("Spikes")]
    public bool spawnSpikes;
    public Tilemap spikeMap;
    public Tile[] spikeTiles;
    public float spikePct = 0.15f;

    [Header("Enemies")]
    public bool spawnEnemies;
    public Transform enemyParent;
    public GameObject enemyPrefab;
    public int minEnemies;
    public int maxEnemies;
    private List<Vector3Int> emptySpaces = new List<Vector3Int>();

    [Header("Player")]
    public bool placePlayer;
    public Transform player;
    private bool playerSet;

    /*[Header("Pathfinding")]
    public PathGrid pathGrid;*/


    void Start()
    {
        Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Generate();
        }
    }

    public void Generate()
    {
        // Reset the scene
        Random.InitState(System.Environment.TickCount);
        map.ClearAllTiles();
        emptySpaces.Clear();

        if (spawnSpikes)
            spikeMap.ClearAllTiles();
                
        if (spawnEnemies)
        {
            foreach (Transform child in enemyParent)
                Destroy(child.gameObject);
        }
        
        if (placePlayer)
            playerSet = false;

        // Fill the map with tiles
        map.FloodFill(new Vector3Int(gridSize.x, gridSize.y, 0), ruleTile);
        Vector2Int currentPos = new Vector2Int(gridSize.x/2, gridSize.y/2);

        // Erase out paths
        for(int i = 0; i < numberOfSteps; i++)
        {
            // Pick a random direction, negate it if we would go out of bounds
            Vector2Int direction = directions[Random.Range(0, directions.Length)];
            if (currentPos.x + direction.x > gridSize.x-2 || currentPos.x + direction.x < 2 || currentPos.y + direction.y > gridSize.y-2 || currentPos.y + direction.y < 2)
            {
                direction *= -1;
            }

            // Erase an empty space in that direction
            for(int j = 1; j <= brushSize; j++)
            {
                for(int k = 1; k <= brushSize; k++) 
                {
                    Vector3Int loc = new Vector3Int(currentPos.x+direction.x+j, currentPos.y+direction.y+k, 0);
                    map.SetTile(loc, null);
                    emptySpaces.Add(loc);
                }
            }
            currentPos += direction * (brushSize/2 + 1);
        }

        // Destroy overhanging pieces
        for(int x = 0; x <= gridSize.x -3; x++)
        {
            for(int y = 0; y <= gridSize.y -3; y++)
            {
                if (NumNeighbors(new Vector3Int(x, y, 0), map) < 2 && map.HasTile(new Vector3Int(x, y, 0)))
                {
                    map.SetTile(new Vector3Int(x, y, 0), null);
                    x = 0;
                    y = 0;
                }
                if (placePlayer)
                {
                    // Move player to an open spot
                    if (!playerSet)
                    {
                        // Check a 5x5 area around current tile
                        bool validSpawn = true;
                        for (int dx = -2; dx <= 2; dx++)
                        {
                            for (int dy = -2; dy <= 2; dy++)
                            {
                                if (map.HasTile(new Vector3Int(x + dx, y + dy, 0)) || spikeMap.HasTile(new Vector3Int(x + dx, y + dy, 0)))
                                {
                                    validSpawn = false;
                                    break;
                                }
                            }
                        }
                        // If empty, place player here
                        if (validSpawn)
                        {
                            playerSet = true;
                            player.position = new Vector3(x, y, 0);
                            RemoveEmpty(new Vector3Int(x, y, 0));
                        }
                    }
                }
            }
        }

        // Create 15-tile border for padding
        for (int x = -15; x < gridSize.x+15; x++)
        {
            for (int y = -15; y < gridSize.y+15; y++)
            {
                if (x < 0 || x > gridSize.x || y < 0 || y > gridSize.y)
                    map.SetTile(new Vector3Int(x, y, 0), ruleTile);
            }
        }

        // Generate spikes
        if (spawnSpikes)
        {
            for(int x = 0; x <= gridSize.x; x++)
            {
                for(int y = 0; y <= gridSize.y; y++)
                {
                    AttemptSpikeSpawn(new Vector3Int(x, y, 0));
                }
            }
        }

        // Spawn enemies
        if (spawnEnemies)
        {
            int offset = 0;
            if (emptySpaces.Count > 600)
                offset = 2;
            else if (emptySpaces.Count > 400)
                offset = 1;
            int numEnemies = Random.Range(minEnemies, maxEnemies-1 + offset);

            int enemiesSpawned = 0;
            int timesAttempted = 0;
            while (enemiesSpawned < numEnemies && timesAttempted < 30)
            {
                Vector3Int randomTile = emptySpaces[Random.Range(0, emptySpaces.Count)];
                timesAttempted++;
                if (NumNeighbors(randomTile, map) == 0 && NumNeighbors(randomTile, spikeMap) == 0 && randomTile.x > 0 && randomTile.y > 0 && randomTile.x < gridSize.x && randomTile.y < gridSize.y)
                {
                    GameObject enemy = Instantiate(enemyPrefab, randomTile, Quaternion.identity, enemyParent);
                    enemy.GetComponent<EnemyMovement>().mode = "IDLE";
                    enemiesSpawned++;
                    RemoveEmpty(randomTile);
                }
            }
        }

        // Recalculate pathfinding
        //pathGrid.CreateGrid();
    }

    private void RemoveEmpty(Vector3Int pos)
    {
        HashSet<Vector3Int> emptySpacesHash = new HashSet<Vector3Int>(emptySpaces);
        for (int dx = -4; dx <= 4; dx++)
        {
            for (int dy = -4; dy <= 4; dy++)
            {
                emptySpacesHash.Remove(pos + new Vector3Int(dx, dy, 0));
            }
        }
        emptySpaces = new List<Vector3Int>(emptySpacesHash);
    }

    // Returns number of neighbors (including diagonals)
    private int NumNeighbors(Vector3Int currentPos, Tilemap tilemap)
    {
        int neighbors = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (map.HasTile(new Vector3Int(currentPos.x + x, currentPos.y + y, 0)) && !(x == 0 && y == 0))
                    neighbors++;
            }
        }
        return neighbors;
    }

    // Returns number of strictly adjacent neighbors
    private int AdjNeighbors(Vector3Int currentPos, Tilemap tilemap)
    {
        int neighbors = 0;
        foreach (Vector2Int dir in directions)
        {
            if (tilemap.HasTile(new Vector3Int(currentPos.x + dir.x, currentPos.y + dir.y, 0)))
                neighbors++;
        }
        return neighbors;
    }

    private void AttemptSpikeSpawn(Vector3Int v3)
    {
        // 3x2 bool groups, check if have space for 3 spikes, and then spawn all 3 at once        
        CheckArea(new bool[]{false, false, false, true, true, true}, new Vector3Int(0, 2, 0), 3, spikeTiles[0], v3);
        CheckArea(new bool[]{true, true, true, false, false, false}, new Vector3Int(0, -2, 0), 3, spikeTiles[1], v3);
        CheckArea(new bool[]{true, false, true, false, true, false}, new Vector3Int(2, 0, 0), 2, spikeTiles[2], v3);
        CheckArea(new bool[]{false, true, false, true, false, true}, new Vector3Int(-2, 0, 0), 2, spikeTiles[3], v3);
    }

    private void CheckArea(bool[] pattern, Vector3Int direction, int cols, Tile spikeTile, Vector3Int v3, int fillDir = 0)
    {
        // Check whether nearby tiles match the provided pattern
        bool match = true;
        for (int i = 0; i < 6; i++)
        {
            Vector3Int loc = new Vector3Int(v3.x + i%cols, v3.y + i/cols, 0);
            if (map.HasTile(loc) != pattern[i] || spikeMap.HasTile(loc + direction))
            {
                match = false;
                break;
            }
        }

        //If so, place spikes and recursively check adjacent areas
        if (match)
        {
            if (Random.Range(0f, 1f) < spikePct || fillDir != 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!pattern[i])
                    {
                        Vector3Int loc = new Vector3Int(v3.x + i%cols, v3.y + i/cols);
                        spikeMap.SetTile(loc, spikeTile);
                    }
                }
                Vector3Int offset = (cols == 3) ? new Vector3Int(1, 0, 0) : new Vector3Int(0, 1, 0);
                if (fillDir == 0 || fillDir == 1)
                    CheckArea(pattern, direction, cols, spikeTile, v3 + offset, 1);
                if (fillDir == 0 || fillDir == -1)
                    CheckArea(pattern, direction, cols, spikeTile, v3 + offset*-1, 1);
            }
        }
    }
}