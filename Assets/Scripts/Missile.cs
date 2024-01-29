using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MissileState
{
    Pursuit,
    Prepare
}

public class Missile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float lifeTime;
    [SerializeField] Transform target;

    // Update is called once per frame
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, target.position, step);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
            Destroy(this.gameObject);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.TryGetComponent(out Player player))
        {
            player.Hit();
            Destroy(this.gameObject);
        }
    }
}
