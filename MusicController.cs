using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource source;
    void Start() {
        StartCoroutine(RestartMusic());
    }

    protected IEnumerator RestartMusic() {
        while (true) {
            yield return new WaitForSeconds(source.clip.length - 4f);
            source.Play();
        }
    }
}
