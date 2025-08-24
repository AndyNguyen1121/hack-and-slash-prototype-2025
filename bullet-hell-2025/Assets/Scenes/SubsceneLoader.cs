using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Subscene
{
    Subscene2 = 1
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            LoadSubscene(Subscene.Subscene2);
        }
    }

    public void LoadSubscene(Subscene subscene)
    {
        SceneManager.LoadSceneAsync(subscene.ToString(), LoadSceneMode.Additive);
    }

    public void UnloadSubscene(Subscene subscene)
    {
        SceneManager.UnloadSceneAsync(subscene.ToString());
    }
}
