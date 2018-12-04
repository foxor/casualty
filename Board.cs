using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    private static Board Instance;
    public static Board Get {
        get {
            if (Instance == null) {
                Instance = new Board();
            }
            return Instance;
        }
    }

    public static event Action OnReset = () => {};

    protected Dictionary<HexPosition, TileState> States = new Dictionary<HexPosition, TileState>();

    public TileState this[HexPosition index] {
        get {
            if (!States.ContainsKey(index)) {
                States[index] = new TileState(index);
            }
            return States[index];
        }
        set {
            States[index] = value;
        }
    }

    public TileState this[int x, int y] {
        get {
            HexPosition hp = new HexPosition(x, y);
            return this[hp];
        }
        set {
            HexPosition hp = new HexPosition(x, y);
            this[hp] = value;
        }
    }
    public int boardEdgeLength = 4;
    public int boardDiameter {
        get {
            return boardEdgeLength * 2 - 1;
        }
    }

    public void Reset(int newEdgeLength) {
        boardEdgeLength = newEdgeLength;
        States.Clear();
        OnReset();
    }

    public static TileState ToState(HexPosition hexPosition) {
        return Instance[hexPosition];
    }
}
