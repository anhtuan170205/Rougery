using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public class CellData
    {
        public bool Passable;
        public CellObject containedObject;
    }
    CellData[,] boardData;
    private Tilemap tileMap;
    private Grid grid;
    public int width;
    public int height;
    public int maxFoodTileCount = 10;
    public int minFoodTileCount = 5;
    public int maxWallTileCount = 20;
    public int minWallTileCount = 15;
    public int maxEnemyTileCount = 5;
    public int minEnemyTileCount = 3;
    public Tile[] groundTiles;
    public Tile[] wallTiles;
    public PlayerController player;
    public FoodObject[] foodPrefabs;
    public WallObject[] wallPrefabs;
    public ExitObject exitPrefab;
    public EnemyObject[] enemyPrefabs;
    List<Vector2Int> emptyCells;
    public void Init()
    {
        tileMap = GetComponentInChildren<Tilemap>();
        grid = GetComponentInChildren<Grid>();
        emptyCells = new List<Vector2Int>();

        boardData = new CellData[width, height];
        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                Tile tile; 
                boardData[x, y] = new CellData();
                if (x == 0 || x == width-1 || y == 0 || y == height-1)
                {
                    tile = wallTiles[Random.Range(0, wallTiles.Length)];
                    boardData[x, y].Passable = false;
                }
                else
                {
                    tile = groundTiles[Random.Range(0, groundTiles.Length)];
                    boardData[x, y].Passable = true;
                    emptyCells.Add(new Vector2Int(x, y));
                }   
                tileMap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        emptyCells.Remove(new Vector2Int(1, 1));
        SpawnExit();
        SpawnWall();
        SpawnFood();
        SpawnEnemy();
    }
    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= width || cellIndex.y < 0 || cellIndex.y >= height)
        {
            return null;
        }
        return boardData[cellIndex.x, cellIndex.y];
    }
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }
    void SpawnFood()
    {
        int foodTileCount = Random.Range(minFoodTileCount, maxFoodTileCount);
        int spawnCount = 0;
        for (int i=0; i<foodTileCount; i++)
        {
            int randIndex = Random.Range(0, emptyCells.Count);
            Vector2Int randCell = emptyCells[randIndex];
            emptyCells.RemoveAt(randIndex);
            int randFoodIndex = Random.Range(0, foodPrefabs.Length);
            FoodObject newFood = Instantiate(foodPrefabs[randFoodIndex]);
            AddObject(newFood, randCell);
        }
        Debug.Log("Spawned " + spawnCount + " food");
    }
    void SpawnWall()
    {
        int wallTileCount = Random.Range(minWallTileCount, maxWallTileCount);
        int spawnCount = 0;
        for (int i=0; i<wallTileCount; i++)
        {
            int randIndex = Random.Range(0, emptyCells.Count);
            Vector2Int randCell = emptyCells[randIndex];
            emptyCells.RemoveAt(randIndex);
            int randWallIndex = Random.Range(0, wallPrefabs.Length);
            WallObject newWall = Instantiate(wallPrefabs[randWallIndex]);
            AddObject(newWall, randCell);
            spawnCount++;
        }
        Debug.Log("Spawned " + spawnCount + " wall");
    }
    void SpawnExit()
    {
        Vector2Int exitCell = new Vector2Int(width-2, height-2);
        ExitObject exit = Instantiate(exitPrefab);
        AddObject(exit, exitCell);
        emptyCells.Remove(exitCell);
    }
    void SpawnEnemy()
    {
        int enemyTileCount = Random.Range(minEnemyTileCount, maxEnemyTileCount);
        int spawnCount = 0;
        for (int i=0; i<enemyTileCount; i++){
            int randIndex = Random.Range(0, emptyCells.Count);
            Vector2Int randCell = emptyCells[randIndex];
            emptyCells.RemoveAt(randIndex);
            int randEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            EnemyObject newEnemy = Instantiate(enemyPrefabs[randEnemyIndex]);
            AddObject(newEnemy, randCell);
            spawnCount++;
        }
        Debug.Log("Spawned " + spawnCount + " enemy");
    }
    public void SetCellTile(Vector2Int cellIndex, Tile tile)
    {
        tileMap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
    }
    public void AddObject(CellObject obj, Vector2Int randCell)
    {
        CellData data = boardData[randCell.x, randCell.y];
        obj.transform.position = CellToWorld(randCell);
        data.containedObject = obj;
        obj.Init(randCell);
    }
    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return tileMap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }
    public void Clean()
    {
        if (boardData == null)
        {
            Debug.LogError("boardData is null. Ensure Init() is called before Clean().");
            return;
        }
        for (int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                var cellData = boardData[x, y];
                if (cellData.containedObject != null)
                {
                    Destroy(cellData.containedObject.gameObject);
                }
                tileMap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }
}
