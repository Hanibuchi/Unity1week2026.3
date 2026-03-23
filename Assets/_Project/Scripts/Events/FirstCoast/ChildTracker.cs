using UnityEngine;

public class ChildTracker : MonoBehaviour
{
    private TurtleBullyingEvent turtleBullyingEvent;
    private EnemyHealth enemyHealth;

    // TurtleBullyingEventをセットするメソッド
    public void SetTurtleBullyingEvent(TurtleBullyingEvent evt)
    {
        turtleBullyingEvent = evt;
    }

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (enemyHealth != null )
        {
            enemyHealth.onDeath += OnChildDeath;
        }
    }

    // EnemyHealthのonDeathに登録するメソッド
    private void OnChildDeath()
    {
        if (turtleBullyingEvent != null)
        {
            turtleBullyingEvent.OnChildEnemyDeath();
        }
    }
}
