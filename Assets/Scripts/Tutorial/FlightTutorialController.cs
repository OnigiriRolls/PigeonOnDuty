using UnityEngine;

public class FlightTutorialController : MonoBehaviour
{
    private enum FlightTutorialStep
    {
        Inactive,
        Story,
        CheckpointOne,
        CheckpointTwo,
        Completed
    }

    [SerializeField] private CheckpointsManager checkpointsManager;
    [SerializeField] private string tutorialTitle = "FLYING";
    [SerializeField] private string tutorialContent = "Reach the checkpoint and learn how to fly.";
    [SerializeField] private string tutorialTitle2 = "Good job!";
    [SerializeField] private string tutorialContent2 = "One more checkpoint and you will start the real delivery missions!";
    [SerializeField] private string storyTitle = "Pigeons Story";
    [SerializeField] private string storyContent = "With phones gone, people employ pigeons to deliver messages and goods. Become the best employed pigeon now!";
    [SerializeField] private FlightControlsUI flightControlsUI;

    private FlightTutorialStep currentStep;

    private void Awake()
    {
        currentStep = FlightTutorialStep.Inactive;
    }

    private void Start()
    {
        checkpointsManager.OnCheckpointReached += HandleCheckpointReached;
        TutorialUI.Instance.OnTutorialClosed += HandleTutorialClosed;
        if (SaveManager.Instance.Data.flightTutorialCompleted)
        {
            if (!MissionManager.Instance.StartMissionAfterLoad)
            {
                MissionManager.Instance.RequestMissionSelection();
            }
        }
        else
        {
            StartTutorial();
        }
    }

    private void OnDisable()
    {
        if (checkpointsManager != null)
            checkpointsManager.OnCheckpointReached -= HandleCheckpointReached;
        if (TutorialUI.Instance != null)
            TutorialUI.Instance.OnTutorialClosed -= HandleTutorialClosed;
    }

    public void StartTutorial()
    {
        if (SaveManager.Instance.Data.flightTutorialCompleted)
        {
            CompleteTutorial();
            return;
        }

        currentStep = FlightTutorialStep.Story;
        ShowStoryMessage();
    }

    private void ShowStoryMessage()
    {
        TutorialUI.Instance.Show(storyTitle, storyContent);
    }

    private void HandleTutorialClosed()
    {
        if (currentStep != FlightTutorialStep.Story)
            return;
        TutorialUI.Instance.OnTutorialClosed -= HandleTutorialClosed;
        currentStep = FlightTutorialStep.CheckpointOne;
        ShowTutorialMessage();
        flightControlsUI.ShowFlightPanel();
        checkpointsManager.SpawnNextCheckpoint();
    }

    private void ShowTutorialMessage()
    {
        TutorialUI.Instance.Show(tutorialTitle, tutorialContent);
    }

    private void HandleCheckpointReached(Transform checkpoint)
    {
        switch (currentStep)
        {
            case FlightTutorialStep.CheckpointOne:
                ShowSecondCheckpointMessage();
                SpawnSecondCheckpoint();
                break;
            case FlightTutorialStep.CheckpointTwo:
                CompleteTutorial();
                break;
        }
    }

    private void ShowSecondCheckpointMessage()
    {
        TutorialUI.Instance.Show(tutorialTitle2, tutorialContent2);
    }

    private void SpawnSecondCheckpoint()
    {
        currentStep = FlightTutorialStep.CheckpointTwo;
        checkpointsManager.SpawnNextCheckpoint();
    }

    private void CompleteTutorial()
    {
        currentStep = FlightTutorialStep.Completed;
        SaveManager.Instance.Data.flightTutorialCompleted = true;
        SaveManager.Instance.Save();
        flightControlsUI.HideFlightPanel();
        checkpointsManager.CleanCurrentObjective();
        MissionManager.Instance.RequestMissionSelection();
    }
}
