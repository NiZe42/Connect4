using System;
using UnityEngine;
using ActorColor = ActorEnum.ActorColor;


public class BoardControl : MonoBehaviour {
    [SerializeField] private GameObject playingPiecePrefab;
    [SerializeField] private Transform playingPiecesParent;

    private BoardCalculator boardCalculator;
    private Board board;

    public Action OnMoveEnded;
    public Action<Column, Tile> OnMoveStarted;
    public Action OnMoveFailed;

    private void Start() {
        board = GetComponent<Board>();
        boardCalculator = GetComponent<BoardCalculator>();
    }

    public bool PlayPiece(Column column, ActorColor color) {
        Tile targetTile = boardCalculator.FindNextTileInColumn(column);

        if (targetTile == null) {
            OnMoveFailed.Invoke();
            return false;
        }

        Vector2 targetPosition = new Vector2(column.transform.localPosition.x, targetTile.transform.localPosition.y);
        PlayingPiece currentPiece = Instantiate(playingPiecePrefab, playingPiecesParent, false).GetComponent<PlayingPiece>();
        currentPiece.OnMoveFinished += FinishTheMove;

        currentPiece.Initialize(color, new Vector2(column.transform.localPosition.x, 2500f), targetPosition, board.playingPieceScale);
        currentPiece.StartLerping();
        targetTile.ownColor = color;

        OnMoveStarted.Invoke(column, targetTile);
        return true;
    }

    private void FinishTheMove(PlayingPiece piece) {

        piece.OnMoveFinished -= FinishTheMove;
        OnMoveEnded.Invoke();
    }



}



