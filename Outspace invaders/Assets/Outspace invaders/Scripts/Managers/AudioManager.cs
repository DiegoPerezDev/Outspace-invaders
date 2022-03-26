using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

/// <summary>
/// <para> This class manage all the audio playing in the game, for both music and sfx, for all scenes. </para>
/// <para> All codes should use audio by using the methods of this class, not playing audio in their own. </para>
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioSource musicSource, UI_AudioSource;
    public static AudioSource[] GameAudioSource;
    private static AudioClip[] songsClips = new AudioClip[Enum.GetValues(typeof(Songs)).Length];
    public static AudioMixer musicMixer, generalSFX_Mixer, UI_SFX_Mixer;
    public static GameObject GameAudioGO;
    public GameObject musicSourceGO, GameAudioSourceGO, UI_AudioSourceGO;
    public enum Songs { mainmenu, level }


    private void OnEnable()
    {
        GameManager.OnStartingScene += StartingScene;
        GameManager.OnLevelStarted += LevelStart;
        GameManager.OnLoseGame += LoseLevel;
    }
    private void OnDisable()
    {
        GameManager.OnStartingScene -= StartingScene;
        GameManager.OnLevelStarted -= LevelStart;
        GameManager.OnLoseGame -= LoseLevel;
    }


    #region Delegate functions

    private void StartingScene()
    {
        // Find resources
        songsClips[(int)Songs.mainmenu] = Resources.Load<AudioClip>("Audio/Music/LoopMenu");
        songsClips[(int)Songs.level] = Resources.Load<AudioClip>("Audio/Music/push_ahead_0");
        musicMixer = Resources.Load<AudioMixer>("Audio/MusicMixer");
        generalSFX_Mixer = Resources.Load<AudioMixer>("Audio/GeneralSFX_Mixer");
        UI_SFX_Mixer = Resources.Load<AudioMixer>("Audio/UI_SFX_Mixer");

        // Find components
        GameAudioGO = GameAudioSourceGO;
        GameAudioSource = GameAudioSourceGO.GetComponents<AudioSource>();
        UI_AudioSource = UI_AudioSourceGO.GetComponent<AudioSource>();
        musicSource = musicSourceGO.GetComponent<AudioSource>();

        // play song
        PlayLevelSong(Songs.mainmenu);
    }
    private void LevelStart() => PlayLevelSong(Songs.level);
    private void LoseLevel() => StopLevelSong();
        //PlayAudio(UI_AudioSource, UI_MenusManagement.UI_Clips[(int)UI_MenusManagement.UI_AudioNames.lose]);
    

    #endregion

    #region SFX methods

    /// <summary>
    /// Try to play a certain sfx in an specific audio source array. If there is another clip playing in one of this sources then go to the next source of the array to play the next clip. If all sources are playing then add a new one to the gameobject so it never interrupt a source that is playing at the time.
    /// </summary>
    public static void PlayAudio(ref AudioSource[] sources, AudioClip clip, GameObject objectOfSources)
    {
        if (!CheckSource(sources[0]) || !CheckClip(clip))
            return;

        AudioSource source = null;
        var sourcesLength = sources.Length;

        for (int i = 0; i < sourcesLength; i++)
        {
            if (i == sourcesLength - 1)
            {
                //make another source if all the sources of the array are playing a sfx
                if (sources[i].isPlaying)
                {
                    Array.Resize(ref sources, sources.Length + 1);
                    source = sources[i + 1] = objectOfSources.AddComponent<AudioSource>();
                    goto Play;
                }
            }
            else
            {
                // Dont play the new sfx on an audiosource that is already playing, continue checking next array slot
                if (sources[i].isPlaying)
                    continue;
            }
            source = sources[i];
        }
    Play:
        PlayAudio(source, clip);
    }

    /// <summary>
    /// <para>Try to play an certain sfx in a specific audio source.</para> 
    /// <para>If there is another clip playing in this source then stop it and change it to the new one to play.</para>
    /// <para>If you don't want to interrupt any sfx then use the overload of this method that uses 'audioSource[]' instead.</para>
    /// </summary>
    public static void PlayAudio(AudioSource source, AudioClip clip)
    {
        if (!CheckSource(source) || (!CheckClip(clip)))
            return;
        StopAudio(source);
        source.clip = clip;
        source.Play();
    }

    /// <summary>
    /// <para>Try to play whichever clip is attached in an specific audio source.</para> 
    /// <para>If you want to play an specific clio in an audioSource, use other overrided method instead.</para>
    /// </summary>
    public static void PlayAudio(AudioSource source)
    {
        if (!CheckSource(source))
            return;
        StopAudio(source);
        source.Play();
    }

    /// <summary> Stops any audio coming from an specific audio source. </summary>
    public static void StopAudio(AudioSource source)
    {
        if (!CheckSource(source))
            return;
        source.loop = false;
        source.Stop();
    }

    /// <summary> Stop an specific audio coming from an also specified audio source.</summary>
    public static void StopAudio(AudioSource source, AudioClip clip)
    {
        if (!CheckSource(source) || (!CheckClip(clip)))
            return;
        if (source.clip == clip)
        {
            source.loop = false;
            source.Stop();
        }
    }

    #endregion

    #region Music methods

    /// <summary> Plays the song of the level just entered in the music audio source. </summary>
    public static void PlayLevelSong(Songs song)
    {
        // Look if we have the clip
        if (!CheckClip(songsClips[(int)song]))
            return;

        // Set desired volume for each music
        if (song == Songs.mainmenu)
            musicSource.volume = 0.8f;
        else if (song == Songs.level)
            musicSource.volume = 0.4f;

        // Play level song
        musicSource.clip = songsClips[(int)song];
        musicSource.Play();
    }

    /// <summary> Stops the music audio source. </summary>
    public static void StopLevelSong()
    {
        if (musicSource == null)
            return;
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    #endregion

    #region Tools for this class functions

    /// <summary> Checks if the source is null or not. </summary>
    private static bool CheckSource(AudioSource source)
    {
        if (source == null)
        {
            print($"No audio source found with name {nameof(source)}");
            return false;
        }
        else
            return true;
    }

    private static bool CheckClip(AudioClip clip)
    {
        if (clip == null)
        {
            print($"No audio clip found with name {nameof(clip)}");
            return false;
        }
        else
            return true;
    }

    #endregion

}