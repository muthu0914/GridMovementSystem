using UnityEngine;
using System.Collections;

public class GridUtil : MonoBehaviour {

    [SerializeField]
    private int rows;
    [SerializeField]
    private int cols;
    [SerializeField]
    private Vector2 gridSize;
    [SerializeField]
    private Vector2 gridOffset;

    [SerializeField]
    private Sprite cellSprite;
    private Vector2 cellSize;
    private Vector2 cellScale;

    void Awake() {
        GenerateGrid(); //Initialize Grid
	}

    void GenerateGrid() {
        GameObject cellObject = new GameObject();
        cellObject.AddComponent<SpriteRenderer>().sprite = cellSprite;
        
        //set color using Hex color codes
        Color color;
        if(ColorUtility.TryParseHtmlString("#0D4CD4",out color)){
            cellObject.GetComponent<SpriteRenderer>().color = color;
        }

        //scaling to fit the grid
        Vector2 newCellSize = new Vector2(gridSize.x / (float)cols, gridSize.y / (float)rows);
        cellSize = cellSprite.bounds.size;
        cellScale.x = newCellSize.x / cellSize.x;
        cellScale.y = newCellSize.y / cellSize.y;
        cellSize = newCellSize; 
        cellObject.transform.localScale = new Vector2(cellScale.x, cellScale.y);

        //offset for grid cells
        gridOffset.x = -(gridSize.x / 2) + cellSize.x / 2;
        gridOffset.y = -(gridSize.y / 2) + cellSize.y / 2;

        for (int row = 0; row < rows; row++) {
            for (int col = 0; col < cols; col++) {
                Vector2 pos = new Vector2(col * cellSize.x + gridOffset.x, row * cellSize.y + gridOffset.y);
                
                GameObject cO = Instantiate(cellObject, pos, Quaternion.identity) as GameObject;

                cO.transform.parent = transform;
            }
        }

        Destroy(cellObject);
    }

    public Vector2 getCellSize(){
        return cellSize;
    }
    
    //Grid Width,Height in Editor View
    void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, gridSize);
    }
}