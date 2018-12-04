using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeManager : MonoBehaviour
{
    protected static ScreenShakeManager Instance;
    public static ScreenShakeManager Get {
        get {
            return Instance;
        }
    }

    public CanvasGroup board;
    public float traumaDecay;
    public float extremeTrauma;
    public float minorTrauma;
    public float perlinScrollSpeed;
    public float maxDisplacement;

    protected float trauma;
    protected Vector2 perlinPosition;
    protected Vector2 perlinVelocity;
    protected Vector2 sampleDelta;

    public void Awake() {
        Instance = this;
        perlinVelocity = Random.insideUnitCircle * perlinScrollSpeed;
        perlinPosition = Random.insideUnitCircle * 1000f;
        // this is perpendicular to the direction of velocity, so that we don't double sample points over time.
        sampleDelta = new Vector2(-perlinVelocity.y, perlinVelocity.x) * 400f;
    }

    protected float takeSample(Vector2 position) {
        return (Mathf.PerlinNoise(position.x, position.y) - 0.5f) * 2f;
    }

    void Update() {
        perlinPosition += perlinVelocity * Time.deltaTime;
        Vector2 sample = new Vector2(takeSample(perlinPosition), takeSample(perlinPosition + sampleDelta));
        sample *= Mathf.Sqrt(trauma) * maxDisplacement;
        board.transform.localPosition = sample;
        trauma = Mathf.Max(0f, trauma - traumaDecay * Time.deltaTime);
    }

    public static void MajorTrauma() {
        Instance.trauma += Instance.extremeTrauma;
    }

    public static void MinorTrauma() {
        Instance.trauma += Instance.minorTrauma;
    }
}
