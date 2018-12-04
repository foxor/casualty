using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatController : MonoBehaviour
{
    void Update() {
        if (Input.GetKeyDown(KeyCode.W)) {
            StartCoroutine(LevelController.Get.WinLevel());
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            StartCoroutine(LevelController.Get.LoseLevel());
        } 
    }
}
