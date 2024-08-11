using UnityEngine;
using ActorColor = ActorEnum.ActorColor;

public abstract class Actor : MonoBehaviour {
    public ActorColor color;

    protected bool isActiveActor = false;

    public Actor(ActorColor pColor, bool pIsActive) {
        color = pColor;
        SetIsActiveActor(pIsActive);
    }

    public virtual void Initialize(ActorColor pColor) {
        color = pColor;
    }

    protected abstract void Act();

    public void SetIsActiveActor(bool pIsActive) {
        isActiveActor = pIsActive;
    }

}
