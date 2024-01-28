using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerceptionBehaviours : MonoBehaviour
{
    [Header("References"), Space()]
    [SerializeField] protected Transform farPercept;
    [SerializeField] protected Transform closePercept;

    [Space(), Header("Status"), Space()]
    [SerializeField] protected bool hasFarPerception;
    [SerializeField] protected bool hasClosePerception;
    [SerializeField] protected float farPerceptRadious, closePerceptRadious;
    [SerializeField] protected AgentStates agentState;
    [SerializeField] protected string playerTag = "";
    [SerializeField] protected float slowingRadious;
    [SerializeField] protected float threshold;
    protected Rigidbody rb;
    protected Collider[] eyesPerceibed, earsPerceibed;
    [SerializeField] protected bool isFar = false;
    [SerializeField] protected bool isClose = false;


    // Start is called before the first frame update
    protected virtual void Start()
    {
     
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        PerceptionManager();
        DecisionManager();
    }

    protected virtual void FixedUpdate()
    {
        eyesPerceibed = Physics.OverlapSphere(farPercept.position, farPerceptRadious);
        earsPerceibed = Physics.OverlapSphere(closePercept.position, closePerceptRadious);

    }

    protected virtual void PerceptionManager()
    {
        //basicAgent.m_target = null;

        if (hasFarPerception)
        {
            isFar = PerceptEnemy(eyesPerceibed);

        }
        if (hasClosePerception)
        {
            isClose = PerceptEnemy(earsPerceibed);
        }
    }

    protected virtual void DecisionManager()
    {

    }

    protected virtual void ActionManager()
    {

    }

    protected virtual void MovementManager()
    {

    }


    /// <summary>
    /// Bool that returns the collider of the enemy that enters the Aegnts perception
    /// </summary>
    protected virtual bool PerceptEnemy(Collider[] perceibed)
    {
        if (perceibed == null) return false;

        for (int i = 0; i < perceibed.Length; i++)
        {

                {
                    var cur = perceibed[i];
                    if (cur.gameObject.GetComponent<Player>())
                    {
                        var player = cur.gameObject.GetComponent<Player>();
                    }
                }   
            
        }

        return false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(farPercept.position, farPerceptRadious);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(closePercept.position, closePerceptRadious);
    }

    protected enum AgentStates
    {
        None,
        Attack,
        Escape,
        Wander,
        FollowingPath,
        FollowingLeader
    }
}


