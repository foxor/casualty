using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameOverStatus {
    NotOver,
    Win,
    Lose_EnemiesSurvived,
    Lose_MultipleDied,
    Lose_NoDeaths,
}

public class RulesChecker : MonoBehaviour {
    public void Awake() {
        DestroyController.OnDestroy += OnDestroy;
        Board.OnReset += ResetDeaths;
    }
    protected static int friendlyDeaths;

    protected static void ResetDeaths() {
        friendlyDeaths = 0;
    }

    protected static void OnDestroy(TileState state) {
        if (state.Friendliness == Friendliness.Friendly) {
            friendlyDeaths ++;
        }
    }

    protected static bool MovableFriendly(TileState state) {
        if (!state.alerted) {
            return false;
        }
        if (state.unit != UnitType.Shield) {
            return state.Friendliness == Friendliness.Friendly;
        }
        else {
            return state.PossibleMoves().Any();
        }
    }

    public static GameOverStatus IsLevelOver() {
        var tiles = HexPosition.AllBoardPositions().Select(Board.ToState);
        bool enemiesRemain = tiles.Any(x => x.Friendliness == Friendliness.Hostile && x.unit != UnitType.Fire);
        if (!enemiesRemain && friendlyDeaths == 1) {
            return GameOverStatus.Win;
        }
        if (friendlyDeaths > 1) {
            return GameOverStatus.Lose_MultipleDied;
        }
        bool forceGameOver = !tiles.Any(MovableFriendly);
        if (forceGameOver) {
            if (enemiesRemain) {
                return GameOverStatus.Lose_EnemiesSurvived;
            }
            return GameOverStatus.Lose_NoDeaths;
        }
        return GameOverStatus.NotOver;
    }
}
