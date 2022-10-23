using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public void LoadLevel(int index)
    {
        StartCoroutine(Load(index));
    }
    public void ReloadLevel()
    {
        StartCoroutine(Load(SceneManager.GetActiveScene().buildIndex));
    }

    IEnumerator Load(int index)
    {
        //play animation
        transition.SetTrigger("Start");
        //Wait
        yield return new WaitForSeconds(transitionTime);
        //load scene
        SceneManager.LoadScene(index);
    }
}
