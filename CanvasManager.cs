using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public float Margin = 1f;

    private static CanvasManager Instance;

    protected RectTransform canvasTransform;

    protected Vector2 lastCanvasSize;

    public static event Action OnScreenResize = () => {};

    public void Awake() {
        Instance = this;
        canvasTransform = GetComponent<RectTransform>();
        lastCanvasSize = canvasTransform.sizeDelta;
    }

    // Size up the game as allowable by screen size
    public static int GameScale() {
        int totalWidth = Board.Get.boardDiameter * HexPosition.HEX_SIZE;
        int totalHeight = (Board.Get.boardDiameter - 1) * HexPosition.INTERROW_HEIGHT + HexPosition.HEX_SIZE;
        float boardAspect = totalWidth * 1f / totalHeight;
        var sizeDelta = Instance.canvasTransform.sizeDelta;
        Vector2 availableScreenSpace = new Vector2(sizeDelta.x - Instance.Margin * 2f, sizeDelta.y - Instance.Margin * 2f);
        float screenAspect = availableScreenSpace.x / availableScreenSpace.y;
        float aspectDelta = screenAspect - boardAspect;
        int boundedBoardDimension;
        float boundedScreenDimension;
        if (aspectDelta < 0) { // width bound
            boundedBoardDimension = totalWidth;
            boundedScreenDimension = availableScreenSpace.x;
        }
        else { // height bound
            boundedBoardDimension = totalHeight;
            boundedScreenDimension = availableScreenSpace.y;
        }
        return Mathf.FloorToInt(boundedScreenDimension / boundedBoardDimension);
    }

    // Move TopLeft to keep the game board centered
    public static Vector2 TopLeft() {
        int scale = GameScale();
        int totalWidth = Board.Get.boardDiameter * HexPosition.HEX_SIZE;
        int totalHeight = (Board.Get.boardDiameter - 1) * HexPosition.INTERROW_HEIGHT + HexPosition.HEX_SIZE;
        totalWidth *= scale;
        totalHeight *= scale;
        var sizeDelta = Instance.canvasTransform.sizeDelta;
        Vector2 leftOver = new Vector2(sizeDelta.x - totalWidth, sizeDelta.y - totalHeight);
        return new Vector2(leftOver.x / 2f - sizeDelta.x / 2f, sizeDelta.y / 2f - (leftOver.y / 2f));
    }

    public void Update() {
        if (lastCanvasSize != canvasTransform.sizeDelta) {
            OnScreenResize();
        }
        lastCanvasSize = canvasTransform.sizeDelta;
    }
}
