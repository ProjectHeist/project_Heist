using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMapManager : MonoBehaviour
{

    //전체 맵의 격자 수, 격자당 크기
    private int width = 50;
    private int height = 50;
    private float gridSpaceSize = 1f;

    [SerializeField] private GameObject gridCellPrefab;
    private GameObject[,] tile;
    private int[,] map;

    private FindPath pathfinder = FindPath.Instance;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //맵 데이터를 바탕으로 맵에 격자 생성
    private void CreateGrid()
    {
        tile = new GameObject[width, height];
        pathfinder.spots = new Vector3Int[width, height];
        pathfinder.width = width;
        pathfinder.height = height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (true)
                {
                    CreateTile(x, y);
                }
                else
                {
                }
            }
        }
    }

    //이후 실제 그리드 생성과 Spots 배정을 분리할 예정
    private void CreateSpots()
    {
        pathfinder.spots = new Vector3Int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (true)
                {
                    pathfinder.spots[x, y] = new Vector3Int(x, y, 0);
                }
                else
                {
                    pathfinder.spots[x, y] = new Vector3Int(x, y, 1);
                }
            }
        }
    }

    //(x,y) 위치에 격자 생성, Astar에 이용될 Spots 매핑
    private void CreateTile(int x, int y)
    {
        tile[x, y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
        tile[x, y].GetComponent<GridCell>().SetPosition(x, y);
        tile[x, y].transform.parent = transform;
    }

    // 실제 좌표를 격자 내 좌표로 표현
    public Vector2Int GetGridPositionFromWorld(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSpaceSize);
        int y = Mathf.FloorToInt(worldPosition.z / gridSpaceSize);
        x = Mathf.Clamp(x, 0, width);
        y = Mathf.Clamp(y, 0, height);
        return new Vector2Int(x, y);
    }

    // 격자 내 좌표를 실제 좌표로 표현
    public Vector3 GetWorldPositionFromGridPosition(Vector2Int gridPosition)
    {
        float x = gridPosition.x * gridSpaceSize;
        float y = gridPosition.y * gridSpaceSize;
        return new Vector3(x, 0, y);
    }
}
