using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public enum EnemyState
{
    Prepare,
    Attack,
    Recover,
    Patrol,
    Pursuit,
    Dead,
}

public class EnemyTemplate : MonoBehaviour
{
    [Header("General Information")]
    [SerializeField] EnemyState currentState;
    //Check if this enemy is ready
    private bool isReady;

    [Header("Movement Settings")]
    [SerializeField, Tooltip("How fast the enemy will patrol or pursuit")] 
    protected float speed;
    [SerializeField, Tooltip("How smooth should this enemy slow down when approaching the player or a patrol positions for example")] 
    protected float smoothFactor;
    protected Vector2 velocity;

    [Header("Perception Settings")]
    [SerializeField, Tooltip("How far can this enemy perceive the player")] 
    protected float perceptionRadius;
    [SerializeField] 
    protected Transform perceptionOrigin;
    [SerializeField] 
    protected string tagToTarget;
    protected Collider2D[] perceivedColliders;
    protected Transform m_target;

    [Header("Health Settings")]
    [Tooltip("With how much health should this enemy start with")]
    public int maxHealth;
    public int currentHealth;

    [Header("Attack Settings")]
    [SerializeField] float attackRange;
    [SerializeField, Tooltip("How many seconds should the enemy wait before htting the player again")] 
    float attackCooldown;
    private float timeLeftToRecover;

    void Start()
    {
        currentHealth = maxHealth;
        timeLeftToRecover = 0;
    }

    private void FixedUpdate()
    {
        perceivedColliders = Physics2D.OverlapCircleAll(new Vector2(perceptionOrigin.position.x, perceptionOrigin.position.y), perceptionRadius);
    }

    private void Update()
    {
        PerceptionManager(perceivedColliders, tagToTarget);
        DecisionManager();
        ActionManager();
        MovementManager();
    }

    protected void DecisionManager()
    {
        if (currentHealth <= 0)
        {
            currentState = EnemyState.Dead;
            return;
        }

        if (!isReady)
        {
            currentState = EnemyState.Prepare;
            return;
        }

        if (m_target != null)
        {
            if (Vector2.Distance(transform.position, m_target.position) < attackRange)
            {
                if (timeLeftToRecover > 0)
                    currentState = EnemyState.Recover;

                else
                    currentState = EnemyState.Attack;

                return;
            }
            else
                currentState = EnemyState.Pursuit;

        }

        else
            currentState = EnemyState.Patrol;


    }

    void PerceptionManager(Collider2D[] perceivedColliders, string tagToTarget)
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
            case EnemyState.Prepare:
                Prepare();
                break;

            case EnemyState.Patrol:
                CheckArrival();
                break;

            case EnemyState.Attack:
                Attack();
                break;

            case EnemyState.Recover:
                Recover();
                break;

            case EnemyState.Dead:
                Death();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Use this function to prepare all the values the enemy will need to work properly, it will be called once at the start. 
    /// NOTE: It is necessary to call the father's function first
    /// </summary>
    virtual protected void Prepare()
    {
        isReady = true;
    }

    /// <summary>
    /// Use this function to change how this enemy patrols
    /// NOTE: it is not necessary to call the father's function
    /// </summary>
    virtual protected void CheckArrival()
    {

    }

    /// <summary>
    /// Function to attack the player and start a cooldown
    /// Note: It is not necessary to call the father's function
    /// </summary>
    virtual protected void Attack()
    {
        m_target.GetComponent<Player>().Hit();
        timeLeftToRecover = attackCooldown;
    }

    /// <summary>
    /// Function to use after an attack to start to cooldown
    /// </summary>
    virtual protected void Recover()
    {
        timeLeftToRecover -= Time.deltaTime;
    }

    /// <summary>
    /// Function to use when this enemy's life reaches zero
    /// </summary>
    virtual protected void Death()
    {

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

    virtual protected void Patrol()
    {

    }

    virtual protected void Pursuit()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(perceptionOrigin.position, perceptionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

