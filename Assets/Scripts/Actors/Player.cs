using UnityEngine;
using ActorColor = ActorEnum.ActorColor;


public class Player : Actor {

    private Selector selector;
    public Player(ActorColor pColor, bool pCanAct) : base(pColor, pCanAct) {

    }


    private void Awake() {

    }

    private void Start() {
        Initalize();
    }

    public void Initalize() {
        selector = GameObject.FindGameObjectWithTag("Selector").GetComponent<Selector>();

        if (selector == null)
            Debug.LogError("Selector has not been found by Player");
    }

    protected override void Act() {
        if (selector.selectorColor != ActorEnum.GetColor(color)) {
            selector.ChangeSelectorColor(ActorEnum.GetColor(color));
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            selector.MoveSelector(SelectorMovementDir.Right);
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            selector.MoveSelector(SelectorMovementDir.Left);
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
            selector.PlayPiece(color);
        }
    }

    private void Update() {
        if (isActiveActor) {
            Act();
        }
    }

}
