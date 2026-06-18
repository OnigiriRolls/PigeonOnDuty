using UnityEngine;

[RequireComponent(typeof(HumanFollower))]
public class HumanRecallController : MonoBehaviour
{
    [SerializeField] private float recallDistance = 10f;
    [SerializeField] private float requiredDot = 0.8f;

    private HumanFollower human;
    private Transform player;
    private HintUI hintUI;

    private void Start()
    {
        human = GetComponent<HumanFollower>();
        player = FindAnyObjectByType<PlayerController>().transform;
        hintUI = FindAnyObjectByType<HintUI>();
    }

    private void Update()
    {
        if (!human.CanBeRecalled)
            return;
        if (!Input.GetKeyDown(KeyCode.E))
            return;
        TryRecall();
    }

    private void TryRecall()
    {
        Vector3 direction = (human.transform.position - player.position).normalized;
        float distance = Vector3.Distance(player.position, human.transform.position);
        if (distance > recallDistance)
            return;
        float dot = Vector3.Dot(player.forward, direction);
        if (dot < requiredDot)
            return;
        human.Recall();
    }

    private void OnDrawGizmos()
    {
        if (human == null)
            return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(  human.transform.position,  recallDistance);
    }
}
