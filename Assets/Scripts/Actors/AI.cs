using System.Collections.Generic;
using UnityEngine;
using ActorColor = ActorEnum.ActorColor;

public class AI : Actor {

    private Board board;
    private BoardCalculator boardCalculator;
    private BoardControl boardControl;

    public AI(ActorColor pColor, bool pCanAct) : base(pColor, pCanAct) {

    }

    private void Start() {
        GameObject boardGameObject = GameObject.FindGameObjectWithTag("Board");
        board = boardGameObject.GetComponent<Board>();
        boardControl = boardGameObject.GetComponent<BoardControl>();
        boardCalculator = boardGameObject.GetComponent<BoardCalculator>();

        if (board == null) {
            Debug.LogError("Board has not been found by AI Actor");
            return;
        }

        if (boardControl == null) {
            Debug.LogError("BoardControl has not been found by AI Actor");
            return;
        }

        if (boardCalculator == null) {
            Debug.LogError("BoardCalculator has not been found by AI Actor");
            return;
        }

    }

    private void Update() {
        if (isActiveActor) {
            Act();
        }

    }

    /// <summary>
    /// AI follows this strategy: 
    /// 1 If Ai can win - it wins 
    /// 2 If Ai can prevent opponent from winning - it prevents
    /// 3 Ai plays piece closest to the center
    /// </summary> 
    protected override void Act() {
        List<GridIndex> possibleMoves = FindPossibleMoves();

        Column winningColumn = FindWinningColumn(possibleMoves, color);
        if (winningColumn != null) {
            boardControl.PlayPiece(winningColumn, color);
            return;
        }

        Column notLosingColumn = FindWinningColumn(possibleMoves, ActorEnum.GetAnotherActorColor(color));
        if (notLosingColumn != null) {
            boardControl.PlayPiece(notLosingColumn, color);
            return;
        }

        GridIndex closestToCenterIndex = FindMoveClosestToTheCenter(possibleMoves);
        if (closestToCenterIndex.IsValid())
            boardControl.PlayPiece(board.GetColumnByIndex(closestToCenterIndex.columnIndex), color);
    }

    /// <summary>
    /// Returns all possible tile grid Indexes for move
    /// </summary> 
    private List<GridIndex> FindPossibleMoves() {
        List<GridIndex> possibleMoves = new List<GridIndex>();

        foreach (Column column in board.columns) {
            GridIndex nextGrindIndex = boardCalculator.FindNextGridIndexInColumn(column);
            if (!nextGrindIndex.IsValid())
                continue;

            possibleMoves.Add(nextGrindIndex);
        }

        //foreach (GridIndex gridIndex in possibleMoves) {
        //    gridIndex.Print();
        //}

        return possibleMoves;
    }


    /// <summary>
    /// Returns column that would win the game with specified color
    /// </summary> 
    private Column FindWinningColumn(List<GridIndex> possibleMoves, ActorColor color) {
        foreach (GridIndex targetGridIndex in possibleMoves) {
            if (boardCalculator.CheckWin(targetGridIndex, color).isWinning) {
                return board.GetColumnByIndex(targetGridIndex.columnIndex);
            }
        }

        return null;
    }

    private GridIndex FindMoveClosestToTheCenter(List<GridIndex> possibleMoves) {
        //the lower centerIndex, the closer tile is to the center
        float centerIndex = float.MaxValue;
        GridIndex closestIndex = new GridIndex(-1, -1);
        Vector2 boardCenter = new Vector2((board.GetHorizontalTileCount() - 1) / 2f, (board.GetVerticalTileCount() - 1) / 2f);
        foreach (GridIndex possibleMove in possibleMoves) {
            float currentCenterIndex = Mathf.Abs(boardCenter.x - possibleMove.columnIndex) + Mathf.Abs(boardCenter.y - possibleMove.tileIndex);
            //Debug.Log($"({possibleMove.columnIndex}; {possibleMove.tileIndex}): {currentCenterIndex}");
            if (currentCenterIndex < centerIndex) {
                centerIndex = currentCenterIndex;
                closestIndex = possibleMove;
            }

            if (currentCenterIndex == centerIndex) {
                if (Random.Range(0, 2) == 1) {
                    closestIndex = possibleMove;
                }
            }
        }

        if (!closestIndex.IsValid())
            Debug.LogError("No closest tiles to move to for Ai");

        return closestIndex;
    }



}
