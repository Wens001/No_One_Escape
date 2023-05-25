using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System;
public class AudioManager : Singleton<AudioManager>
{
    private Dictionary<int, string> audioPathDict;      // 存放音频文件路径
    private AudioSource musicAudioSource;
    private List<AudioSource> unusedSoundAudioSourceList;   // 存放可以使用的音频组件
    private List<AudioSource> usedSoundAudioSourceList;     // 存放正在使用的音频组件
    private Dictionary<int, AudioClip> audioClipDict;       // 缓存音频文件
    private float musicVolume = 1;
    private float soundVolume = 1;
    private int poolCount = 3;         // 对象池数量
    void Awake()
    {
        audioPathDict = new Dictionary<int, string>()       // 这里设置音频文件路径,在Resources目录下。 TODO
        {
            { 1, "AudioClip/开关" },
            { 2, "AudioClip/大门打开" },
            { 3, "AudioClip/斧头挥击"},
            { 4, "AudioClip/砍到人"},
            { 5, "AudioClip/变身喊叫"},
            { 6, "AudioClip/变身效果"},
            { 7, "AudioClip/感叹号"},
            { 8, "AudioClip/击破防暴盾"},
            { 9, "AudioClip/猎枪击发"},
            { 10, "AudioClip/猎枪上膛"},
            { 11, "AudioClip/电锯击杀到人"},
            { 12, "AudioClip/电锯击杀盾牌"},
            { 101, "AudioClip/BGM/H_Normal"},
            { 102, "AudioClip/BGM/H_High"},
            { 103, "AudioClip/BGM/H_ToNormal"},
            { 104, "AudioClip/BGM/Z_Normal"},
        };
        musicAudioSource = gameObject.AddComponent<AudioSource>();
        unusedSoundAudioSourceList = new List<AudioSource>();
        usedSoundAudioSourceList = new List<AudioSource>();
        audioClipDict = new Dictionary<int, AudioClip>();
    }
    void Start()
    {

    }
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="id"></param>
    /// <param name="loop"></param>
    public void PlayMusic(int id, bool loop = true)
    {
        // 通过Tween将声音淡入淡出
        DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value, 0, 0.1f).OnComplete(() =>
        {
            musicAudioSource.clip = GetAudioClip(id);
            musicAudioSource.clip.LoadAudioData();
            musicAudioSource.loop = loop;
            musicAudioSource.volume = musicVolume;
            musicAudioSource.Play();
            DOTween.To(() => musicAudioSource.volume, value => musicAudioSource.volume = value, musicVolume, 0.1f);
        });
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="id"></param>
    public void PlaySound(int id, Action action = null)
    {
        if (unusedSoundAudioSourceList.Count != 0)
        {
            AudioSource audioSource = UnusedToUsed();
            audioSource.clip = GetAudioClip(id);
            audioSource.clip.LoadAudioData();
            audioSource.Play();
            StartCoroutine(WaitPlayEnd(audioSource, action));
        }
        else
        {
            AddAudioSource();
            AudioSource audioSource = UnusedToUsed();
            audioSource.clip = GetAudioClip(id);
            audioSource.clip.LoadAudioData();
            audioSource.volume = soundVolume;
            audioSource.loop = false;
            audioSource.Play();
            StartCoroutine(WaitPlayEnd(audioSource, action));
        }
    }
    /// <summary>
    /// 播放3d音效
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    public void Play3dSound(int id, Vector3 position)
    {
        AudioClip ac = GetAudioClip(id);
        AudioSource.PlayClipAtPoint(ac, position);
    }
    /// <summary>
    /// 当播放音效结束后，将其移至未使用集合
    /// </summary>
    /// <param name="audioSource"></param>
    /// <returns></returns>
    IEnumerator WaitPlayEnd(AudioSource audioSource, Action action)
    {
        yield return new WaitUntil(() => { return !audioSource.isPlaying; });
        UsedToUnused(audioSource);
        if (action != null)
        {
            action();
        }
    }
    /// <summary>
    /// 获取音频文件，获取后会缓存一份
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private AudioClip GetAudioClip(int id)
    {
        if (!audioClipDict.ContainsKey(id))
        {
            if (!audioPathDict.ContainsKey(id))
                return null;
            AudioClip ac = Resources.Load(audioPathDict[id]) as AudioClip;
            audioClipDict.Add(id, ac);
        }
        return audioClipDict[id];
    }
    /// <summary>
    /// 添加音频组件
    /// </summary>
    /// <returns></returns>
    private AudioSource AddAudioSource()
    {
        if (unusedSoundAudioSourceList.Count != 0)
        {
            return UnusedToUsed();
        }
        else
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            unusedSoundAudioSourceList.Add(audioSource);
            return audioSource;
        }
    }
    /// <summary>
    /// 将未使用的音频组件移至已使用集合里
    /// </summary>
    /// <returns></returns>
    private AudioSource UnusedToUsed()
    {
        AudioSource audioSource = unusedSoundAudioSourceList[0];
        unusedSoundAudioSourceList.RemoveAt(0);
        usedSoundAudioSourceList.Add(audioSource);
        return audioSource;
    }
    /// <summary>
    /// 将使用完的音频组件移至未使用集合里
    /// </summary>
    /// <param name="audioSource"></param>
    private void UsedToUnused(AudioSource audioSource)
    {
        if (usedSoundAudioSourceList.Contains(audioSource))
        {
            usedSoundAudioSourceList.Remove(audioSource);
        }
        if (unusedSoundAudioSourceList.Count >= poolCount)
        {
            Destroy(audioSource);
        }
        else if (audioSource != null && !unusedSoundAudioSourceList.Contains(audioSource))
        {
            unusedSoundAudioSourceList.Add(audioSource);
        }
    }
    /// <summary>
    /// 修改背景音乐音量
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeMusicVolume(float volume)
    {
        musicVolume = volume;
        musicAudioSource.volume = volume;
    }
    /// <summary>
    /// 修改音效音量
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeSoundVolume(float volume)
    {
        soundVolume = volume;
        for (int i = 0; i < unusedSoundAudioSourceList.Count; i++)
        {
            unusedSoundAudioSourceList[i].volume = volume;
        }
        for (int i = 0; i < usedSoundAudioSourceList.Count; i++)
        {
            usedSoundAudioSourceList[i].volume = volume;
        }
    }
}
/*	例子
	void Start () {
        AudioManager.Instance.PlayMusic( 1 );
        AudioManager.Instance.PlaySound( 11, OnSoundPlayEnd );
        AudioManager.Instance.ChangeMusicVolume( 0.5f );
        AudioManager.Instance.ChangeSoundVolume( 0.5f );
    }
    void OnSoundPlayEnd()
    {
        Debug.Log( "音效播放完了" );
    }
*/