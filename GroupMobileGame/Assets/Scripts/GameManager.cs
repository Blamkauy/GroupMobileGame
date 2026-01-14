using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager main;
    public bool gamePaused = false;
    public GameDifficulty difficulty;
    public GameControlScheme controlScheme;
    [SerializeField] AudioClip[] allAudio;
    public Dictionary<string, AudioClip> loadedAudio;
    List<AudioSource> audioChannels;
    public GameObject[] UIMenus;
    public Slider volumeSlider;
    [SerializeField] DroppedItem droppedItemPrefab;
    [SerializeField] ParticleSystem[] effectParticles;
    public WeaponEntry[] AllAvailiableWeapons;
    public Material[] avaliableMaterials;
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
        volumeSlider.value = 0.5f;

        for(int i = 0; i < AllAvailiableWeapons.Length; i++)
        {
            AllAvailiableWeapons[i].prefabReference.SpawnID = i;
        }
    }

    public void SetPaused(bool value)
    {
        gamePaused = value;
        Time.timeScale = gamePaused ? 0f : 1;
        UIMenus[0].gameObject.SetActive(value);
        UIMenus[1].gameObject.SetActive(false);
    }
    public void SetSettingsmenu(bool value)
    {
        UIMenus[1].gameObject.SetActive(value);
    }
    public void LoadScene(int ID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ID);
    }
    public void SetVolume(float value)
    {
        if(volumeSlider.value!=value) volumeSlider.value = value; else
            AudioListener.volume = value;
    }
    public DroppedItem SpawnDroppedItem(int ID, int Seed, Vector3 pos)
    {
        DroppedItem di = Instantiate(droppedItemPrefab, Vector3.zero, Quaternion.identity);
        di.ItemID = ID; di.randomSeed = Seed; di.position = pos;
        return di;

    }
    public Weapon SpawnWeapon(int ID, int Seed, Vector3 pos)
    {
        Weapon di = Instantiate(AllAvailiableWeapons[ID].prefabReference, Vector3.zero, Quaternion.identity);
        di.Randomize(Seed); di.position = pos;
        return di;

    }
    public ParticleSystem SpawnParticles(int ID)
    {
        return Instantiate(effectParticles[ID]);
    }
    private void Update()
    {
    }
}
public enum GameDifficulty {Easy,Normal,Hard}
public enum GameControlScheme {Null,Mobile,PC}