using UnityEngine;


public class ActorEnum {
    public enum ActorColor : int {
        Red = 0,
        Yellow = 1,
        Empty = 2
    }

    static readonly Color[] RGBColors = new Color[] {
            Color.red,
            Color.yellow,
            Color.gray
        };

    public static Color GetColor(ActorColor color) {
        return RGBColors[(int)color];
    }

    public static ActorColor GetAnotherActorColor(ActorColor color) {
        if(color == ActorColor.Red) return ActorColor.Yellow;
        if(color == ActorColor.Yellow) return ActorColor.Red;

        Debug.LogError("Get Another Actor Color returned Empty");
        return ActorColor.Empty;
    }
}

