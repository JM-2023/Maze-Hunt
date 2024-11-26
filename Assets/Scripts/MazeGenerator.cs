using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Settings")]
    public int width = 10; // Number of cells horizontally
    public int height = 10; // Number of cells vertically
    public int pathWidth = 2; // Width of the paths
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject propPrefab;
    public int totalProps = 5;

    [Header("Starting Room Settings")]
    public int roomWidth = 4; // Width of the starting room
    public int roomHeight = 4; // Height of the starting room

    private bool[,] maze;

    void Start()
    {
        GenerateMaze();
        BuildMaze();
    }

    void GenerateMaze()
    {
        // Adjust maze size to account for path width
        int mazeWidth = width * (pathWidth + 1) + 1;
        int mazeHeight = height * (pathWidth + 1) + 1;

        maze = new bool[mazeWidth, mazeHeight];

        // Initialize maze with walls
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                maze[x, y] = true; // Wall
            }
        }

        // Create starting room
        for (int x = 1; x < roomWidth + 1; x++)
        {
            for (int y = 1; y < roomHeight + 1; y++)
            {
                maze[x, y] = false; // Path
            }
        }

        // Start Recursive Backtracking from the edge of the starting room
        CarvePassagesFrom(roomWidth, roomHeight);
    }

    void CarvePassagesFrom(int currentX, int currentY)
    {
        maze[currentX, currentY] = false; // Path

        int[] directions = { 0, 1, 2, 3 }; // Up, Down, Left, Right
        ShuffleArray(directions);

        foreach (int direction in directions)
        {
            int dx = 0, dy = 0;

            switch (direction)
            {
                case 0: dy = (pathWidth + 1); break; // Up
                case 1: dy = -(pathWidth + 1); break; // Down
                case 2: dx = -(pathWidth + 1); break; // Left
                case 3: dx = (pathWidth + 1); break; // Right
            }

            int newX = currentX + dx;
            int newY = currentY + dy;

            if (IsInMaze(newX, newY) && maze[newX, newY])
            {
                // Carve path width
                CarvePath(currentX, currentY, dx, dy);
                CarvePassagesFrom(newX, newY);
            }
        }
    }

    void CarvePath(int x, int y, int dx, int dy)
    {
        int pathLength = pathWidth + 1;

        for (int i = 0; i <= pathLength; i++)
        {
            int nx = x + (dx != 0 ? (dx / Mathf.Abs(dx)) * i : 0);
            int ny = y + (dy != 0 ? (dy / Mathf.Abs(dy)) * i : 0);

            if (IsInMaze(nx, ny))
            {
                // Carve out the path width
                for (int px = -pathWidth / 2; px <= pathWidth / 2; px++)
                {
                    for (int py = -pathWidth / 2; py <= pathWidth / 2; py++)
                    {
                        int carveX = nx + px;
                        int carveY = ny + py;

                        if (IsInMaze(carveX, carveY))
                        {
                            maze[carveX, carveY] = false;
                        }
                    }
                }
            }
        }
    }

    bool IsInMaze(int x, int y)
    {
        return x > 0 && x < maze.GetLength(0) - 1 && y > 0 && y < maze.GetLength(1) - 1;
    }

    void ShuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int rnd = Random.Range(i, array.Length);
            int temp = array[rnd];
            array[rnd] = array[i];
            array[i] = temp;
        }
    }

    void BuildMaze()
    {
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Vector3 position = new Vector3(x, 0, y);

                if (maze[x, y])
                {
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, transform);
                }
            }
        }

        // Place Props
        PlaceProps();
    }

    void PlaceProps()
    {
        int placedProps = 0;

        while (placedProps < totalProps)
        {
            int x = Random.Range(1, maze.GetLength(0) - 1);
            int y = Random.Range(1, maze.GetLength(1) - 1);

            if (!maze[x, y]) // If it's a path
            {
                Vector3 position = new Vector3(x, 0.5f, y);
                Instantiate(propPrefab, position, Quaternion.identity);
                placedProps++;
            }
        }
    }
}
