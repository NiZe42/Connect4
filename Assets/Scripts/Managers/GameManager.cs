using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ActorColor = ActorEnum.ActorColor;
public enum GameMode : int {
    PlayerAndPlayer = 0,
    PlayerAndAi = 1,
    AiAndPlayer = 2
}

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject aIPrefab;
    [SerializeField] private GameObject markPrefab;

    [Header("")]
    [SerializeField] private Transform actorsTransform;
    [SerializeField] private Board board;
    [SerializeField] private BoardControl boardControl;
    [SerializeField] private BoardCalculator boardCalculator;

    [Header("")]
    [SerializeField] private GameMode gameMode;

    private GridIndex currentTargetTile = new GridIndex(-1, -1);
    private int turnCount = 0;

    //could only be 2 players
    [HideInInspector]
    public Actor[] actors;

    private int currentActorIndex;
    private int CurrentActorIndex {
        get {
            return currentActorIndex;
        }
        set {
            currentActorIndex = value % 2;
        }
    }

    public void Awake() {

        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.LogWarning("GameManager already exists in the scene. Deleting this instance.");
            Destroy(this);
        }


    }

    public void Start() {
        InitializeBoard();
        InitalizeActors(gameMode);
    }

    private void InitializeBoard() {
        if (board == null) {
            Debug.LogError("Board has not been found by GameManager");
            return;
        }

        if (boardControl == null) {
            Debug.LogError("BoardControl has not been found by GameManager");
            return;
        }

        if (boardCalculator == null) {
            Debug.LogError("BoardCalculator has not been found by GameManager");
            return;
        }

        board.GenerateBoard();
        boardControl.OnMoveEnded += HandleBoardMoveEnd;
        boardControl.OnMoveStarted += HandleStartOfBoardMove;
        boardControl.OnMoveFailed += HandleBoardMoveFail;
    }

    private void InitalizeActors(GameMode gameMode) {
        actors = new Actor[2];

        switch (gameMode) {
            case GameMode.PlayerAndPlayer:
                actors[0] = Instantiate(playerPrefab, actorsTransform, false).GetComponent<Player>();
                actors[1] = Instantiate(playerPrefab, actorsTransform, false).GetComponent<Player>();
                break;

            case GameMode.PlayerAndAi:
                actors[0] = Instantiate(playerPrefab, actorsTransform, false).GetComponent<Player>();
                actors[1] = Instantiate(aIPrefab, actorsTransform, false).GetComponent<AI>();
                break;

            case GameMode.AiAndPlayer:
                actors[0] = Instantiate(aIPrefab, actorsTransform, false).GetComponent<AI>();
                actors[1] = Instantiate(playerPrefab, actorsTransform, false).GetComponent<Player>();
                break;

            default:
                Debug.LogError($"GameMode value is {gameMode} and it is not recognized");
                break;

        }

        actors[0].Initialize(ActorColor.Red);
        actors[1].Initialize(ActorColor.Yellow);

        CurrentActorIndex = 0;
        if (gameMode == GameMode.AiAndPlayer)
            StartCoroutine(DelayActivation(1f));
        else
            actors[currentActorIndex].SetIsActiveActor(true);

        UIManager.Instance.DisplayMessage($"{(ActorColor)currentActorIndex} turn");
    }

    private IEnumerator DelayActivation(float seconds) {
        yield return new WaitForSeconds(seconds);
        actors[currentActorIndex].SetIsActiveActor(true);
    }

    private void HandleStartOfBoardMove(Column column, Tile tile) {
        actors[CurrentActorIndex].SetIsActiveActor(false);

        currentTargetTile = board.GetIndexesByColumnAndTile(column, tile);
    }

    private void HandleBoardMoveFail() {
        UIManager.Instance.DisplayMessageForTime("The column is full. Please, select another", 2f);
    }

    private void HandleBoardMoveEnd() {

        (bool hasWon, List<GridIndex> winningLine) = boardCalculator.CheckWin(currentTargetTile, (ActorColor)currentActorIndex);
        if (hasWon) {
            winningLine.Add(new GridIndex(currentTargetTile.columnIndex, currentTargetTile.tileIndex));
            ProcessGameOver((ActorColor)currentActorIndex, winningLine);
            return;
        }

        turnCount++;
        if (turnCount == board.GetHorizontalTileCount() * board.GetVerticalTileCount()) {
            ProcessGameOver(ActorColor.Empty, null);
            return;
        }

        currentTargetTile = new GridIndex(-1, -1);
        CurrentActorIndex++;
        actors[CurrentActorIndex].SetIsActiveActor(true);
        UIManager.Instance.DisplayMessage($"{(ActorColor)currentActorIndex} turn");
    }

    private void ProcessGameOver(ActorColor winningColor, List<GridIndex> winningLine) {

        StartCoroutine(RestartGame());

        actors[CurrentActorIndex].SetIsActiveActor(false);

        if (winningColor != ActorColor.Empty) {
            UIManager.Instance.DisplayMessage($"{winningColor} has won");
            HighlightWinningLine(winningLine);
            return;
        }

        UIManager.Instance.DisplayMessage("Draw");
    }

    private void HighlightWinningLine(List<GridIndex> winningLine) {
        foreach (GridIndex winningTileIndex in winningLine) {
            Instantiate(markPrefab, board.GetTileByIndex(winningTileIndex).transform, false);
        }
    }

    private IEnumerator RestartGame() {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("GameScene");
    }

}
