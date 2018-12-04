using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBlocker : MonoBehaviour
{
    public static event System.Action OnClicked = () => {};

    public void ClickProxy() {
        OnClicked();
    }
}
