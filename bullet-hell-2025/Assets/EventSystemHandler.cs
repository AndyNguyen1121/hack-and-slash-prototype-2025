using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[System.Serializable]
public class ButtonNames
{
    public string name;
    public GameObject buttonGameObject;
}
public class EventSystemHandler : MonoBehaviour
{
    public List<ButtonNames> buttons = new List<ButtonNames>();

    [SerializeField]
    private List<GameObject> buttonStack = new();

    public static EventSystemHandler instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("There is more than one EventSystemHandler in the scene.");
    }
    public void ChangeSelectedButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttons[index].buttonGameObject);
        buttonStack.Add(buttons[index].buttonGameObject);
    }

    public void SearchForValidButton()
    {

    }
}
