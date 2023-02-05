using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource msuicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] float fadeDuration = 0.75f;
    float originalMusicVol;
    public static AudioManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        originalMusicVol = msuicPlayer.volume;
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    {
        if (clip == null) return;

        StartCoroutine(PlayMusicAsync(clip, loop, fade));
    }

    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        /*等待音樂淡出*/
        if (fade)
            yield return msuicPlayer.DOFade(0, fadeDuration).WaitForCompletion();

        msuicPlayer.clip = clip;
        msuicPlayer.loop = loop;
        msuicPlayer.Play();
        if (fade)
            yield return msuicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();

    }

}
