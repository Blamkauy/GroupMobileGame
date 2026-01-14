using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject[] menus;//0: Input Select, 1: MainMenu, 2: play menu
    private void Awake()
    {
        SelectMenu(GameManager.main.controlScheme == GameControlScheme.Null ? 0 : 1);
    }
    public void SelectMenu(int ID)
    {
        foreach (GameObject go in menus) { go.SetActive(false); }
        menus[ID].SetActive(true);
    }
    public void SelectInputType(int ID)
    {
        GameManager.main.controlScheme = (GameControlScheme)ID;
    }
    public void OpenSettings()
    {
        GameManager.main.SetSettingsmenu(true);
    }
    public void StartGame(int difficulty)
    {
        GameManager.main.difficulty = (GameDifficulty)difficulty;
        Debug.Log($"Player has started the game on difficulty: {GameManager.main.difficulty.ToString()}");
        PlayerController.SpawnWithWeaponID = -1;
        LoadScene(1);
    }
    public void LoadScene(int ID)
    {
        GameManager.main.LoadScene(ID);
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Player as exitted game!");
    }
}
