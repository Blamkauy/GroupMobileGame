using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    public GameDifficulty difficulty;
    public GameControlScheme controlScheme;
    [SerializeField] AudioClip[] allAudio;
    public Dictionary<string, AudioClip> loadedAudio;
    List<AudioSource> audioChannels;
    public static void PlayAudio(string audioName,int channel=0,float volume=1f,float pitch=1f,float panning=0f,bool ignoreListenerEffects = false)
    {
        if (channel < 0) { Debug.LogError("Channel cannot be negative."); return; }
        while(channel>=main.audioChannels.Count)
        {
            AudioSource newAudioSource = main.gameObject.AddComponent<AudioSource>();
            newAudioSource.loop = false; newAudioSource.playOnAwake = false;
            main.audioChannels.Add(newAudioSource);
        }
        if (!main.loadedAudio.ContainsKey(audioName)) { Debug.LogError($"\"{audioName}\" is not a recognized audio file. Did you make a typo or forgot to load it into the prefab?"); return; }
        main.audioChannels[channel].clip = main.loadedAudio[audioName];
        main.audioChannels[channel].volume = volume;
        main.audioChannels[channel].pitch = pitch;
        main.audioChannels[channel].panStereo = panning;
        main.audioChannels[channel].Play();
    }
    private void Awake()
    {
        if(main != null)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
        DontDestroyOnLoad(gameObject);
        loadedAudio = new Dictionary<string, AudioClip>();
        foreach(AudioClip clip in allAudio)
        {
            loadedAudio.Add(clip.name, clip);
        }
        audioChannels = new List<AudioSource>();
    }
}
public enum GameDifficulty {Easy,Normal,Hard}
public enum GameControlScheme {Null,Mobile,PC}