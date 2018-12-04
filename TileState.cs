using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UnitType {
    Empty,
    Sword,
    Shield,
    Arrow,
    Fang,
    Bomb,
    Fire,
    Hole
}

public enum Friendliness {
    Neutral,
    Friendly,
    Hostile,
}

public class TileState {
    public HexPosition position;
    protected bool _alerted;
    public bool alerted {
        get {
            return _alerted;
        }
        set {
            if (_alerted != value) {
                _alerted = value;
                OnChanged(this);
            }
        }
    }
    protected UnitType _unit;
    public UnitType unit {
        get {
            return _unit;
        }
        set {
            if (_unit != value) {
                Clear();
                _unit = value;
                OnChanged(this);
            }
        }
    }
    protected bool _incomingMove;
    public bool incomingMove {
        get {
            return _incomingMove;
        }
        set {
            if (_incomingMove != value) {
                _incomingMove = value;
                OnChanged(this);
            }
        }
    }
    protected bool _isSelected;
    public bool isSelected {
        get {
            return _isSelected;
        }
        set {
            if (_isSelected != value) {
                _isSelected = value;
                OnChanged(this);
            }
        }
    }
    protected bool _fireIncoming;
    public bool fireIncoming {
        get {
            return _fireIncoming;
        }
        set {
            if (_fireIncoming != value) {
                _fireIncoming = value;
                OnChanged(this);
            }
        }
    }

    public bool PendingAwaken;

    public int moveOrder;
    public int playOrder;

    private void Clear() {
        _alerted = false;
        _incomingMove = false;
        _isSelected = false;
        _unit = UnitType.Empty;
        _fireIncoming = false;
    }
    
    public event Action<TileState> OnChanged = (_) => {};

    public bool CanBeSelected {
        get {
            return alerted && !Selector.Get.UnitSelected;
        }
    }

    public TileState(HexPosition position) {
        this.position = position;
    }
    public Friendliness Friendliness {
        get {
            switch (unit) {
            case UnitType.Arrow:
            case UnitType.Sword:
            case UnitType.Shield:
                return Friendliness.Friendly;
            case UnitType.Fang:
            case UnitType.Fire:
            case UnitType.Bomb:
                return Friendliness.Hostile;
            default:
            case UnitType.Empty:
            case UnitType.Hole:
                return Friendliness.Neutral;
            }
        }
    }

    public IEnumerable<HexPosition> PossibleMoves() {
        switch (unit) {
        case UnitType.Sword:
            foreach (var option in position.Neighbors()) {
                if (Board.Get[option].unit == UnitType.Empty) {
                    yield return option;
                }
            }
            break;
        case UnitType.Arrow:
            foreach (var direction in ((HexDirection[])Enum.GetValues(typeof(HexDirection)))) {
                foreach (var tile in position.Neighbors(direction)) {
                    yield return tile;
                }
            }
            break;
        case UnitType.Shield:
            foreach (var direction in ((HexDirection[])Enum.GetValues(typeof(HexDirection)))) {
                var neighborPosition = position.Neighbor(direction);
                if (Board.Get[neighborPosition].Friendliness == Friendliness.Friendly) {
                    var leapPosition = neighborPosition.Neighbor(direction);
                    if (leapPosition.IsInBounds && Board.Get[leapPosition].unit == UnitType.Empty) {
                        yield return leapPosition;
                    }
                }
            }
            break;
        }
    }

    protected static void AlertNeighbors(HexPosition position) {
        foreach (var enemy in position.Neighbors().Select(Board.ToState).Where(x => x.Friendliness == Friendliness.Hostile)) {
            enemy.PendingAwaken = true;
        }
    }

    public IEnumerator ExecuteMove(HexPosition target) {
        switch (unit) {
        case UnitType.Sword:
            SFXController.PlayMove();
            Board.Get[target].unit = unit;
            Board.Get[target].alerted = false;
            unit = UnitType.Empty;
            alerted = false;
            AlertNeighbors(target);

            var directions = ((HexDirection[])Enum.GetValues(typeof(HexDirection))).Where(x => {
                var neighbor = position.Neighbor(x);
                return neighbor.Equals(target);
            });
            yield return new WaitForSeconds(0.3f);
            if (directions.Any()) {
                var stabTile = Board.Get[target.Neighbor(directions.Single())];
                if (stabTile.Friendliness != Friendliness.Neutral) {
                    SFXController.PlayFire();
                    yield return DestroyController.Destroy(stabTile);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            break;
        case UnitType.Arrow:
            SFXController.PlayFire();
            yield return new WaitForSeconds(0.3f);
            var direction = ((HexDirection[])Enum.GetValues(typeof(HexDirection)))
                .Where(x => position.Neighbors(x).Contains(target))
                .Single();
            var tile = Board.Get[position.Neighbor(direction)];
            while (tile.position.IsInBounds && tile.Friendliness == Friendliness.Neutral) {
                tile = Board.Get[tile.position.Neighbor(direction)];
            }
            if (tile.position.IsInBounds) {
                yield return DestroyController.Destroy(tile);
            }
            yield return new WaitForSeconds(0.5f);
            alerted = true;
            break;
        case UnitType.Shield:
            SFXController.PlayMove();
            Board.Get[target].unit = unit;
            Board.Get[target].alerted = true;
            unit = UnitType.Empty;
            alerted = false;
            AlertNeighbors(target);
            yield return new WaitForSeconds(0.3f);
            break;
        }
        
        yield return YieldFast.Get;
    }

    public static IEnumerable<TileState> AllTiles() {
        return HexPosition.AllBoardPositions().Select(x => Board.Get[x]);
    }
}