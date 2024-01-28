using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnemy : EnemyTemplate
{
    [Header("Point Settings")]
    [SerializeField, Tooltip("All the positions this enemy will move towards, they can be children of this object as their positions are changed to Vector2")] 
    Transform[] checkpoints;
    [SerializeField, Tooltip("Distance needed to move towards the next position")]
    protected float distance;
    //The current position this enemy is targeting
    int currentIndex;
    //All the positions this enemy will move towards gradually
    Vector2[] targetPositions;

    protected override void Prepare()
    {
        base.Prepare();
        currentIndex = 0;
        targetPositions = new Vector2[checkpoints.Length];
        for (int i = 0; i < checkpoints.Length; i++)
        {
            targetPositions[i] = new Vector2(checkpoints[i].position.x, checkpoints[i].position.y);
            checkpoints[i].gameObject.SetActive(false);
        }
    }

    override protected void Attack()
    {
        base.Attack();
    }

    override protected void Recover()
    {
        base.Recover();
    }

    protected override void CheckArrival()
    {
        float currentDistance = Vector2.Distance(transform.position, targetPositions[currentIndex]);
        if (currentDistance < distance)
        {
            if (currentIndex < targetPositions.Length - 1) currentIndex++;
            else currentIndex = 0;
        }
    }

    override protected void Patrol()
    {
        transform.position = Vector2.SmoothDamp(transform.position, targetPositions[currentIndex], ref velocity, smoothFactor, speed);
    }

    override protected void Pursuit()
    {
        transform.position = Vector2.SmoothDamp(transform.position, m_target.position, ref velocity, smoothFactor, speed);
    }
}
