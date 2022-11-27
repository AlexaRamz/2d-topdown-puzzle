using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
    }

    public void NextLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void QuitGame()
    {//bless unity tutorials man
        Debug.Log("Quit");
        Application.Quit();
    }

}
