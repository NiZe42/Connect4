using System.Collections.Generic;
using UnityEngine;
using ActorColor = ActorEnum.ActorColor;


public class BoardCalculator : MonoBehaviour {
    private struct LineDirection {
        private int columnDirection;
        public int ColumnDirection {
            get {
                return columnDirection;
            }
            set {
                if (value != 0 && Mathf.Abs(value) != 1) {
                    Debug.LogError("ColumnDirection in BoardCalculator is not 1, -1 or 0");
                }

                columnDirection = value;
            }
        }

        private int tileDirection;
        public int TileDirection {
            get {
                return tileDirection;
            }
            set {
                if (value != 0 && Mathf.Abs(value) != 1) {
                    Debug.LogError("TileDirection in BoardCalculator is not 1, -1 or 0");
                }

                tileDirection = value;
            }
        }


        public LineDirection(int pColumnDirection, int pTileDirection) {
            columnDirection = int.MinValue;
            tileDirection = int.MinValue;

            ColumnDirection = pColumnDirection;
            TileDirection = pTileDirection;

            if (columnDirection == int.MinValue || tileDirection == int.MinValue)
                Debug.LogError("columnDirection or tileDirection in BoardCalculator is not 1, -1 or 0");
        }
    }

    [Header("Game Settings")]
    [SerializeField] private int winningLength = 4;

    private readonly LineDirection[] lineDirections = { new LineDirection(1, 0),
                                               new LineDirection(1, 1),
                                               new LineDirection(0, 1),
                                               new LineDirection(1, -1)};

    private Board board;

    private void Awake() {


    }

    private void Start() {
        board = GetComponent<Board>();
    }

    public void Initialize(int pWinningLength) {
        winningLength = pWinningLength;
    }

    /// <summary>
    /// Returns (true, list of winning pieces) if game would be over with someone's win when piece of color would be placed on target position.
    /// Otherwise return (false, null)
    /// </summary>
    public (bool isWinning, List<GridIndex> returnWinningLine) CheckWin(GridIndex targetGridIndex, ActorColor color) {
        List<GridIndex> winningLine = GetLongestSurroundingLine(targetGridIndex, color);
        if (winningLine.Count >= winningLength - 1) {
            return (isWinning: true, returnWinningLine: winningLine);
        }

        return (isWinning: false, returnWinningLine: null);
    }

    /// <summary>
    /// Returns longest connected line of specific color not counting sent tile 
    /// </summary> 
    public List<GridIndex> GetLongestSurroundingLine(GridIndex tileGridIndex, ActorColor color) {

        int longestLine = 0;
        List<GridIndex> longestLineIndexes = new List<GridIndex>();
        foreach (LineDirection lineDirection in lineDirections) {
            List<GridIndex> tmpLineIndexes = GetLineAlongBothDirectionsAroundTile(tileGridIndex, lineDirection, color);
            int tmpLength = tmpLineIndexes.Count;

            if (tmpLength > longestLine) {
                longestLine = tmpLength;
                longestLineIndexes = tmpLineIndexes;
            }
        }
        return longestLineIndexes;
    }

    /// <summary>
    /// Returns list of GridIndexes of the connected line along a specific direction in both ways not counting sent tile
    /// </summary> 

    private List<GridIndex> GetLineAlongBothDirectionsAroundTile(GridIndex tileGridIndex, LineDirection direction, ActorColor color) {

        List<GridIndex> lineIndexes = GetLineAlongOneDirectionAroundTile(tileGridIndex, direction, color);
        List<GridIndex> otherLineIndexes = GetLineAlongOneDirectionAroundTile(tileGridIndex,
                                                                              new LineDirection(-direction.ColumnDirection, -direction.TileDirection), color);
        lineIndexes.AddRange(otherLineIndexes);
        ;

        return lineIndexes;
    }

    /// <summary>
    /// Returns list of GrindIndexes of the connected line along a specific direction in one way not counting sent tile
    /// </summary> 
    private List<GridIndex> GetLineAlongOneDirectionAroundTile(GridIndex tileGridIndex, LineDirection direction, ActorColor color) {
        List<GridIndex> lineIndexes = new List<GridIndex>();
        for (int i = 0; i < winningLength - 1; i++) {
            int nextColumnIndex = tileGridIndex.columnIndex + (i + 1) * direction.ColumnDirection;
            int nextTileIndex = tileGridIndex.tileIndex + (i + 1) * direction.TileDirection;
            if (!IsColumnIndexInRange(nextColumnIndex) || !IsTileIndexInRange(nextTileIndex))
                break;

            if (board.columns[nextColumnIndex].tiles[nextTileIndex].ownColor == color) {
                lineIndexes.Add(new GridIndex(nextColumnIndex, nextTileIndex));
            }

            else
                break;
        }
        return lineIndexes;
    }

    private bool IsColumnIndexInRange(int columnIndex) {
        if (columnIndex >= board.columns.Count || columnIndex < 0)
            return false;

        return true;
    }

    private bool IsTileIndexInRange(int tileIndex) {
        if (tileIndex >= board.columns[0].tiles.Count || tileIndex < 0)
            return false;

        return true;
    }

    public Tile FindNextTileInColumn(Column column) {

        for (int i = 0; i < column.tiles.Count; i++) {
            Tile currentTile = column.tiles[i];
            if (currentTile.ownColor == ActorColor.Empty)
                return currentTile;
        }
        return null;
    }

    public GridIndex FindNextGridIndexInColumn(Column column) {

        for (int i = 0; i < column.tiles.Count; i++) {
            Tile currentTile = column.tiles[i];
            if (currentTile.ownColor == ActorColor.Empty)
                return new GridIndex(board.GetIndexByColumn(column), i);
        }
        return new GridIndex(-1, -1);
    }
}
