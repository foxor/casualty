using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public static event Action OnSelectionMade = () => {};
    protected static Selector Instance;
    public static Selector Get {
        get {
            return Instance;
        }
    }
    public bool ShouldAllowInput {
        get {
            return TileController.AreClicksAllowed();
        }
        protected set {
            TileController.SetAllowClick(value);
        }
    }

    public bool UnitSelected {
        get {
            return Selected != null;
        }
    }

    protected TileState Selected;
    protected int moveCount = 0;

    public void Awake() {
        Instance = this;
        TileView.OnClicked += OnClicked;
        Board.OnReset += () => {moveCount = 0;};
        ClickBlocker.OnClicked += Deselect;
    }

    protected void OnClicked(HexPosition position) {
        var chosen = Board.Get[position];
        if (chosen.Friendliness == Friendliness.Friendly && !UnitSelected && chosen.alerted) {
            ShowOptions(chosen);
        }
        else if (UnitSelected && chosen.incomingMove) {
            ExecuteMove(chosen);
        }
        else {
            Deselect();
        }
    }

    protected void ShowOptions(TileState chosen) {
        var moves = chosen.PossibleMoves();
        if (moves.Any()) {
            foreach (var move in moves) {
                Board.Get[move].incomingMove = true;
            }
            Selected = chosen;
            chosen.isSelected = true;
            OnSelectionMade();
        }
    }

    protected void ExecuteMove(TileState chosen) {
        chosen.moveOrder = moveCount++;
        StartCoroutine(MoveCoroutine(Selected.ExecuteMove(chosen.position)));
    }

    protected IEnumerator MoveCoroutine(IEnumerator move) {
        ShouldAllowInput = false;
        Deselect();
        yield return StartCoroutine(move);
        yield return AIController.TakeAITurn();
        ShouldAllowInput = true;
        var overType = RulesChecker.IsLevelOver();
        switch (overType) {
        case GameOverStatus.NotOver:
            OnSelectionMade();
            break;
        case GameOverStatus.Win:
            yield return new WaitForSeconds(0.5f);
            yield return GameOverController.Win();
            yield return LevelController.Get.WinLevel();
            break;
        case GameOverStatus.Lose_MultipleDied:
            yield return new WaitForSeconds(0.5f);
            yield return GameOverController.LoseMultiDeath();
            yield return LevelController.Get.LoseLevel();
            break;
        case GameOverStatus.Lose_EnemiesSurvived:
            yield return new WaitForSeconds(0.5f);
            yield return GameOverController.LoseEnemySurvived();
            yield return LevelController.Get.LoseLevel();
            break;
        case GameOverStatus.Lose_NoDeaths:
            yield return new WaitForSeconds(0.5f);
            yield return GameOverController.LoseNoDeaths();
            yield return LevelController.Get.LoseLevel();
            break;
        }
    }

    protected void Deselect() {
        Selected = null;
        foreach (var position in HexPosition.AllBoardPositions()) {
            Board.Get[position].incomingMove = false;
            Board.Get[position].isSelected = false;
        }
        OnSelectionMade();
    }

    public static void SelectEnemy(TileState state) {
        state.isSelected = true;
        OnSelectionMade();
    }

    public static void DeselectEnemy(TileState state) {
        state.isSelected = false;
        OnSelectionMade();
    }
}
