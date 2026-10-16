using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string cutScene = "";
    [SerializeField] private string SceneName = "";
    public void PlayGame()
    {
        SceneManager.LoadScene(cutScene);
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Button Pressed");
        Application.Quit();
    }
}