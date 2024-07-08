using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float moveSpeed = 5f; // скорость движения персонажа
    public float jumpForce = 5f; // сила прыжка
    public float dashSpeed = 10f; // скорость ускорения
    public float dashTime = 1f; // время после ускорения
    private bool isGrounded; // проверка находится ли персонаж на земле
    private bool isDashing; // проверка, находится ли персонаж в состоянии ускорения
    public Animator anim;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        rb.MovePosition(transform.position + movement * Time.deltaTime * moveSpeed);

        // Устанавливаем значение "moveX" в единицу, если есть горизонтальное движение
        if (Mathf.Abs(movement.x) > 0.01f)
        {
            anim.SetFloat("moveX", 1f);
            // Отзеркаливаем персонажа по горизонтали в зависимости от направления
            if (movement.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (movement.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            anim.SetFloat("moveX", 0f);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing)
        {
            StartCoroutine(Dash(movement));
        }
    }

    IEnumerator Dash(Vector3 direction)
    {
        isDashing = true;
        rb.AddForce(direction.normalized * dashSpeed, ForceMode.Impulse);
        yield return new WaitForSeconds(dashTime); // длительность ускорения
        isDashing = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // Останавливаем вращение
        }
    }
}