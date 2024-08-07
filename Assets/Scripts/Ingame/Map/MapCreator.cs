using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Logics;

public class MapCreator : MonoBehaviour
{

    //전체 맵의 격자 수, 격자당 크기
    public int width = 50;
    public int height = 50;
    private float gridSpaceSize = 1f;
    private MapData mapdata;

    [SerializeField] private GameObject gridCellPrefab;

    // Start is called before the first frame update

    public void init()
    {
        CreateGrid();
    }

    //맵 데이터를 바탕으로 맵에 격자 생성
    private void CreateGrid()
    {
        IngameManager.Instance.mapManager.width = width;
        IngameManager.Instance.mapManager.height = height;
        IngameManager.Instance.mapManager.gridSpaceSize = gridSpaceSize;
        IngameManager.Instance.mapManager.tile = new GameObject[width, height];
        IngameManager.Instance.mapManager.spots = new Vector3Int[width, height];
        IngameManager.Instance.mapManager.spots = new Vector3Int[width, height];

        mapdata = GameManager.Instance._data.totalDB.mapDatabase.MapDataList[GameManager.Instance.mapIndex];
        List<Vector2Int> doorPos = mapdata.getDoorPos();
        List<Vector2Int> inwalkable = mapdata.getInwalkables();
        List<Vector2Int> forbiddens = mapdata.getForbiddens();

        IngameManager.Instance.mapManager.map = new int[width, height];
        IngameManager.Instance.mapManager.forbiddens = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int currentCell = new Vector2Int(x, y);
                if (!inwalkable.Contains(currentCell))
                {
                    CreateTile(x, y);
                    IngameManager.Instance.mapManager.spots[x, y] = new Vector3Int(x, y, 0);
                }
                else if (doorPos.Contains(currentCell))
                {
                    CreateTile(x, y);
                    IngameManager.Instance.mapManager.spots[x, y] = new Vector3Int(x, y, 2);
                }
                else
                {
                    CreateTile(x, y);
                    IngameManager.Instance.mapManager.spots[x, y] = new Vector3Int(x, y, 1);
                }
                if (forbiddens.Contains(currentCell))
                {
                    IngameManager.Instance.mapManager.map[x, y] = 10;
                    IngameManager.Instance.mapManager.forbiddens.Add(currentCell);
                }
                else
                {
                    IngameManager.Instance.mapManager.map[x, y] = 0;
                }
            }
        }
    }

    //(x,y) 위치에 격자 생성, Astar에 이용될 Spots 매핑
    public void CreateTile(int x, int y)
    {
        IngameManager.Instance.mapManager.tile[x, y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
        IngameManager.Instance.mapManager.tile[x, y].GetComponent<GridCell>().SetPosition(x, y);
        IngameManager.Instance.mapManager.tile[x, y].transform.parent = transform;
    }


}
