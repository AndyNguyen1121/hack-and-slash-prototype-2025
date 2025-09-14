using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    public WaveManager waveManager;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highestScoreText;

    private void OnEnable()
    {
        if (waveManager.waveCount > PlayerPrefs.GetFloat("HighScore"))
        {
            PlayerPrefs.SetFloat("HighScore", waveManager.waveCount);
            PlayerPrefs.Save();
        }

        finalScoreText.text = $"Final Score: {waveManager.waveCount}";
        highestScoreText.text = $"Personal Best: {PlayerPrefs.GetFloat("HighScore")}";
    }

}
