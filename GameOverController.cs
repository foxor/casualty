using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverController : MonoBehaviour
{
    protected static GameOverController Instance;

    public CanvasGroup canvasGroup;
    public float fadeTime = 1.3f;
    public float pauseTime = 0.8f;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Quote;
    public List<string> loseQuotesMultiDeath = new List<string>();
    public List<string> loseQuotesEnemySurvived = new List<string>();
    public List<string> loseQuotesNoDeaths = new List<string>();
    public List<string> winQuotes = new List<string>();
    protected int loseMultiDeathIndex;
    protected int loseEnemySurvivedIndex;
    protected int loseNoDeathsIndex;
    protected int winIndex;

    public void Awake() {
        Instance = this;
    }

    protected IEnumerator Show(string title, List<string> quoteSource, int index) {
        Title.text = title;
        index %= quoteSource.Count;
        Quote.text = quoteSource[index].Replace("\\n", "\n");
        float elapsed = 0f;
        while (elapsed < fadeTime) {
            canvasGroup.alpha = elapsed / fadeTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        elapsed = fadeTime;
        yield return new WaitForSeconds(pauseTime);
        while (elapsed > 0f) {
            canvasGroup.alpha = elapsed / fadeTime;
            elapsed -= Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public static IEnumerator LoseMultiDeath() {
        return Instance.Show("You Lose", Instance.loseQuotesMultiDeath, Instance.loseMultiDeathIndex++);
    }

    public static IEnumerator LoseEnemySurvived() {
        return Instance.Show("You Lose", Instance.loseQuotesEnemySurvived, Instance.loseEnemySurvivedIndex++);
    }

    public static IEnumerator LoseNoDeaths() {
        return Instance.Show("You Lose", Instance.loseQuotesNoDeaths, Instance.loseNoDeathsIndex++);
    }

    public static IEnumerator Win() {
        return Instance.Show("You Win", Instance.winQuotes, Instance.winIndex++);
    }

    public static IEnumerator GameOver() {
        Instance.Title.text = "\nThanks for playing!";
        Instance.Quote.text = "Credits: Isaac James";
        float elapsed = 0f;
        while (elapsed < Instance.fadeTime) {
            Instance.canvasGroup.alpha = elapsed / Instance.fadeTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        Instance.canvasGroup.alpha = 1f;
        elapsed = Instance.fadeTime;
    }
}
