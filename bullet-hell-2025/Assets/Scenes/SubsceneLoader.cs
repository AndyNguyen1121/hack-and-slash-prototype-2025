using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Subscene
{
    MainMenu = 0,
    SampleScene = 1,
    SubScene1 = 2,
    Subscene2 = 3,
}
public class SubsceneLoader : MonoBehaviour
{
    public static SubsceneLoader instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "SampleScene")
            SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
    }

    public void LoadSubscene(Subscene subscene)
    {
        SceneManager.LoadSceneAsync(subscene.ToString(), LoadSceneMode.Additive);
    }

    public void UnloadSubscene(Subscene subscene)
    {
        SceneManager.UnloadSceneAsync(subscene.ToString());
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
        Time.timeScale = 1;
    }

    public void QuitApplication()
    {
        Application.Quit();
        Time.timeScale = 1;
    }
}
