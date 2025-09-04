using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    private PlayerManager playerManager;    

    [Header("Sliders")]
    public Slider healthSlider;
    public Slider trailingHealthSlider;
    public Transform healthCanvasGroup;

    [Header("Lock-On Sliders")]
    public Slider lockOnSlider;
    public GameObject lockOnCanvas;

    [Header("Pause Menu")]
    public bool isPaused;
    public MenuAnimator pauseMenuAnimator;
    public GameObject pauseMenuCanvas;


    private DG.Tweening.Sequence sliderUpdateSequence;
    private Tween sliderShake;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerManager = PlayerManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        HandleLockOnSlider();
        HandlePauseMenu();
    }

    public void SetHealthSliderValue(float value)
    {
        healthSlider.value = value;
        trailingHealthSlider.value = value;
    }

    public void UpdateHealthSliders(float health, float maxHealth)
    {
        if (sliderShake != null)
            sliderShake.Rewind();

        if (sliderUpdateSequence != null)
            sliderUpdateSequence.Kill();

        sliderShake = healthCanvasGroup.DOShakePosition(0.5f, 10, 20, 90, false, true, ShakeRandomnessMode.Harmonic);

        sliderUpdateSequence = DOTween.Sequence();
        sliderUpdateSequence
            .Append(healthSlider.DOValue(health / maxHealth, 0.1f))
            .AppendInterval(0.5f)
            .Append(trailingHealthSlider.DOValue(health / maxHealth, 0.1f));
    }

    IDamageable healthScript;
    Transform currentTarget;
    public void HandleLockOnSlider()
    {
        if (playerManager.playerCameraManager.currentLockOnTarget == null)
        {
            currentTarget = null;
            healthScript = null;
            lockOnCanvas.SetActive(false);
            return;
        }

        Transform lockOnTarget = playerManager.playerCameraManager.currentLockOnTarget;


        if (lockOnTarget != currentTarget)
        {
            currentTarget = lockOnTarget;
            healthScript = currentTarget.GetComponentInChildren<IDamageable>();
        }

        if (healthScript == null) return;

        lockOnSlider.value = healthScript.Health / healthScript.MaxHealth;
        lockOnCanvas.SetActive(true);

    }

    public void HandlePauseMenu()
    {
        if (PlayerInputManager.instance.pauseButtonPressed)
        {
            if (!isPaused)
            {
                isPaused = true;
                pauseMenuCanvas.gameObject.SetActive(true);
                pauseMenuAnimator.gameObject.SetActive(true);
                PlayerInputManager.instance.DisableAllInputsExceptUI();
                EventSystemHandler.instance.ChangeSelectedButton(0);
                Time.timeScale = 0;
            }
            else
            {
                if (pauseMenuAnimator.gameObject.activeSelf)
                {
                    isPaused = false;
                    pauseMenuAnimator.DisableMenu();
                    PlayerInputManager.instance.EnableAllInputs();
                    Time.timeScale = 1f;
                }
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        PlayerInputManager.instance.EnableAllInputs();
        pauseMenuAnimator.DisableMenu();
        Time.timeScale = 1f;
    }
}
