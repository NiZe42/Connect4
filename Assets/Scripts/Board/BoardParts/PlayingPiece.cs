using System;
using System.Collections;
using UnityEngine;
using ActorColor = ActorEnum.ActorColor;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayingPiece : MonoBehaviour {

    [Tooltip("Time of move execution")]
    [SerializeField] private float moveTime;

    private ActorColor ownColor;

    private Vector2 targetPosition;
    public Action<PlayingPiece> OnMoveFinished;

    public void Initialize(ActorColor pColor, Vector2 pLocalPosition, Vector2 pTargetPosition, Vector3 localScale) {
        SetOwnColor(pColor);
        transform.localPosition = new Vector3(pLocalPosition.x, pLocalPosition.y, 0f);
        transform.localScale = localScale;
        SetTargetPosition(pTargetPosition);
    }

    public void SetOwnColor(ActorColor pColor) {
        ownColor = pColor;

        gameObject.GetComponent<SpriteRenderer>().color = ActorEnum.GetColor(ownColor);
    }

    public void SetTargetPosition(Vector2 pTargetPosition) {
        targetPosition = pTargetPosition;
    }

    public void StartLerping() {
        StartCoroutine(LerpToPosition());
    }

    public IEnumerator LerpToPosition() {
        float t = 0f;
        Vector2 start = new Vector2(transform.localPosition.x, transform.localPosition.y);

        while (t < moveTime) {
            t += Time.deltaTime;


            float proportion = t / moveTime;

            if (proportion >= 1) {
                proportion = 1;
                OnMoveFinished.Invoke(this);
            }

            transform.localPosition = Vector2.Lerp(start, targetPosition, proportion);

            yield return null;
        }
    }
}
