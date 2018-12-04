using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController {
    public static IEnumerator TakeAITurn() {
        foreach (var state in TileState.AllTiles()
            .Where(x => x.Friendliness == Friendliness.Hostile)
            .OrderBy(x => -x.playOrder))
        {
            if (state.Friendliness == Friendliness.Hostile) {
                yield return TakeTurn(state);
                if (state.unit != UnitType.Fire && state.alerted) {
                    yield return new WaitForSeconds(0.6f);
                }
            }
        }
        foreach (var state in TileState.AllTiles()) {
            if (state.fireIncoming) {
                state.unit = UnitType.Fire;
                state.alerted = true;
                if (!Generator.isRunning) {
                    SFXController.PlayFire();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        yield return YieldFast.Get;
    }

    protected static IEnumerator TakeTurn(TileState state) {
        if (state.alerted) {
            yield return DoBehavior(state);
        }
        else {
            if (state.PendingAwaken) {
                state.alerted = true;
                state.PendingAwaken = false;
            }
        }
        yield return YieldFast.Get;
    }

    protected static int GetInteractionRange(UnitType unit) {
        switch (unit) {
            default:
                return 0;
            case UnitType.Fire:
            case UnitType.Fang:
                return 1;
        }
    }

    protected static IEnumerator DoBehavior(TileState state) {
        Selector.SelectEnemy(state);
        int range = GetInteractionRange(state.unit);
        var neighbors = state.position.Neighbors(maxDistance: range).Select(Board.ToState);
        state.alerted = false;
        switch (state.unit) {
        case UnitType.Bomb:
            yield return DestroyController.Destroy(state);
            break;
        case UnitType.Fire:
            foreach (var neighborState in neighbors) {
                if (!neighborState.fireIncoming && neighborState.unit != UnitType.Fire && neighborState.unit != UnitType.Hole) {
                    yield return DestroyController.Destroy(neighborState);
                    SFXController.PlayFire();
                    neighborState.fireIncoming = true;
                    if (!Generator.isRunning) {
                        yield return new WaitForSeconds(0.05f);
                    }
                }
            }
            break;
        case UnitType.Fang:
            yield return new WaitForSeconds(0.7f);
            var enemies = neighbors.Where(x => x.Friendliness == Friendliness.Friendly)
                .OrderBy(x => -x.moveOrder * 1000 + x.playOrder);
            if (enemies.Any()) {
                var target = enemies.First();
                SFXController.PlayFire();
                if (target.unit != UnitType.Shield) {
                    yield return DestroyController.Destroy(enemies.First());
                }
                else {
                    yield return DestroyController.Destroy(state);
                }
                // TODO: strike animation
                yield return new WaitForSeconds(0.6f);
            }
            break;
        }
        Selector.DeselectEnemy(state);
        yield return YieldFast.Get;
    }
}
