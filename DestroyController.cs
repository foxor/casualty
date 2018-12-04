using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DestroyController {
    public static event Action<TileState> OnDestroy = (_) => {};
    public static IEnumerator Destroy(TileState state) {
        if (state.unit == UnitType.Fire) {
            state.alerted = true;
        }
        else {
            OnDestroy(state);
            if (state.unit == UnitType.Bomb) {
                var unlitNeighbors = state.position.Neighbors(maxDistance: 1).Select(x => Board.Get[x]).Where(x => !x.fireIncoming).ToArray();
                float playerCanSeeFireIncomingTime = 0f;
                foreach (var neighbor in unlitNeighbors) {
                    neighbor.fireIncoming = true;
                    playerCanSeeFireIncomingTime += 0.05f;
                }
                yield return new WaitForSeconds(playerCanSeeFireIncomingTime);
                state.unit = UnitType.Fire;
                state.fireIncoming = true;
                SFXController.PlayExplosion();
                ScreenShakeManager.MajorTrauma();
                foreach (var neighbor in unlitNeighbors) {
                    if (neighbor.Friendliness != Friendliness.Neutral) {
                        yield return Destroy(neighbor);
                        neighbor.fireIncoming = true;
                    }
                }
                yield return new WaitForSeconds(0.3f);
            }
            else {
                if (state.Friendliness == Friendliness.Friendly || state.Friendliness == Friendliness.Hostile) {
                    ScreenShakeManager.MinorTrauma();
                }
                state.unit = UnitType.Empty;
            }
            state.alerted = false;
        }
        yield return YieldFast.Get;
    }
}
