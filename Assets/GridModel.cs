using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridModel : MonoBehaviour
{
    public static GridModel Instance { get; private set; }
    
    private Grid grid;
    
    //Public default variabLes?
    public GameObject cellPrefab;
    public GameObject gridAnchorPoint;
    public float cellSize = 120;
    public float borderSize = 20;
    public float cellSpacing = 20;
    
    //Public settings variables
    public int xSize = 12;
    public int ySize = 12;

    public int snakeStartX = -1;
    public int snakeStartY = -1;
    
    //Colors
    public Color backGroundColor;
    public Color snakeColor;
    public Color fruitColor;
    public Color wallColor;
    public Color emptyColor;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        if (grid == null)
        {
            CreateNewGrid();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Create grid in Edit mode")]
    public void CreateNewGrid()
    {
        if (gridAnchorPoint.transform.childCount > 0)
        {
            var childList = new Transform[gridAnchorPoint.transform.childCount];
            var i = 0;
            for (; i < gridAnchorPoint.transform.childCount; i++)
            {
                childList[i] = gridAnchorPoint.transform.GetChild(i);
            }

            for (i = i - 1; i > -1; i--)
            {
                GameObject.DestroyImmediate(childList[i].gameObject);
            }
        }

        if (snakeStartX < 0 || snakeStartY < 0)
        {
            grid = new Grid(xSize, ySize);

        }
        else
        {
            grid = new Grid(xSize, ySize, snakeStartX, snakeStartY);

        }
        
        //Create and Render the models
        Vector3 anchorStart = gridAnchorPoint.transform.position;

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                Occupant o = grid.GetCellValue(x, y);
                GameObject go = Instantiate(cellPrefab, gridAnchorPoint.transform);
                CellModel cm = go.GetComponent<CellModel>();

                cm.Init(grid.GetCell(x,y));
                cm.SetSize(cellSize - borderSize, cellSize);
                grid.GetCell(x, y).SetModel(cm);

                var newPos = anchorStart;
                newPos.x += (x * (cellSize + cellSpacing)) + cellSpacing;
                newPos.y += (y * (cellSize + cellSpacing)) + cellSpacing;
                
                go.transform.position = newPos;
                
                cm.UpdateCellColor(GetOccupantColor(o));
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public Color GetOccupantColor(Occupant occupant)
    {
        switch (occupant)
        {
            case Occupant.empty:
                return emptyColor;
            case Occupant.fruit:
                return fruitColor;
            case Occupant.snake:
                return snakeColor;
            case Occupant.wall:
                return wallColor;
            default:
                Debug.LogError("Somehow we got passed an occupant that doesn't exist");
                return Color.magenta;
        }
    }

    public CellModel SetCell(int x, int y, Occupant newOccupant)
    {
        var cell = grid.GetCell(x, y);
        cell.SetCellValue(newOccupant);
        cell.GetModel().UpdateCellColor();

        return cell.GetModel();
    }

    public Cell GetCell(int x, int y)
    {
        //Debug.Log(grid.GetCell(x,y));
        return grid.GetCell(x, y);
    }
}
