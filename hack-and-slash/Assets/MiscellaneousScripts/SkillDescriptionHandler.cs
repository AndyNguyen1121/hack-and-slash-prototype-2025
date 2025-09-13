using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillDescriptionHandler : MonoBehaviour
{
    public List<GameObject> descriptions = new();
    public int currentDescription;
    public GameObject nextButton;
    public GameObject prevButton;

    private void OnEnable()
    {
        currentDescription = 0;
        EnableDescription(0);

        prevButton.SetActive(true);
        nextButton.SetActive(true);
    }
    public void NextDescription()
    {
        if (currentDescription + 1 >= descriptions.Count)
        {
            return;
        }

        ++currentDescription;

        EnableDescription(currentDescription);

        nextButton.SetActive(true);
        prevButton.SetActive(true);
    }

    public void PreviousDescription()
    {
        if (currentDescription - 1 < 0)
        {
            return;
        }

        --currentDescription;

        EnableDescription(currentDescription);
       
        prevButton.SetActive(true);
        nextButton.SetActive(true);
    }

    private void EnableDescription(int description)
    {
        for (int i = 0; i < descriptions.Count; ++i)
        {
            if (i == description)
            {
                descriptions[i].SetActive(true);
            }
            else
            {
                descriptions[i].SetActive(false);
            }
        }
    }
}
