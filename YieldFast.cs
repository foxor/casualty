using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YieldFast : MonoBehaviour
{
    protected static Coroutine oldCoroutine;

    public static Coroutine Get {
        get {
            return oldCoroutine;
        }
    }

    void Start() {
        oldCoroutine = StartCoroutine(EarlyReturn());
    }

    protected IEnumerator EarlyReturn() {
        yield return null;
    }
}
