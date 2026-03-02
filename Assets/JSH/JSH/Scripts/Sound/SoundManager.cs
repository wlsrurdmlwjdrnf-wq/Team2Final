using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource mBGMSource;
    [SerializeField] private AudioSource mSFXSourcePrefab;
    [SerializeField] private SoundDatabaseSO mDatabase;

    private List<AudioSource> mSFXPool = new List<AudioSource>();
    private WaitForSeconds mWaitForSeconds = new WaitForSeconds(0.5f);
    private AudioSource mLoopSource;

    [Header("Event Channel")]
    [SerializeField] private GameEventChannelSO mEventChannel;

    private float mBGMVolume = 1f;
    private float mSFXVolume = 1f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitPool();
            mDatabase.Init();

            mLoopSource = gameObject.AddComponent<AudioSource>();
            mLoopSource.loop = true;
            mLoopSource.playOnAwake = false;
            //볼륨저장된거 불러오기
            mBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
            mSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

            mBGMSource.volume = mBGMVolume;
            foreach (var src in mSFXPool) src.volume = mSFXVolume;
            mLoopSource.volume = mSFXVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        mEventChannel.OnEventRaised += HandleGameEvent;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        mEventChannel.OnEventRaised -= HandleGameEvent;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void InitPool() 
    {
        for (int i = 0; i < 10; i++) 
        {
            AudioSource source = Instantiate(mSFXSourcePrefab, transform);
            mSFXPool.Add(source);
        }
    }
    private AudioSource GetSource() 
    {
        foreach (var src in mSFXPool) 
        {
            if (!src.isPlaying) return src;
        }
        return mSFXPool[0];
    }
    public void PlayBGM(EBGMType type) 
    {
        AudioClip clip = mDatabase.GetBGM(type);
        if (clip != null) 
        {
            StartCoroutine(FadeInBGM(clip));
        }      
    }
    public void PlaySFX(ESFXType type) 
    {
        AudioClip clip = mDatabase.GetSFX(type);
        if (clip != null)
        {
            AudioSource src = GetSource();
            src.clip = clip;
            src.volume = mSFXVolume;
            src.Play();
        }
    }
    public void StopSFX(ESFXType type) 
    {
        foreach (var src in mSFXPool) 
        {
            if (src.isPlaying && src.clip == mDatabase.GetSFX(type)) 
            {
                src.Stop();
            }
        }
    }
    private IEnumerator FadeInBGM(AudioClip clip) 
    {
        //지금 플레이하는거 볼륨 점점 줄이고
        if (mBGMSource.isPlaying) 
        {
            for (float v = mBGMVolume; v >= 0; v -= 0.05f) 
            {
                mBGMSource.volume = v;
                yield return mWaitForSeconds;
            }
        }
        //다 줄이면 새로운거
        mBGMSource.clip = clip;
        mBGMSource.Play();
        for (float v = 0; v <= mBGMVolume; v += 0.05f) 
        {
            mBGMSource.volume = v;
            yield return mWaitForSeconds;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        switch (scene.name) 
        {
            case "BattleScene":
                PlayBGM(EBGMType.Battle);
                break;
        }
    }
    private void HandleGameEvent(EGameEventType type, object payload) 
    {
        switch (type) 
        {
            case EGameEventType.SkillUsed:
                break;
            case EGameEventType.DamageDealt:
                PlaySFX(ESFXType.Hit);
                break;
            case EGameEventType.ButtonClicked:
                PlaySFX(ESFXType.Button);
                break;
            case EGameEventType.VolumeBGMUpdate:
                break;
            case EGameEventType.VolumeSFXUpdate:
                break;
            case EGameEventType.VolumeBGMMuteToggle:
                break;
            case EGameEventType.VolumeSFXMuteToggle:
                break;
        }
    }
    public float GetBGMVolume() => mBGMVolume;
    public float GetSFXVolume() => mSFXVolume;
}
