using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "";
    [SerializeField] private string sceneName2 = "";
    public void PlayGame()
    {
        SceneManager.LoadScene(sceneName);
    }
    
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName2);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Button Pressed");
        Application.Quit();
    }
}