using UnityEngine;

public class MobileControlsManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject inventoryButtons;
    [SerializeField] private GameObject flightControls;
    [SerializeField] private GameObject gpsControls;
    [SerializeField] private GameObject newspaperControls;

    private void Awake()
    {
        ShowNormalControls();
    }

    private void Start()
    {
        MissionManager.Instance.OnMissionSelected += HandleMissionSelected;
    }

    private void HandleMissionSelected(MissionData mission)
    {
        if (mission is GPSMission)
        {
            ShowGPSControls();
        }
        else if (mission is NewsMission)
        {
            ShowNewspaperControls();
        }
        else
        {
            ShowNormalControls();
        }
    }

    public void ShowBaseControls()
    {
        flightControls.SetActive(true);
        cameraButton.SetActive(true);
        pauseButton.SetActive(true);
    }

    public void ShowGPSControls()
    {
        ShowBaseControls();
        gpsControls.SetActive(true);
        newspaperControls.SetActive(true);
    }

    public void ShowNewspaperControls()
    {
        ShowBaseControls();
        gpsControls.SetActive(false);
        newspaperControls.SetActive(true);
    }

    public void ShowNormalControls()
    {
        ShowBaseControls();
        gpsControls.SetActive(false);
        newspaperControls.SetActive(false);
    }

    private void OnDisable()
    {
        MissionManager.Instance.OnMissionSelected -= HandleMissionSelected;
    }
}
