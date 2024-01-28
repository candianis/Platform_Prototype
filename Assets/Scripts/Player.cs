using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float Speed;
    [SerializeField] private float JumpForce;
    [SerializeField] private float RayDistance;
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private int Health = 5;
    [SerializeField] private bool Grounded;

    private Rigidbody2D Rigidbody2D;
    private float Horizontal;
    private float LastShoot;

    private void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
       
    }

    private void Update()
    {
        // Movimiento
        Horizontal = Input.GetAxisRaw("Horizontal");
        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // Detectar Suelo
        Debug.DrawRay(transform.position, Vector3.down * RayDistance, Color.red);
        if (Physics2D.Raycast(transform.position, Vector3.down, RayDistance))
        {
            Grounded = true;
        }
        else 
        {
            Grounded = false;
        }
        // Salto
        if (Input.GetButton("Jump") && Grounded)
        {
            Jump();
        }

        // Disparar
        if (Input.GetKey(KeyCode.F) && Time.time > LastShoot + 0.25f)
        {
            Shoot();
            LastShoot = Time.time;
        }
    }

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Rigidbody2D.velocity.y);
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector3.right;
        else direction = Vector3.left;

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.1f, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }

    public void Hit()
    {
        Health -= 1;
        if (Health == 0) Destroy(gameObject);
    }
}
