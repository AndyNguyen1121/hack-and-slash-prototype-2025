using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider volumeSlider;
    public Slider lookSlider;

    public CinemachineInputAxisController inputController;

    FMOD.Studio.Bus master;
    Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
        if (AudioManager.instance == null)
        {
            Instantiate(Resources.Load<GameObject>("AudioManager"));
        }
        mainCam = Camera.main;
        master = FMODUnity.RuntimeManager.GetBus("bus:/");
        if (PlayerPrefs.GetFloat("Volume") == 0)
        {
            PlayerPrefs.SetFloat("Volume", 0.25f);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.GetFloat("Look Sensitivity") == 0)
        {
            PlayerPrefs.SetFloat("Look Sensitivity", 0.25f);
            PlayerPrefs.Save();
        }

        if (inputController != null)
        {
            inputController.Controllers[0].Input.Gain = PlayerPrefs.GetFloat("Look Sensitivity") * 200;
            inputController.Controllers[1].Input.Gain = -PlayerPrefs.GetFloat("Look Sensitivity");
            lookSlider.value = PlayerPrefs.GetFloat("Look Sensitivity");
        }

        lookSlider.onValueChanged.AddListener(ChangeLookSensitivity);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        master.setVolume(PlayerPrefs.GetFloat("Volume"));
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
    }

    public void ChangeVolume(float value)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.sliderChange, mainCam.transform.position);
        PlayerPrefs.SetFloat("Volume", value);
        master.setVolume(value); 
        PlayerPrefs.Save();
    }

    public void ChangeLookSensitivity(float value)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.sliderChange, mainCam.transform.position);
        PlayerPrefs.SetFloat("Look Sensitivity", value);

        if (inputController != null)
        {
            inputController.Controllers[0].Input.Gain = PlayerPrefs.GetFloat("Look Sensitivity") * 200;
            inputController.Controllers[1].Input.Gain = -PlayerPrefs.GetFloat("Look Sensitivity");
        }

        PlayerPrefs.Save();
    }
}
