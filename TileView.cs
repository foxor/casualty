using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [SerializeField]
    protected HexPosition HexPosition;

    public static event Action<HexPosition> OnClicked = (_) => {};

    public Image UnitImage;
    public Image Background;
    public Button Button;

    public Sprite FriendlyIdle;
    public Sprite FriendlyPressed;

    public void Start() {
        CanvasManager.OnScreenResize += UpdateSize;
        Selector.OnSelectionMade += ForceUpdateContents;
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.01f;
    }

    public void OnDestroy() {
        CanvasManager.OnScreenResize -= UpdateSize;
        Selector.OnSelectionMade -= ForceUpdateContents;
    }

    protected void UpdateSize() {
        int scale = CanvasManager.GameScale();
        var RectTransform = GetComponent<RectTransform>();
        RectTransform.localPosition = HexPosition.ToWorldPosition();
        RectTransform.localScale = new Vector3(scale, scale, 1f);
    }
    
    public void Move(HexPosition newPosition) {
        Board.Get[HexPosition].OnChanged -= UpdateContents;
        HexPosition = newPosition;
        Board.Get[HexPosition].OnChanged += UpdateContents;
        gameObject.name = "Tile " + newPosition;
        UpdateSize();
        ForceUpdateContents();
    }

    protected void ForceUpdateContents() {
        UpdateContents(Board.Get[HexPosition]);
    }
    public void UpdateContents(TileState state) {
        UnitImage.sprite = UnitToImage.Get[state];
        Background.sprite = TileStateToBackground.Get[state];
        var spriteState = Button.spriteState;
        if (Background.sprite == FriendlyIdle) {
            spriteState.pressedSprite = FriendlyPressed;
        }
        else {
            spriteState.pressedSprite = null;
        }
        Button.spriteState = spriteState;
    }

    public void ClickProxy() {
        OnClicked(HexPosition);
    }
}
