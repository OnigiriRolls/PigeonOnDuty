using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private EndlessRunManager runManager;
    [SerializeField] private TextMeshProUGUI throttleText;
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

    private AltitudeLayer currentLayer;

    private void Start()
    {
        AudioManager.Instance.CrossfadeMusic(lowMusic);
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
        throttleText.text = $"Throttle: {player.Throttle:F0}";
    }

    private void UpdateSpeed()
    {
        float speedKmh = player.Velocity.magnitude * 3.6f;
        speedText.text = $"Speed: {speedKmh:F0} km/h";
    }

    private void UpdateAltitude()
    {
        altitudeText.text = $"Altitude: {player.transform.position.y:F0} m";
    }

    private void CheckAltitudeLayer()
    {
        AltitudeLayer newLayer = GetAltitudeLayer();
        if (newLayer == currentLayer)
            return;

        currentLayer = newLayer;
        ShowAltitudeTransition();
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
        float timer = runManager.CheckpointTimer;
        UpdateTimerAudio(timer);
        timerText.text = Mathf.CeilToInt(timer).ToString();
        if (timer <= 10f)
        {
            timerText.color = Color.red;
            if (!clockAnimator.enabled)
            {
                clockAnimator.enabled = true;
            }
        }
        else
        {
            if (clockAnimator.enabled)
            {
                clockAnimator.enabled = false;
            }
            timerText.color = Color.black;
        }
    }

    private void UpdateTimerAudio(float timer)
    {
        if (timer <= 5f)
        {
            AudioManager.Instance.PlayUILoop(clockCriticalClip);
        }
        else if (timer <= 10f)
        {
            AudioManager.Instance.PlayUILoop(clockWarningClip);
        }
        else
        {
            AudioManager.Instance.StopUILoop();
        }
    }
}
