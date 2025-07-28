using UnityEngine;
using System.Collections;

public class Chr_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    public float moveSpeed;

    public bool isMediumStress = false;
    public bool isHighStress = false;

    public bool freezed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (freezed)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            TryTriggerFreeze();
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // ACTUALIZA EL SPEED CON VELOCIDAD CORRECTA
        float speed = Mathf.Abs(rb.velocity.x);
        animator.SetFloat("Speed", speed);

        // Debug opcional:
        Debug.Log("Speed: " + speed);
    }

    void TryTriggerFreeze()
    {
        int chance = -1;

        if (isHighStress) chance = 5;
        else if (isMediumStress) chance = 10;

        if (chance > 0)
        {
            int roll = Random.Range(1, chance + 1);
            if (roll == 1)
            {
                StartCoroutine(FreezeForSeconds(5f));
            }
        }
    }

    IEnumerator FreezeForSeconds(float seconds)
    {
        freezed = true;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0f);
        yield return new WaitForSeconds(seconds);
        freezed = false;
    }
}
