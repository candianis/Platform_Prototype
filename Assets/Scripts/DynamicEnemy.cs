using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EnemyState
{
    Prepare,
    Attack,
    Recover,
    Patrol,
    Pursuit,
    Dead,
}

public class DynamicEnemy : MonoBehaviour
{
    [Header("General Information")]
    [SerializeField] EnemyState currentState;

    [Header("Movement Settings")]
    [SerializeField] float speed;
    [SerializeField] float distance;
    [SerializeField] float smoothFactor;

    [Header("Point Settings")]
    [SerializeField] Transform[] checkpoints;
    int currentIndex;
    int listLength;

    Vector2[] targetPositions;
    Vector2 velocity;

    [Header("Perception Settings")]
    [SerializeField] float perceptionRadius;
    [SerializeField] Transform perceptionOrigin;
    [SerializeField] string tagToTarget;
    Collider2D[] perceivedColliders;
    Transform m_target;

    [Header("Health Settings")]
    public int maxHealth;
    [HideInInspector]
    public int currentHealth;

    [Header("Attack Settings")]
    [SerializeField] float attackRange;
    [SerializeField] float attackCooldown;
    private float timeLeftToRecover;

    void Start()
    {
        currentIndex = 0;
        listLength = checkpoints.Length;
        targetPositions = new Vector2[checkpoints.Length];
        for(int i = 0; i < checkpoints.Length; i++)
        {
            targetPositions[i] = new Vector2(checkpoints[i].position.x, checkpoints[i].position.y);
            checkpoints[i].gameObject.SetActive(false);
        }

        currentHealth = maxHealth;
        timeLeftToRecover = 0;
    }

    void Update()
    {
        PerceptionManager(perceivedColliders, tagToTarget);
        DecisionManager();
    }

    private void FixedUpdate()
    {
        perceivedColliders = Physics2D.OverlapCircleAll(new Vector2(perceptionOrigin.position.x, perceptionOrigin.position.y), perceptionRadius);
    }

    void DecisionManager()
    {
        if (currentHealth <= 0)
            currentState = EnemyState.Dead;

        if(m_target != null)
        {
            if (Vector2.Distance(transform.position, m_target.position) < attackRange)
            {
                if (timeLeftToRecover > 0)
                    currentState = EnemyState.Recover;
                
                else
                    currentState = EnemyState.Attack;
            }
            else
                currentState = EnemyState.Pursuit;
        }

        else
            currentState = EnemyState.Patrol;

        ActionManager();
        MovementManager();
    }

    public void PerceptionManager(Collider2D[] perceivedColliders, string tagToTarget)
    {
        m_target = null;
        if (perceivedColliders != null)
        {
            foreach (Collider2D collider in perceivedColliders)
            {
                //Sometimes the reference to the collider is lost so we just continue
                if (!collider)
                    continue;

                if (collider.CompareTag(tagToTarget))
                {
                    m_target = collider.transform;
                }
            }
        }
    }

    void ActionManager()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                CheckArrival();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Recover:
                Recover();
                break;

            default:
                break;
        }
    }

    void Attack()
    {
        m_target.GetComponent<Player>().Hit();
        timeLeftToRecover = attackCooldown;
    }

    void Recover()
    {
        timeLeftToRecover -= Time.deltaTime;
    }

    void CheckArrival()
    {
        float currentDistance = Vector2.Distance(transform.position, targetPositions[currentIndex]);
        if (currentDistance < distance)
        {
            if (currentIndex < listLength - 1) currentIndex++;
            else currentIndex = 0;
        }
    }

    void MovementManager()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Pursuit:
                Pursuit();
                break;

            default:
                break;
        }
    }

    void Patrol()
    {
        transform.position = Vector2.SmoothDamp(transform.position, targetPositions[currentIndex], ref velocity, smoothFactor, speed);
    }

    void Pursuit()
    {
        transform.position = Vector2.SmoothDamp(transform.position, m_target.position, ref velocity, smoothFactor, speed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(perceptionOrigin.position, perceptionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
