using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    protected static TileController Instance;
    public static TileController Get {
        get {
            return Instance;
        }
    }

    public GameObject tilePrefab;
    public CanvasGroup tileParent;

    public void Awake() {
        Instance = this;
        Board.OnReset += ResetViews;
    }

    protected void ResetViews() {
        if (!Generator.isRunning) {
            foreach (var tileView in GameObject.FindObjectsOfType<TileView>()) {
                GameObject.Destroy(tileView.gameObject);
            }
            foreach (var hexPosition in HexPosition.AllBoardPositions()) {
                GameObject tile = (GameObject)GameObject.Instantiate(tilePrefab, tileParent.transform);
                tile.GetComponent<TileView>().Move(hexPosition);
            }
        }
    }

    public static void SetAllowClick(bool shouldBeAllowed) {
        Instance.tileParent.blocksRaycasts = shouldBeAllowed;
    }

    public static bool AreClicksAllowed() {
        return Instance.tileParent.blocksRaycasts;
    }
}
