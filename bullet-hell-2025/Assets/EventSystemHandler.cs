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

    public void ChangeSelectedButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttons[index].buttonGameObject);
    }
}
