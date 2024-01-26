using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Speed;
 
    private Rigidbody2D Rigidbody2D;
    private Vector3 Direction;
    private float time;
    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = Direction * Speed;
        time+= Time.deltaTime;

        if (time > 2)
        {
            DestroyBullet();
        }
    }

    public void SetDirection(Vector3 direction)
    {
        Direction = direction;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //GruntScript grunt = other.GetComponent<GruntScript>();
        Player john = other.GetComponent<Player>();
        //if (grunt != null)
        //{
        //    grunt.Hit();
        //}
        if (john != null)
        {
            john.Hit();
        }
        DestroyBullet();
    }
}
