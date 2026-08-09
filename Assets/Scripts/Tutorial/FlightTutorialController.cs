using UnityEngine;

public class FlightTutorialController : MonoBehaviour
{
    private enum FlightTutorialStep
    {
        Inactive,
        CheckpointOne,
        CheckpointTwo,
        Completed
    }

    [SerializeField] private CheckpointsManager checkpointsManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private string tutorialTitle = "FLYING";
    [SerializeField] private string tutorialContent = "Reach the checkpoint and learn how to fly.";
    [SerializeField] private FlightControlsUI flightControlsUI;

    private FlightTutorialStep currentStep;

    private void Awake()
    {
        currentStep = FlightTutorialStep.Inactive;
    }

    private void OnEnable()
    {
        if (checkpointsManager != null)
            checkpointsManager.OnCheckpointReached += HandleCheckpointReached;
    }

    private void OnDisable()
    {
        if (checkpointsManager != null)
            checkpointsManager.OnCheckpointReached -= HandleCheckpointReached;
    }

    public void StartTutorial()
    {
        if (SaveManager.Instance.Data.flightTutorialCompleted)
        {
            CompleteTutorial();
            return;
        }

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
        TutorialUI.Instance.Show("Good job!", "One more checkpoint and you will start the real delivery missions!");
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
        missionManager.RequestMissionSelection();
    }
}
