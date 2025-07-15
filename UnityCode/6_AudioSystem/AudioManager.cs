using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource crowdSource;
    public AudioSource commentarySource;
    
    [Header("Game Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;
    
    [Header("Ball Sounds")]
    public AudioClip[] kickSounds;
    public AudioClip[] bounceSounds;
    public AudioClip[] grassSounds;
    public AudioClip postHitSound;
    public AudioClip goalpostSound;
    
    [Header("Player Sounds")]
    public AudioClip[] runSounds;
    public AudioClip[] jumpSounds;
    public AudioClip[] tackleSound;
    public AudioClip[] collisionSounds;
    
    [Header("Goal Sounds")]
    public AudioClip goalSound;
    public AudioClip goalCelebration;
    public AudioClip[] goalScoredSounds;
    
    [Header("Crowd Sounds")]
    public AudioClip crowdCheer;
    public AudioClip crowdBoo;
    public AudioClip crowdGasp;
    public AudioClip crowdSilence;
    public AudioClip[] crowdChants;
    
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    public AudioClip menuOpen;
    public AudioClip menuClose;
    
    [Header("Commentary")]
    public AudioClip[] goalCommentary;
    public AudioClip[] missCommentary;
    public AudioClip[] saveCommentary;
    public AudioClip[] kickoffCommentary;
    public AudioClip[] halftimeCommentary;
    public AudioClip[] fulltimeCommentary;
    
    [Header("Referee Sounds")]
    public AudioClip whistleStart;
    public AudioClip whistleEnd;
    public AudioClip whistleHalfTime;
    public AudioClip whistleFullTime;
    public AudioClip whistleFoul;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    [Range(0f, 1f)]
    public float crowdVolume = 0.8f;
    [Range(0f, 1f)]
    public float commentaryVolume = 0.9f;
    
    private Dictionary<string, AudioClip> soundDictionary = new Dictionary<string, AudioClip>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void InitializeAudio()
    {
        // Configurar fuentes de audio
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.volume = musicVolume * masterVolume;
        }
        
        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume * masterVolume;
        }
        
        if (crowdSource != null)
        {
            crowdSource.loop = true;
            crowdSource.volume = crowdVolume * masterVolume;
        }
        
        if (commentarySource != null)
        {
            commentarySource.loop = false;
            commentarySource.volume = commentaryVolume * masterVolume;
        }
        
        // Reproducir música de fondo
        PlayMusic(menuMusic);
        
        // Reproducir sonido de multitud
        PlayCrowdSound(crowdSilence);
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlaySFX(randomClip);
        }
    }
    
    public void PlayCrowdSound(AudioClip clip)
    {
        if (crowdSource != null && clip != null)
        {
            crowdSource.clip = clip;
            crowdSource.Play();
        }
    }
    
    public void PlayCommentary(AudioClip clip)
    {
        if (commentarySource != null && clip != null)
        {
            commentarySource.PlayOneShot(clip);
        }
    }
    
    public void PlayCommentary(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0)
        {
            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlayCommentary(randomClip);
        }
    }
    
    // Métodos específicos para eventos del juego
    public void PlayKickSound()
    {
        PlaySFX(kickSounds);
    }
    
    public void PlayBounceSound()
    {
        PlaySFX(bounceSounds);
    }
    
    public void PlayGrassSound()
    {
        PlaySFX(grassSounds);
    }
    
    public void PlayGoalSound()
    {
        PlaySFX(goalSound);
        PlayCrowdSound(crowdCheer);
        PlayCommentary(goalCommentary);
    }
    
    public void PlayMissSound()
    {
        PlaySFX(crowdGasp);
        PlayCommentary(missCommentary);
    }
    
    public void PlaySaveSound()
    {
        PlaySFX(crowdGasp);
        PlayCommentary(saveCommentary);
    }
    
    public void PlayWhistleSound(WhistleType type)
    {
        switch (type)
        {
            case WhistleType.Start:
                PlaySFX(whistleStart);
                break;
            case WhistleType.End:
                PlaySFX(whistleEnd);
                break;
            case WhistleType.HalfTime:
                PlaySFX(whistleHalfTime);
                PlayCommentary(halftimeCommentary);
                break;
            case WhistleType.FullTime:
                PlaySFX(whistleFullTime);
                PlayCommentary(fulltimeCommentary);
                break;
            case WhistleType.Foul:
                PlaySFX(whistleFoul);
                break;
        }
    }
    
    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }
    
    public void PlayVictoryMusic()
    {
        PlayMusic(victoryMusic);
        PlayCrowdSound(crowdCheer);
    }
    
    public void PlayDefeatMusic()
    {
        PlayMusic(defeatMusic);
        PlayCrowdSound(crowdBoo);
    }
    
    public void PlayButtonSound()
    {
        PlaySFX(buttonClick);
    }
    
    public void PlayHoverSound()
    {
        PlaySFX(buttonHover);
    }
    
    // Métodos para controlar volumen
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
    }
    
    public void SetCrowdVolume(float volume)
    {
        crowdVolume = Mathf.Clamp01(volume);
        if (crowdSource != null)
            crowdSource.volume = crowdVolume * masterVolume;
    }
    
    public void SetCommentaryVolume(float volume)
    {
        commentaryVolume = Mathf.Clamp01(volume);
        if (commentarySource != null)
            commentarySource.volume = commentaryVolume * masterVolume;
    }
    
    void UpdateAllVolumes()
    {
        if (musicSource != null)
            musicSource.volume = musicVolume * masterVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume * masterVolume;
        if (crowdSource != null)
            crowdSource.volume = crowdVolume * masterVolume;
        if (commentarySource != null)
            commentarySource.volume = commentaryVolume * masterVolume;
    }
    
    // Método para reaccionar a eventos del juego
    public void OnGameEvent(GameEvent gameEvent)
    {
        switch (gameEvent)
        {
            case GameEvent.GameStart:
                PlayGameplayMusic();
                PlayWhistleSound(WhistleType.Start);
                PlayCommentary(kickoffCommentary);
                break;
                
            case GameEvent.Goal:
                PlayGoalSound();
                break;
                
            case GameEvent.Miss:
                PlayMissSound();
                break;
                
            case GameEvent.Save:
                PlaySaveSound();
                break;
                
            case GameEvent.HalfTime:
                PlayWhistleSound(WhistleType.HalfTime);
                break;
                
            case GameEvent.FullTime:
                PlayWhistleSound(WhistleType.FullTime);
                break;
                
            case GameEvent.Victory:
                PlayVictoryMusic();
                break;
                
            case GameEvent.Defeat:
                PlayDefeatMusic();
                break;
        }
    }
    
    // Método para generar sonidos dinámicos de multitud
    public void UpdateCrowdIntensity(float intensity)
    {
        if (crowdSource != null)
        {
            crowdSource.volume = (crowdVolume * masterVolume) * intensity;
            crowdSource.pitch = 0.8f + (intensity * 0.4f);
        }
    }
    
    // Método para pausar/reanudar audio
    public void PauseAll()
    {
        if (musicSource != null) musicSource.Pause();
        if (sfxSource != null) sfxSource.Pause();
        if (crowdSource != null) crowdSource.Pause();
        if (commentarySource != null) commentarySource.Pause();
    }
    
    public void ResumeAll()
    {
        if (musicSource != null) musicSource.UnPause();
        if (sfxSource != null) sfxSource.UnPause();
        if (crowdSource != null) crowdSource.UnPause();
        if (commentarySource != null) commentarySource.UnPause();
    }
    
    // Método para mutear/desmutear
    public void MuteAll(bool mute)
    {
        if (musicSource != null) musicSource.mute = mute;
        if (sfxSource != null) sfxSource.mute = mute;
        if (crowdSource != null) crowdSource.mute = mute;
        if (commentarySource != null) commentarySource.mute = mute;
    }
}

public enum WhistleType
{
    Start,
    End,
    HalfTime,
    FullTime,
    Foul
}

public enum GameEvent
{
    GameStart,
    Goal,
    Miss,
    Save,
    HalfTime,
    FullTime,
    Victory,
    Defeat
}