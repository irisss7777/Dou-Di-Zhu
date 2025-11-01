using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private WebGameManager webGameManager;
    private void Awake()
    {
        webGameManager.OnGameStart += () => {OpenLevel(1);};
    }
    
    public void PlayButtonClick()
    {
        OpenLevel(1);
    }
    
    public void CloseGame(){
        SceneManager.LoadScene(0);
    }

    public void FullyExitGame()
    {
        Application.Quit();
    }

    public void OpenLevel(int number){
        SceneManager.LoadScene(number);
    }

}
