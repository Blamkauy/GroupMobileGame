using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame(int difficulty)
    {
        GameManager.main.difficulty = (GameDifficulty)difficulty;
        Debug.Log($"Player has started the game on difficulty:{GameManager.main.difficulty.ToString()}");
        LoadScene(1);
    }
    public void LoadScene(int ID)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(ID);
        Debug.Log("Player as exited game!");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
