using UnityEngine;

public class NewspaperTutorialController : MonoBehaviour
{
    private enum TutorialStep
    {
        Inactive,
        GetNewspaper,
        DeliverNewspaper,
        Completed
    }

    [SerializeField] private TutorialDogController tutorialDogPrefab;
    [SerializeField] private ClientController tutorialClientPrefab;
    [SerializeField] private Transform dogSpawnPoint;
    [SerializeField] private Transform clientSpawnPoint;
    [SerializeField] private Transform player;
    [SerializeField] private ThrowableInventory playerInventory;
    [SerializeField] private ThrowableData newspaperData;
    [SerializeField] private ThrowableData featherData;
    [SerializeField] private NewsMissionController newsMissionController;
    [SerializeField] private FlightControlsUI controlsUI;
    [SerializeField] private MinimapMissionController minimapController;
    [SerializeField] private ScreenTransition screenTransition;
    [SerializeField] private string firstTitle = "GET THE NEWSPAPER";
    [SerializeField] private string firstContent = "The dog has stolen the newspaper. Hit the dog with a Feather to get it back.";
    [SerializeField] private string secondTitle = "DELIVER THE NEWSPAPER";
    [SerializeField] private string secondContent = "You got the newspaper! Now deliver it to the person.";
    [SerializeField] private string completedTitle = "YOU'RE READY!";
    [SerializeField] private string completedContent = "You are ready for the real deal!";

    private TutorialStep currentStep;
    private TutorialDogController tutorialDog;
    private ClientController tutorialClient;
    private NewsMission pendingMission;
    private bool waitingForNewspaper;

    private void Awake()
    {
        currentStep = TutorialStep.Inactive;
    }

    private void Update()
    {
        if (currentStep != TutorialStep.GetNewspaper)
            return;
        if (!waitingForNewspaper)
            return;
        if (playerInventory.GetAmount(newspaperData) <= 0)
            return;

        waitingForNewspaper = false;
        StartDeliveryStep();
    }

    public void StartTutorial(NewsMission mission)
    {
        if (SaveManager.Instance.Data.newspaperTutorialCompleted)
        {
            newsMissionController.StartMission(mission);
            return;
        }

        pendingMission = mission;
        currentStep = TutorialStep.GetNewspaper;
        SetupInventory();
        SpawnTutorialCharacters();
        controlsUI.ShowNewsPanel();
        TutorialUI.Instance.Show(firstTitle, firstContent);
    }

    private void SetupInventory()
    {
        playerInventory.SetAmount(newspaperData, 0);
        playerInventory.SetAmount(featherData, 1);
        playerInventory.SetEnabled(newspaperData, true);
        playerInventory.SetEnabled(featherData, true);
        playerInventory.Select(featherData);
    }

    private void SpawnTutorialCharacters()
    {
        tutorialDog = Instantiate(tutorialDogPrefab, dogSpawnPoint.position, dogSpawnPoint.rotation);
        tutorialDog.SetupTutorialNewspaper();
        tutorialDog.OnScared += HandleDogScared;
        tutorialClient = Instantiate(tutorialClientPrefab, clientSpawnPoint.position, clientSpawnPoint.rotation);
        tutorialClient.Initialize(newspaperData);
        tutorialClient.BecomeClient();
        tutorialClient.OnDeliveryCompleted += HandleDeliveryCompleted;
        minimapController.ShowClient(tutorialClient.transform);
        minimapController.ShowCheckpoint(tutorialDog.transform);
    }

    private void HandleDogScared()
    {
        if (currentStep != TutorialStep.GetNewspaper)
            return;
        waitingForNewspaper = true;
        TutorialUI.Instance.Show("Pick Up", "Pick up the newspaper the dog dropped. And if you remain with no feathers, you can always collect more from the Stadium");
    }

    private void StartDeliveryStep()
    {
        currentStep = TutorialStep.DeliverNewspaper;
        TutorialUI.Instance.Show(secondTitle, secondContent);
    }

    private void HandleDeliveryCompleted(ClientController client)
    {
        if (currentStep != TutorialStep.DeliverNewspaper)
            return;
        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        currentStep = TutorialStep.Completed;
        SaveManager.Instance.Data.newspaperTutorialCompleted = true;
        SaveManager.Instance.Save();
        CleanupTutorial();
        controlsUI.HideNewsPanel();
        if (TutorialUI.Instance != null)
            TutorialUI.Instance.OnTutorialClosed += HandleTutorialClosed;
        TutorialUI.Instance.Show(completedTitle, completedContent);
    }

    private void HandleTutorialClosed()
    {
        StartRealMission();
        if (TutorialUI.Instance != null)
            TutorialUI.Instance.OnTutorialClosed -= HandleTutorialClosed;
    }

    private void StartRealMission()
    {
        screenTransition.FadeToBlack();
        newsMissionController.StartMission(pendingMission);
    }

    private void CleanupTutorial()
    {
        minimapController.HideClient(tutorialClient.transform);
        minimapController.HideCheckpoint(tutorialDog.transform);
        if (tutorialDog != null)
        {
            tutorialDog.OnScared -= HandleDogScared;
            Destroy(tutorialDog.gameObject);
        }

        if (tutorialClient != null)
        {
            tutorialClient.OnDeliveryCompleted -= HandleDeliveryCompleted;
            Destroy(tutorialClient.gameObject);
        }

        playerInventory.SetAmount(newspaperData, 0);
        playerInventory.SetAmount(featherData, 0);
        playerInventory.SetEnabled(newspaperData, false);
        playerInventory.SetEnabled(featherData, false);
    }

    private void OnDestroy()
    {
        if (tutorialDog != null)
            tutorialDog.OnScared -= HandleDogScared;
        if (tutorialClient != null)
            tutorialClient.OnDeliveryCompleted -= HandleDeliveryCompleted;
    }
}
