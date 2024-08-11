using UnityEngine;
using ActorColor = ActorEnum.ActorColor;

public enum SelectorMovementDir : int {
    Right = 1,
    Left = -1
}

public class Selector : MonoBehaviour {
    private Board board;
    private BoardControl boardControl;
    private int currentColumnIndex;

    public Color selectorColor = ActorEnum.GetColor(ActorColor.Empty);

    [SerializeField] public GameObject topDiamond;
    [SerializeField] public GameObject bottomDiamond;

    public void Initialize(int pColumnIndex) {
        currentColumnIndex = pColumnIndex;
        topDiamond.GetComponent<SpriteRenderer>().color = selectorColor;
        bottomDiamond.GetComponent<SpriteRenderer>().color = selectorColor;

        board = GetComponentInParent<Board>();
        if (board == null)
            Debug.LogError("No Board component found");

        boardControl = GetComponentInParent<BoardControl>();
        if (boardControl == null)
            Debug.LogError("No BoardControl component found");

    }

    public void ChangeSelectorColor(Color newColor) {
        selectorColor = newColor;
        topDiamond.GetComponent<SpriteRenderer>().color = selectorColor;
        bottomDiamond.GetComponent<SpriteRenderer>().color = selectorColor;
    }


    public void MoveSelector(SelectorMovementDir direction) {
        currentColumnIndex = (currentColumnIndex + (int)direction) % board.columns.Count;
        if (currentColumnIndex == -1)
            currentColumnIndex = 6;

        transform.localPosition = new Vector3(board.columns[currentColumnIndex].transform.localPosition.x, 0, 0);
    }

    public bool PlayPiece(ActorColor color) {
        return boardControl.PlayPiece(board.columns[currentColumnIndex], color);
    }


}
