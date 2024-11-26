using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class MazeGeneratorTool : EditorWindow
{
    private int width = 10;
    private int height = 10;
    private int pathWidth = 5;
    private int wallThickness = 1;
    private int startingRoomSizeX = 3;
    private int startingRoomSizeY = 3;
    private int endRoomSizeX = 3;
    private int endRoomSizeY = 3;
    private GameObject wallPrefab;
    private GameObject floorPrefab;
    private GameObject mazeParent;
    private bool[,] maze;
    private Vector2Int endRoomPosition;

    [MenuItem("Tools/Maze Generator")]
    public static void ShowWindow()
    {
        GetWindow<MazeGeneratorTool>("Maze Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Maze Generator Settings", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        GUILayout.Label("Size Settings", EditorStyles.boldLabel);
        width = EditorGUILayout.IntField("Width (cells)", width);
        height = EditorGUILayout.IntField("Height (cells)", height);
        pathWidth = EditorGUILayout.IntField("Path Width", pathWidth);
        wallThickness = EditorGUILayout.IntField("Wall Thickness", wallThickness);

        EditorGUILayout.Space();
        GUILayout.Label("Room Settings", EditorStyles.boldLabel);
        startingRoomSizeX = EditorGUILayout.IntField("Starting Room Width", startingRoomSizeX);
        startingRoomSizeY = EditorGUILayout.IntField("Starting Room Height", startingRoomSizeY);
        endRoomSizeX = EditorGUILayout.IntField("End Room Width", endRoomSizeX);
        endRoomSizeY = EditorGUILayout.IntField("End Room Height", endRoomSizeY);

        EditorGUILayout.Space();
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);
        wallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", wallPrefab, typeof(GameObject), false);
        floorPrefab = (GameObject)EditorGUILayout.ObjectField("Floor Prefab", floorPrefab, typeof(GameObject), false);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Maze"))
        {
            GenerateMaze();
        }

        if (GUILayout.Button("Save as Prefab"))
        {
            if (mazeParent != null)
            {
                SaveMazeAsPrefab();
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please generate a maze first!", "OK");
            }
        }
    }

    void GenerateMaze()
    {
        if (wallPrefab == null || floorPrefab == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign all prefabs first!", "OK");
            return;
        }

        // Remove old maze if it exists
        if (mazeParent != null)
        {
            DestroyImmediate(mazeParent);
        }

        // Create parent object for maze
        mazeParent = new GameObject("Generated Maze");
        Undo.RegisterCreatedObjectUndo(mazeParent, "Generate Maze");

        // Add outer wall padding to the grid dimensions
        int gridWidth = width * (pathWidth + wallThickness) + wallThickness + (2 * wallThickness);
        int gridHeight = height * (pathWidth + wallThickness) + wallThickness + (2 * wallThickness);
        maze = new bool[gridWidth, gridHeight];

        // Initialize maze with walls
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                maze[x, y] = true;
            }
        }

        // Create starting room
        CreateStartingRoom();

        // Determine end room position (opposite corner from start)
        int endX = gridWidth - wallThickness - (endRoomSizeX * (pathWidth + wallThickness));
        int endY = gridHeight - wallThickness - (endRoomSizeY * (pathWidth + wallThickness));
        endRoomPosition = new Vector2Int(endX, endY);
        
        // Start carving from just outside the starting room
        CarvePassagesFrom(startingRoomSizeX * (pathWidth + wallThickness) + wallThickness, 
                         startingRoomSizeY * (pathWidth + wallThickness) + wallThickness);

        // Create end room
        CreateEndRoom();

        // Ensure path to end room
        EnsurePathToEndRoom();

        BuildMaze();
    }

    void CreateStartingRoom()
    {
        int roomWidth = startingRoomSizeX * (pathWidth + wallThickness) - wallThickness;
        int roomHeight = startingRoomSizeY * (pathWidth + wallThickness) - wallThickness;

        // Adjust start position to account for outer walls
        int startX = wallThickness * 2;
        int startY = wallThickness * 2;

        for (int x = startX; x < startX + roomWidth; x++)
        {
            for (int y = startY; y < startY + roomHeight; y++)
            {
                if (IsInMaze(x, y))
                {
                    maze[x, y] = false;
                }
            }
        }
    }

    void CreateEndRoom()
    {
        int roomWidth = endRoomSizeX * (pathWidth + wallThickness) - wallThickness;
        int roomHeight = endRoomSizeY * (pathWidth + wallThickness) - wallThickness;

        for (int x = endRoomPosition.x; x < endRoomPosition.x + roomWidth; x++)
        {
            for (int y = endRoomPosition.y; y < endRoomPosition.y + roomHeight; y++)
            {
                if (IsInMaze(x, y))
                {
                    maze[x, y] = false;
                }
            }
        }
    }

    void EnsurePathToEndRoom()
    {
        // Create a path from the last carved passage to the end room
        int currentX = maze.GetLength(0) - wallThickness * 3;
        int currentY = maze.GetLength(1) - wallThickness * 3;
        
        // Find the nearest carved path
        bool foundPath = false;
        int searchRadius = 1;
        while (!foundPath && searchRadius < maze.GetLength(0))
        {
            for (int x = -searchRadius; x <= searchRadius; x++)
            {
                for (int y = -searchRadius; y <= searchRadius; y++)
                {
                    int checkX = currentX + x;
                    int checkY = currentY + y;
                    
                    if (IsInMaze(checkX, checkY) && !maze[checkX, checkY])
                    {
                        // Found a path, now connect to it
                        CarvePath(checkX, checkY);
                        CarvePath((checkX + currentX) / 2, (checkY + currentY) / 2);
                        foundPath = true;
                        break;
                    }
                }
                if (foundPath) break;
            }
            searchRadius++;
        }
    }

    void CarvePassagesFrom(int currentX, int currentY)
    {
        CarvePath(currentX, currentY);

        int[] directions = { 0, 1, 2, 3 };
        ShuffleArray(directions);

        foreach (int direction in directions)
        {
            int dx = 0, dy = 0;

            switch (direction)
            {
                case 0: dy = (pathWidth + wallThickness); break;
                case 1: dy = -(pathWidth + wallThickness); break;
                case 2: dx = -(pathWidth + wallThickness); break;
                case 3: dx = (pathWidth + wallThickness); break;
            }

            int newX = currentX + dx;
            int newY = currentY + dy;

            if (IsInMaze(newX, newY) && maze[newX, newY])
            {
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
                    maze[px, py] = false;
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
        float wallHeight = 3f;

        // Create floor parent
        GameObject floorParent = new GameObject("Floor");
        floorParent.transform.parent = mazeParent.transform;

        // Create walls parent
        GameObject wallsParent = new GameObject("Walls");
        wallsParent.transform.parent = mazeParent.transform;

        // Create outer walls parent
        GameObject outerWallsParent = new GameObject("Outer Walls");
        outerWallsParent.transform.parent = mazeParent.transform;

        // Place floor and inner walls
        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                Vector3 position = new Vector3(x, 0, y);

                if (!maze[x, y])
                {
                    GameObject floor = (GameObject)PrefabUtility.InstantiatePrefab(floorPrefab, floorParent.transform);
                    floor.transform.position = position;
                }
                else if (!IsOuterWall(x, y))
                {
                    Vector3 wallPosition = new Vector3(x, wallHeight / 2, y);
                    GameObject wall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, wallsParent.transform);
                    wall.transform.position = wallPosition;
                }
            }
        }

        // Place outer walls
        PlaceOuterWalls(outerWallsParent, wallHeight);
    }

    bool IsOuterWall(int x, int y)
    {
        return x == 0 || y == 0 || x == maze.GetLength(0) - 1 || y == maze.GetLength(1) - 1;
    }

    void PlaceOuterWalls(GameObject parent, float wallHeight)
    {
        int maxX = maze.GetLength(0) - 1;
        int maxY = maze.GetLength(1) - 1;

        // Place bottom and top walls
        for (int x = 0; x <= maxX; x++)
        {
            Vector3 bottomPosition = new Vector3(x, wallHeight / 2, 0);
            Vector3 topPosition = new Vector3(x, wallHeight / 2, maxY);
            
            GameObject bottomWall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent.transform);
            GameObject topWall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent.transform);
            
            bottomWall.transform.position = bottomPosition;
            topWall.transform.position = topPosition;
        }

        // Place left and right walls
        for (int y = 1; y < maxY; y++)
        {
            Vector3 leftPosition = new Vector3(0, wallHeight / 2, y);
            Vector3 rightPosition = new Vector3(maxX, wallHeight / 2, y);
            
            GameObject leftWall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent.transform);
            GameObject rightWall = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefab, parent.transform);
            
            leftWall.transform.position = leftPosition;
            rightWall.transform.position = rightPosition;
        }
    }

    void SaveMazeAsPrefab()
    {
        string path = EditorUtility.SaveFilePanel("Save Maze Prefab", "Assets", "GeneratedMaze", "prefab");
        if (string.IsNullOrEmpty(path)) return;

        // Convert absolute path to relative path
        path = "Assets" + path.Substring(Application.dataPath.Length);

        // Create the prefab
        PrefabUtility.SaveAsPrefabAsset(mazeParent, path);
        EditorUtility.DisplayDialog("Success", "Maze saved as prefab!", "OK");
    }
}
#endif