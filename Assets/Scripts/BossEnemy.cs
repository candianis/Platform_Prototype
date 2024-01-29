using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BossAttackState
{
    HorizontalMissiles,
    GuidedMissiles,
    Punch,
    Dash,
    Laser,
    Walk
}

public class BossEnemy : EnemyTemplate
{
    [Header("Boss Attack Information")]
    [SerializeField] BossAttackState currentAttack;

    [Header("Missile Settings")]
    [SerializeField] float missileCooldown;
    public float missileCurrentTime;
    [SerializeField] Transform missileOrigin;
    [SerializeField] int maxMissileAmount;
    [SerializeField] GameObject guidedMissilePrefab;

    [Header("Dash Settings")]
    [SerializeField, Tooltip("How fast the boss will ")] 
    float dashSpeed;
    [SerializeField] float timeToRecoverFromDash;
    [SerializeField] Transform[] dashTargetPositions;
    [SerializeField] Vector2[] dashTargets;
    int currentDashPosition;

    [Header("Punch Settings")]
    [SerializeField] Transform punchOrigin;
    [SerializeField] Vector3 hitbox;
    [SerializeField] float timeToRecoverFromPunch;

    [Header("Laser Settings")]
    //The time left for the boss to reuse the laser. It needs to reach zero to become available again
    public float laserCurrentTime;
    [SerializeField, Tooltip("Amount of time it will take for the laser attack to be used again")] 
    float laserCooldown;
    [SerializeField] Vector3 laserHitbox;

    protected override void Prepare()
    {
        base.Prepare();

        dashTargets = new Vector2[dashTargetPositions.Length];
        for(int i = 0; i < dashTargetPositions.Length; i++)
        {
            dashTargets[i] = dashTargetPositions[i].position;
        }
    }

    override protected void AttackManager()
    {
        int rand = Random.Range(0, 9);

        if (laserCurrentTime <= 0)
            currentAttack = BossAttackState.Laser;

        else if (missileCurrentTime <= 0)
        {
            currentAttack = BossAttackState.GuidedMissiles;
        }

        else
        {
            if (rand >= 5)
                currentAttack = BossAttackState.Dash;

            else if(!currentAttack.Equals(BossAttackState.Dash))
                currentAttack = BossAttackState.Punch;

            else
                currentAttack = BossAttackState.Dash;
        }

        ExecuteAttack();
    }

    void ExecuteAttack()
    {
        switch (currentAttack)
        {
            case BossAttackState.GuidedMissiles:
                FireGuidedMissiles();
                break;

            case BossAttackState.Laser:
                FireLaser();
                break;

            case BossAttackState.Punch:
                Punch();
                break;

            default:
                break;
        }
    }

    void FireLaser()
    {
        Collider2D[] collidersToPunch;
        collidersToPunch = Physics2D.OverlapBoxAll(transform.position, laserHitbox, 0);
        foreach (Collider2D collider in collidersToPunch)
        {
            if (collider.TryGetComponent(out Player player))
            {
                player.Hit();
            }
        }
        timeLeftToRecover = laserCooldown;
        laserCurrentTime = laserCooldown * 3;
    }

    void FireGuidedMissiles()
    {
        StartCoroutine(FiringMissiles());
        timeLeftToRecover = missileCooldown;
        missileCurrentTime = missileCooldown * 2;
    }

    IEnumerator FiringMissiles()
    {
        int missileAmount = 0;
        while(missileAmount < maxMissileAmount)
        {
            SpawnMissile();
            ++missileAmount;
            yield return new WaitForSeconds(1);
        }
    }

    void SpawnMissile()
    {
        GameObject missile = Instantiate(guidedMissilePrefab);
        missile.transform.localPosition = missileOrigin.position;
        missile.GetComponent<Missile>().SetTarget(m_target);
    }

    void Punch()
    {
        Collider2D[] collidersToPunch;
        collidersToPunch = Physics2D.OverlapBoxAll(transform.position, hitbox, 0);
        foreach(Collider2D collider in collidersToPunch)
        {
            if(collider.TryGetComponent(out Player player))
            {
                player.Hit();
            }
        }
        timeLeftToRecover = timeToRecoverFromPunch;
    }

    override protected void LowerCooldown()
    {
        if (laserCurrentTime > 0)
            laserCurrentTime -= Time.deltaTime;

        if (missileCurrentTime > 0)
            missileCurrentTime -= Time.deltaTime;
    }

    override protected void AttackMovement()
    {
        switch (currentAttack)
        {
            case BossAttackState.Dash:
                DashMovement();
                break;

            default:
                break;
        }
    }

    void DashMovement()
    {
        if(Vector2.Distance(transform.position, dashTargets[currentDashPosition]) < 1)
        {
            if (currentDashPosition < dashTargets.Length - 1) currentDashPosition++;
            else currentDashPosition = 0;

            timeLeftToRecover = timeToRecoverFromDash;
            return;
        }

        transform.position = Vector2.SmoothDamp(punchOrigin.position, dashTargets[currentDashPosition], ref velocity, smoothFactor, dashSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!currentAttack.Equals(BossAttackState.Dash))
            return;

        if(collision.transform.TryGetComponent(out Player player))
        {
            player.Hit();
        }
    }

    private void OnDrawGizmos()
    {
        GeneralGizmos();

        if (currentAttack.Equals(BossAttackState.Punch))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(punchOrigin.position, hitbox);

        }

        if (currentAttack.Equals(BossAttackState.Laser))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, laserHitbox);
        }
    }
}
