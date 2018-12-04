using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour
{
    public CanvasGroup TitleScreen;

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Screen.fullScreen = true;
        }
        if (Input.GetMouseButtonUp(0)) {
            if (TitleScreen.alpha > 0f) {
                TitleScreen.alpha = 0f;
                LevelController.Get.StartLevel(0);
            }
        }
    }
}
