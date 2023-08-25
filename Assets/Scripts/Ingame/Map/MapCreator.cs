using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapCreator : MonoBehaviour
{

    //전체 맵의 격자 수, 격자당 크기
    public int width = 50;
    public int height = 50;
    private float gridSpaceSize = 1f;

    [SerializeField] private GameObject gridCellPrefab;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {
        CreateGrid();
        CreateSpots();
    }

    //맵 데이터를 바탕으로 맵에 격자 생성
    private void CreateGrid()
    {
        MapManager.Instance.width = width;
        MapManager.Instance.height = height;
        MapManager.Instance.gridSpaceSize = gridSpaceSize;
        MapManager.Instance.tile = new GameObject[width, height];
        MapManager.Instance.spots = new Vector3Int[width, height];

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
        MapManager.Instance.spots = new Vector3Int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (true)
                {
                    MapManager.Instance.spots[x, y] = new Vector3Int(x, y, 0);
                }
                else
                {
                    MapManager.Instance.spots[x, y] = new Vector3Int(x, y, 1);
                }
            }
        }
    }

    //(x,y) 위치에 격자 생성, Astar에 이용될 Spots 매핑
    private void CreateTile(int x, int y)
    {
        MapManager.Instance.tile[x, y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
        MapManager.Instance.tile[x, y].GetComponent<GridCell>().SetPosition(x, y);
        MapManager.Instance.tile[x, y].transform.parent = transform;
    }


}
