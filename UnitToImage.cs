using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitToImage : MonoBehaviour
{
    protected static UnitToImage Instance;
    public static UnitToImage Get {
        get {
            return Instance;
        }
    }

    public void Awake() {
        Instance = this;
    }

    public Sprite Arrow;
    public Sprite Bomb;
    public Sprite Empty;
    public Sprite Fang;
    public Sprite Fire;
    public Sprite Shield;
    public Sprite Sword;
    public Sprite this[TileState key] {
        get {
            switch (key.unit) {
            case UnitType.Arrow:
                return Arrow;
            case UnitType.Bomb:
                return Bomb;
            case UnitType.Empty:
                return Empty;
            case UnitType.Fang:
                return Fang;
            case UnitType.Fire:
                return Fire;
            case UnitType.Shield:
                return Shield;
            case UnitType.Sword:
                return Sword;
            case UnitType.Hole:
                return Empty;
            default:
                throw new System.Exception("No sprite for unit type: " + key);
            }
        }
    }
}
