using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    // Start is called before the first frame update
    private int width = 25;
    private int height = 25;
    private float gridSpaceSize = 1f;
    [SerializeField] private GameObject gridCellPrefab;
    private GameObject[,] tile;
    void Start()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void CreateGrid()
    {
        tile = new GameObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tile[x, y] = Instantiate(gridCellPrefab, new Vector3(x * gridSpaceSize, 0, y * gridSpaceSize), Quaternion.identity);
                tile[x, y].transform.parent = transform;
            }
        }
    }
}
