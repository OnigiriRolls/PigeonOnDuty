using UnityEngine;

public class EnemyAggroManager : MonoBehaviour
{
    public static EnemyAggroManager Instance { get; private set; }

    private IEnemyPursuer activePursuer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool TryAcquire(IEnemyPursuer pursuer)
    {
        if (activePursuer != null)
        {
            return false;
        }
        activePursuer = pursuer;
        return true;
    }

    public void Release(IEnemyPursuer pursuer)
    {
        if (activePursuer == pursuer)
            ResetAggro();
    }

    public void ResetAggro()
    {
        activePursuer = null;
    }
}
