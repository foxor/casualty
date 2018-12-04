using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStateToBackground : MonoBehaviour {
    protected static TileStateToBackground Instance;
    public static TileStateToBackground Get {
        get {
            return Instance;
        }
    }

    public void Awake() {
        Instance = this;
    }

    public Sprite Empty;
    public Sprite FriendlyIdle;
    public Sprite EnemyIdle;
    public Sprite FriendlyActive;
    public Sprite EnemyActive;
    public Sprite Transparent;

    public Sprite this [TileState state] {
        get {
            if (state.incomingMove) {
                return Instance.FriendlyIdle;
            }
            if (state.fireIncoming) {
                return Instance.EnemyIdle;
            }
            if (state.unit == UnitType.Hole) {
                return Instance.Transparent;
            }
            switch (state.Friendliness) {
            case Friendliness.Neutral:
            default:
                return Instance.Empty;
            case Friendliness.Friendly:
                if (state.isSelected) {
                    return Instance.FriendlyActive;
                }
                if (state.CanBeSelected && Selector.Get.ShouldAllowInput) {
                    return Instance.FriendlyIdle;
                }
                else {
                    return Instance.Empty;
                }
            case Friendliness.Hostile:
                if (Selector.Get.ShouldAllowInput || state.unit == UnitType.Fire) {
                    if (state.alerted) {
                        return Instance.EnemyActive;
                    }
                    else {
                        return Instance.EnemyIdle;
                    }
                }
                else {
                    if (state.isSelected) {
                        return Instance.EnemyActive;
                    }
                    else {
                        return Instance.Empty;
                    }
                }
            }
        }
    }
}
