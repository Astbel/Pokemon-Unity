using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
public class AudioManager : MonoBehaviour
{


    [SerializeField] List<AudioData> sfxList;
    [SerializeField] AudioSource msuicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] float fadeDuration = 0.75f;
    AudioClip currMusic;
    float originalMusicVol;
    Dictionary<AudioID, AudioData> sfxLookup;
    public static AudioManager i { get; private set; }

    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        originalMusicVol = msuicPlayer.volume;
        sfxLookup = sfxList.ToDictionary(x => x.id);
    }

    public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;
        if (pauseMusic)
        {
            msuicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }

        sfxPlayer.PlayOneShot(clip);
    }

    public void PlaySfx(AudioID audioID, bool pauseMusic = false)
    {
        if (!sfxLookup.ContainsKey(audioID)) return;

        var audioData = sfxLookup[audioID];

        PlaySfx(audioData.clip, pauseMusic);
    }

    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    {
        //檢查當前BGM是否跟原本一樣如果一樣則不重新撥放
        if (clip == null||clip==currMusic) return;

        currMusic=clip;
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
    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        msuicPlayer.volume = 0;
        msuicPlayer.UnPause();
        msuicPlayer.DOFade(originalMusicVol, fadeDuration);

    }
}


public enum AudioID
{
    UISelect,
    Hit,
    Faint,
    ExpGain,
    ItemObtained,
    PokemonObtained
}
[System.Serializable]
public class AudioData
{
    public AudioID id;
    public AudioClip clip;
}





