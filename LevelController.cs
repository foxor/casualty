using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    protected static LevelController Instance;
    public static LevelController Get {
        get {
            return Instance;
        }
    }
    public Level[] Levels;
    protected int levelIndex;

    public void Awake() {
        Instance = this;
    }

    public void StartLevel(int index) {
        Levels[index].Setup();
        levelIndex = index;
    }

    public IEnumerator WinLevel() {
        if (!Generator.isRunning) {
            yield return new WaitForSeconds(0.5f);
            // show you win text
        }
        if (levelIndex >= Levels.Length - 1) {
            yield return GameOverController.GameOver();
        }
        else {
            StartLevel(levelIndex + 1);
        }
        yield return YieldFast.Get;
    }

    public IEnumerator LoseLevel() {
        if (!Generator.isRunning) {
            yield return new WaitForSeconds(0.5f);
            // show you lose text
        }
        StartLevel(levelIndex);
        yield return YieldFast.Get;
    }
}
