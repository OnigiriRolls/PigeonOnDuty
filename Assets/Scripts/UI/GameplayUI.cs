using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : StopAudio
{
    public AltitudeLayer CurrentLayer => currentLayer;

    [SerializeField] private PlayerController player;
    [SerializeField] private MissionManager deliveryMissionManager;
    [SerializeField] private TextMeshProUGUI throttleText;
    [SerializeField] private Image throttleBar;
    [SerializeField] private GameObject throttleLimit;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI altitudeText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Animator altitudeAnimator;
    [SerializeField] private Animator clockAnimator;
    [SerializeField] private AudioClip clockWarningClip;
    [SerializeField] private AudioClip clockCriticalClip;
    [SerializeField] private AudioClip altitudeTransitionClip;
    [SerializeField] private AudioClip lowMusic;
    [SerializeField] private AudioClip midMusic;
    [SerializeField] private AudioClip highMusic;
    [SerializeField] private float midAltitude = 51f;
    [SerializeField] private float highAltitude = 151f;
    [SerializeField] private float maxThrottle = 130f;
    [SerializeField] private MissionTimer missionTimer;

    private AltitudeLayer currentLayer;
    private Color timerInitialColor;

    protected override void Start()
    {
        base.Start();
        timerInitialColor = timerText.color;
    }

    private void Update()
    {
        UpdateThrottle();
        UpdateSpeed();
        UpdateAltitude();
        UpdateTimer();
        CheckAltitudeLayer();
    }

    private void UpdateThrottle()
    {
        throttleBar.fillAmount = player.Throttle / maxThrottle;
    }

    private void UpdateSpeed()
    {
        float speedKmh = player.Velocity.magnitude * 3.6f;
        speedText.text = $"Speed: {speedKmh:F0} km/h";
    }

    private void UpdateAltitude()
    {
       // altitudeText.text = $"Altitude: {player.transform.position.y:F0} m";
    }

    private void CheckAltitudeLayer()
    {
        //AltitudeLayer newLayer = GetAltitudeLayer();
        //if (newLayer == currentLayer)
        //    return;

        //currentLayer = newLayer;
        //ShowAltitudeTransition();
    }

    private void ShowAltitudeTransition()
    {
        altitudeAnimator.SetTrigger("Pulse");
        AudioManager.Instance.PlaySFX(altitudeTransitionClip);
        switch (currentLayer)
        {
            case AltitudeLayer.Low:
                AudioManager.Instance.CrossfadeMusic(lowMusic);
                break;
            case AltitudeLayer.Mid:
                AudioManager.Instance.CrossfadeMusic(midMusic);
                break;
            case AltitudeLayer.High:
                AudioManager.Instance.CrossfadeMusic(highMusic);
                break;
        }
    }

    private AltitudeLayer GetAltitudeLayer()
    {
        float altitude = player.transform.position.y;
        if (altitude < midAltitude)
        {
            return AltitudeLayer.Low;
        }
        if (altitude < highAltitude)
        {
            return AltitudeLayer.Mid;
        }
        return AltitudeLayer.High;
    }

    private void UpdateTimer()
    {
        if (!missionTimer.IsRunning)
        {
            AudioManager.Instance.StopUILoop();
            timerText.text = "";
            if (clockAnimator.enabled)
                clockAnimator.enabled = false;
            timerText.color = timerInitialColor;
            return;
        }
        float timer = missionTimer.RemainingTime;
        timer = Mathf.Max(timer, 0f);
        timerText.text = Mathf.CeilToInt(timer).ToString();
        UpdateTimerAudio(timer);
        if (timer <= 10f)
        {
            timerText.color = Color.red;
            if (!clockAnimator.enabled)
                clockAnimator.enabled = true;
        }
        else
        {
            if (clockAnimator.enabled)
                clockAnimator.enabled = false;
            timerText.color = timerInitialColor;
        }
    }

    private void UpdateTimerAudio(float timer)
    {
        if (timer > 0 && timer <= 5f)
        {
            AudioManager.Instance.PlayUILoop(clockCriticalClip);
        }
        else if (timer > 5 && timer <= 10f)
        {
            AudioManager.Instance.PlayUILoop(clockWarningClip);
        }
        else
        {
            AudioManager.Instance.StopUILoop();
        }
    }

    protected override void HandleGameOver()
    {
        AudioManager.Instance.StopUILoop();
        gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
