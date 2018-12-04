using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HexDirection {
    UpLeft,
    UpRight,
    Left,
    Right,
    DownLeft,
    DownRight,
}

[Serializable]
public class HexPosition : IEquatable<HexPosition>
{
    public static readonly int HEX_SIZE = 32;
    public static readonly int HEX_RADIUS = HEX_SIZE / 2;
    public static readonly int HEX_OFFSET = HEX_SIZE / 4;
    public static readonly int Y_OFFSET_PER_ROW = 1;
    public static readonly int INTERROW_HEIGHT = HEX_RADIUS + HEX_OFFSET - Y_OFFSET_PER_ROW;

    public int x;
    public int y;

    public HexPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }
    public HexPosition() : this(0, 0) {}

    public bool IsInOffsetRow {
        get {
            return y % 2 == Board.Get.boardEdgeLength % 2;
        }
    }

    public override bool Equals(object obj) {
        if (obj is HexPosition) {
            return Equals(obj as HexPosition);
        }
        return false;
    }

    public bool Equals(HexPosition other) {
        return x == other.x && y == other.y;
    }

    public override int GetHashCode() {
        return x + y * Board.Get.boardDiameter;
    }

    protected static int PhantomColumns(int y) {
        int distanceFromYBound = Mathf.Min(y, Board.Get.boardDiameter - 1 - y);
        return Mathf.FloorToInt((Board.Get.boardEdgeLength - 1 - distanceFromYBound) / 2f);
    }

    public Vector2 ToWorldPosition() {
        int scale = CanvasManager.GameScale();
        int pX = x * HEX_SIZE;
        if (IsInOffsetRow) {
            pX += HEX_RADIUS;
        }
        int pY = y * INTERROW_HEIGHT;
        pX += PhantomColumns(y) * HEX_SIZE;
        return CanvasManager.TopLeft() + new Vector2(pX, -pY) * scale;
    }

    public bool IsInBounds {
        get {
            if (x < 0 || x >= Board.Get.boardDiameter || y < 0 || y >= Board.Get.boardDiameter) {
                return false;
            }
            int maxX = Board.Get.boardDiameter - (PhantomColumns(y) * 2) - (IsInOffsetRow ? 1 : 0);
            return x < maxX;
        }
    }

    public override string ToString() {
        return "(" + x + ", " + y + ")";
    }

    public HexPosition Neighbor(HexDirection direction) {
        bool UpAwayFromCenter = y < Board.Get.boardEdgeLength;
        bool DownAwayFromCenter = y >= Board.Get.boardEdgeLength - 1;
        int upLeftDeltaX = UpAwayFromCenter ? -1 : 0;
        int downLeftDeltaX = DownAwayFromCenter ? -1 : 0;
        switch (direction) {
        default:
        case HexDirection.UpLeft:    return new HexPosition(x + upLeftDeltaX, y - 1);
        case HexDirection.UpRight:   return new HexPosition(x + upLeftDeltaX + 1, y - 1);
        case HexDirection.Left:      return new HexPosition(x - 1, y - 0);
        case HexDirection.Right:     return new HexPosition(x + 1, y - 0);
        case HexDirection.DownLeft:  return new HexPosition(x + downLeftDeltaX, y + 1);
        case HexDirection.DownRight: return new HexPosition(x + downLeftDeltaX + 1, y + 1);
        }
    }

    public IEnumerable<HexPosition> Neighbors(HexDirection direction) {
        var neighbor = Neighbor(direction);
        while (neighbor.IsInBounds) {
            yield return neighbor;
            neighbor = neighbor.Neighbor(direction);
        }
    }

    public IEnumerable<HexPosition> Neighbors(HashSet<HexPosition> explored = null, int maxDistance = 1) {
        if (maxDistance <= 0) {
            yield break;
        }
        if (explored == null) {
            explored = new HashSet<HexPosition>();
        }
        Func<HexPosition, bool> Allowed = (pos) => {
            return pos.IsInBounds && explored.Add(pos);
        };
        if (maxDistance == 1) {
            HexPosition UpLeft      = Neighbor(HexDirection.UpLeft);
            HexPosition UpRight     = Neighbor(HexDirection.UpRight);
            HexPosition Left        = Neighbor(HexDirection.Left);
            HexPosition Right       = Neighbor(HexDirection.Right);
            HexPosition DownLeft    = Neighbor(HexDirection.DownLeft);
            HexPosition DownRight   = Neighbor(HexDirection.DownRight);
            if (Allowed(UpLeft))    yield return UpLeft;
            if (Allowed(UpRight))   yield return UpRight;
            if (Allowed(Left))      yield return Left;
            if (Allowed(Right))     yield return Right;
            if (Allowed(DownLeft))  yield return DownLeft;
            if (Allowed(DownRight)) yield return DownRight;
        }
        else {
            foreach (var neighbor in Neighbors(explored)) {
                yield return neighbor;
                foreach (var furtherNeighbor in neighbor.Neighbors(explored, maxDistance - 1)) {
                    yield return furtherNeighbor;
                }
            }
        }
    }

    public static IEnumerable<HexPosition> AllBoardPositions() {
        HexPosition origin = new HexPosition();
        HashSet<HexPosition> explored = new HashSet<HexPosition>();
        explored.Add(origin);
        yield return origin;
        foreach (var neighbor in origin.Neighbors(explored, int.MaxValue)) {
            yield return neighbor;
        }
    }
}
