using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GridIndex {
    public int columnIndex;
    public int tileIndex;

    public GridIndex(int pColumnIndex, int pTileIndex) {
        columnIndex = pColumnIndex;
        tileIndex = pTileIndex;
    }

    public bool IsValid() {
        if (columnIndex == -1 || tileIndex == -1) return false;

        return true;
    }

    public void Print() {
        Debug.Log($"GridIndex: {columnIndex}; {tileIndex}");
    }
}
[RequireComponent(typeof(BoardCalculator))]
[RequireComponent(typeof(BoardControl))]
public class Board : MonoBehaviour {
    [Header("Prefabs")]
    [SerializeField] private GameObject columnPrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject selectorPrefab;

    [Header("Coords of the board on Canvas")]
    [SerializeField] private Vector2 bottomLeftBoardPoint;
    [SerializeField] private Vector2 topRightBoardPoint;

    [Header("Tile count by X and Y respectively")]
    [SerializeField] private int horizontalTileCount;
    [SerializeField] private int verticalTileCount;

    [Header("CheckerBoard Color")]
    [SerializeField] private Color lightTile;
    [SerializeField] private Color darkTile;

    [Header("Selector properties")]
    [Tooltip("The size of Selector horizontally is ( horizontal size of tile) * horizontalSelectorSizeMult")]
    [SerializeField] private float horizontalSelectorMult;
    [Tooltip("The size of Selector vertically is ( vertical size of tile) / (2 + verticalSelectorSizeMult)")]
    [SerializeField] private float verticalSelectorMult;

    private List<GameObject> generatedObjects = null;

    [HideInInspector]
    public Vector3 playingPieceScale;

    public List<Column> columns = null;

    public void Initialize(int pHorizontalTileCount, int pVerticalTileCount) {
        horizontalTileCount = pHorizontalTileCount;
        verticalTileCount = pVerticalTileCount;
    }

    public void GenerateBoard() {

        if (horizontalTileCount == 0) {
            Debug.LogError("horizontalTileCount cant be 0");
            return;
        }

        if (verticalTileCount == 0) {
            Debug.LogError("verticalTileCount cant be 0");
            return;
        }

        if (bottomLeftBoardPoint.x > topRightBoardPoint.x || bottomLeftBoardPoint.y > topRightBoardPoint.y) {
            Debug.LogError("top right should be more top right than bottom left :0");
            return;
        }

        DeleteGenerated();
        ClearChildren();

        float horizontalTileScale = (topRightBoardPoint.x - bottomLeftBoardPoint.x) / horizontalTileCount;
        float verticalTileScale = (topRightBoardPoint.y - bottomLeftBoardPoint.y) / (verticalTileCount + .5f); // we are adding .25f for each selector

        //45 and 320 values are hardcoded as this is playing piece object relative to tile size
        playingPieceScale = new Vector3(horizontalTileScale / 320 * 45, verticalTileScale / 320 * 45, 0f);

        GenerateTiles(horizontalTileScale, verticalTileScale);
        GenerateSelectors(horizontalTileScale, verticalTileScale);
    }

    private void GenerateSelectors(float horizontalTileScale, float verticalTileScale) {
        Selector selector = Instantiate(selectorPrefab, transform, false).GetComponent<Selector>();

        AddGeneratedObject(selector.gameObject);
        selector.name = "Selector";

        
        selector.transform.localPosition = new Vector3(bottomLeftBoardPoint.x + 0.5f * horizontalTileScale, 0f, 0f);
        selector.Initialize(0);

        selector.topDiamond.transform.localPosition = new Vector3(0f, topRightBoardPoint.y - 0.125f * verticalTileScale, 0f);
        selector.topDiamond.transform.localScale = new Vector3(horizontalTileScale * horizontalSelectorMult,
            verticalTileScale / (2f + verticalSelectorMult), 0f);

        selector.bottomDiamond.transform.localPosition = new Vector3(0f, bottomLeftBoardPoint.y + 0.125f * verticalTileScale, 0f);
        selector.bottomDiamond.transform.localScale = new Vector3(horizontalTileScale * horizontalSelectorMult,
            verticalTileScale / (2f + verticalSelectorMult), 0f);

    }

    private void GenerateTiles(float horizontalTileScale, float verticalTileScale) {
        for (int x = 0; x < horizontalTileCount; x++) {
            Column genCol = Instantiate(columnPrefab, transform, false).GetComponent<Column>();

            AddColumn(genCol);
            AddGeneratedObject(genCol.gameObject);
            genCol.transform.localPosition = new Vector3(bottomLeftBoardPoint.x + (x + 0.5f) * horizontalTileScale, 0f, 0f);
            genCol.name = $"Column {x}";

            for (int y = 0; y < verticalTileCount; y++) {

                GameObject genTile = Instantiate(tilePrefab, genCol.transform, false);

                if ((x + y) % 2 == 0)
                    genTile.GetComponent<Image>().color = lightTile;
                else
                    genTile.GetComponent<Image>().color = darkTile;

                genTile.name = $"Tile {y}";
                genTile.transform.localPosition = new Vector3(0f, bottomLeftBoardPoint.y + (y + 0.75f) * verticalTileScale, 0f);
                genTile.transform.localScale = new Vector3(horizontalTileScale, verticalTileScale, 0);
            }

            genCol.GetTilesFromChildren();
        }
    }

    public void UpdateBoardVariables(Vector2 pBottomLeftBoardPoint, Vector2 pTopRightBoardPoint, int pHorizontalTileCount, int pVerticalTileCount) {
        bottomLeftBoardPoint = pBottomLeftBoardPoint;
        topRightBoardPoint = pTopRightBoardPoint;
        horizontalTileCount = pHorizontalTileCount;
        verticalTileCount = pVerticalTileCount;
    }


    private void AddColumn(Column newColumn) {
        if (columns == null) {
            columns = new List<Column>();
        }
        columns.Add(newColumn);
    }

    private void AddGeneratedObject(GameObject genObj) {
        if (generatedObjects == null) {
            generatedObjects = new List<GameObject>();
        }
        generatedObjects.Add(genObj);
    }

    public void DeleteGenerated() {
        if (generatedObjects == null)
            return;
        foreach (GameObject column in generatedObjects) {
            if (column == null)
                continue;

            DestroyImmediate(column.gameObject);
        }
        generatedObjects = null;

        columns = null;
    }

    public void ClearChildren() {
        GameObject tmp = new GameObject();
        while (transform.childCount != 0) {
            transform.GetChild(0).SetParent(tmp.transform);
        }

        DestroyImmediate(tmp);
        generatedObjects = null;
        columns = null;
    }

    public Column GetColumnByIndex(int columnIndex) {
        return columns[columnIndex];
    }

    public int GetIndexByColumn(Column column) {
        return columns.IndexOf(column);
    }

    public Tile GetTileByIndex(GridIndex gridIndex) {
        return columns[gridIndex.columnIndex].GetTileByIndex(gridIndex.tileIndex);
    }

    public GridIndex GetIndexesByTile(Tile tile) {
        foreach (Column column in columns) {
            int tileIndex = column.GetIndexByTile(tile);
            if (tileIndex != -1) {
                return new GridIndex(GetIndexByColumn(column), tileIndex);
            }
        }
        return new GridIndex(-1, -1);
    }

    public GridIndex GetIndexesByColumnAndTile(Column column, Tile tile) {
        return new GridIndex(GetIndexByColumn(column), column.GetIndexByTile(tile));
    }

    public int GetHorizontalTileCount() {
        return horizontalTileCount;
    }

    public int GetVerticalTileCount() {
        return verticalTileCount;
    }

    public void PrintColumns() {
        Debug.Log($"Columns: \n");
        foreach (Column column in columns) {
            Debug.Log($"{column}; \n");
        }
    }
}
