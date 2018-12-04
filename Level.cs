using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnitPlacement {
    public HexPosition position;
    public UnitType unit;
    public bool startsAwake;
}

[CreateAssetMenu]
public class Level : ScriptableObject {
    public UnitPlacement[] placements;
    public int boardEdgeLength;
    public string notes;

    public void Setup() {
        Board.Get.Reset(boardEdgeLength);
        int placeOrder = 0;
        foreach (var placement in placements) {
            Board.Get[placement.position].unit = placement.unit;
            Board.Get[placement.position].alerted = placement.startsAwake;
            Board.Get[placement.position].playOrder = placeOrder++;
        }
    }
}
