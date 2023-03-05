using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
/*切換場景的動畫*/
public class Fader : MonoBehaviour
{
    public static Fader i { get; private set; }
    Image image;

    private void Awake()
    {
        i = this;
        image = GetComponent<Image>();

    }

    public IEnumerator FaderIn(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FaderOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }

}
