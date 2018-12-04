using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    protected static SFXController Instance;

    public AudioClip Explosion;
    public AudioClip Fire;
    public AudioClip Move;

    public void Awake() {
        Instance = this;
    }

    protected void PlaySound(AudioClip clip) {
        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = clip;
        newSource.Play();
        GameObject.Destroy(newSource, clip.length + 0.5f);
    }

    public static void PlayExplosion() {
        Instance.PlaySound(Instance.Explosion);
    }

    public static void PlayFire() {
        Instance.PlaySound(Instance.Fire);
    }

    public static void PlayMove() {
        Instance.PlaySound(Instance.Move);
    }
}
