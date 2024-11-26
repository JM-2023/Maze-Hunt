using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int width = 10; // Number of cells horizontally
    public int height = 10; // Number of cells vertically
    public int pathWidth = 5; // Desired path width in units
    public int wallThickness = 1; // Wall thickness in units

    [Header("Prefabs")]
    public GameObject wallPrefab; // Prefab for walls
    public GameObject floorPrefab; // Prefab for floor tiles

    [Header("Starting Room Settings")]
    public int startingRoomSizeX = 3; // Starting room width in cells
    public int startingRoomSizeY = 3; // Starting room height in cells

    private bool[,] maze; // 2D array to represent the maze structure

    void Start()
    {
        GenerateMaze();
        BuildMaze();
        PositionPlayer();
    }

    void GenerateMaze()
    {
        int gridWidth = width * (pathWidth + wallThickness) + wallThickness;
        int gridHeight = height * (pathWidth + wallThickness) + wallThickness;
        maze = new bool[gridWidth, gridHeight];

        // Initialize maze with walls
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                maze[x, y] = true; // Wall by default
            }
        }

        // Create starting room
        CreateStartingRoom();

        // Start carving from just outside the starting room
        CarvePassagesFrom(startingRoomSizeX * (pathWidth + wallThickness), startingRoomSizeY * (pathWidth + wallThickness));
    }

    void CreateStartingRoom()
    {
        int roomWidth = startingRoomSizeX * (pathWidth + wallThickness) - wallThickness;
        int roomHeight = startingRoomSizeY * (pathWidth + wallThickness) - wallThickness;

        int startX = wallThickness;
        int startY = wallThickness;

        for (int x = startX; x < startX + roomWidth; x++)
        {
            for (int y = startY; y < startY + roomHeight; y++)
            {
                if (IsInMaze(x, y))
                {
                    maze[x, y] = false; // Path
                }
            }
        }
    }

    void CarvePassagesFrom(int currentX, int currentY)
    {
        CarvePath(currentX, currentY);

        int[] directions = { 0, 1, 2, 3 }; // Up, Down, Left, Right
        ShuffleArray(directions);

        foreach (int direction in directions)
        {
            int dx = 0, dy = 0;

            switch (direction)
            {
                case 0: dy = (pathWidth + wallThickness); break; // Up
                case 1: dy = -(pathWidth + wallThickness); break; // Down
                case 2: dx = -(pathWidth + wallThickness); break; // Left
                case 3: dx = (pathWidth + wallThickness); break; // Right
            }

            int newX = currentX + dx;
            int newY = currentY + dy;

            if (IsInMaze(newX, newY) && maze[newX, newY])
            {
                // Carve out the wall between cells
                int betweenX = currentX + dx / 2;
                int betweenY = currentY + dy / 2;
                CarvePath(betweenX, betweenY);

                CarvePassagesFrom(newX, newY);
            }
        }
    }

    void CarvePath(int x, int y)
    {
        int halfWidth = pathWidth / 2;

        for (int i = -halfWidth; i <= halfWidth; i++)
        {
            for (int j = -halfWidth; j <= halfWidth; j++)
            {
                int px = x + i;
                int py = y + j;

                if (IsInMaze(px, py))
                {
                    maze[px, py] = false; // Path
                }
            }
        }
    }

    bool IsInMaze(int x, int y)
    {
        return x >= 0 && x < maze.GetLength(0) && y >= 0 && y < maze.GetLength(1);
    }

    void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = temp;
        }
    }

    void BuildMaze()
    {
        float wallHeight = 3f; // Wall height

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Vector3 position = new Vector3(x, 0, y);

                if (!maze[x, y])
                {
                    // Place floor tile without scaling
                    Instantiate(floorPrefab, position, Quaternion.identity, transform);
                }
            }
        }

        // After placing floor tiles, place walls
        PlaceWalls(wallHeight);
    }

    void PlaceWalls(float wallHeight)
    {
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (maze[x, y])
                {
                    Vector3 position = new Vector3(x, wallHeight / 2, y);
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    void PositionPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            player.transform.position = new Vector3((startingRoomSizeX * (pathWidth + wallThickness)) / 2f, 1, (startingRoomSizeY * (pathWidth + wallThickness)) / 2f);
        }
    }
}
