using UnityEngine;
using System.Collections;

public class Chr_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    public float moveSpeed;

    // Stress levels (to be set from another script)
    public bool isMediumStress = false;
    public bool isHighStress = false;

    // Freeze state
    public bool freezed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (freezed)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            TryTriggerFreeze(); // Only try to freeze when movement key is pressed
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void TryTriggerFreeze()
    {
        int chance = -1;

        if (isHighStress)
        {
            chance = 5; // 1 in 5
        }
        else if (isMediumStress)
        {
            chance = 10; // 1 in 10
        }

        if (chance > 0)
        {
            int roll = Random.Range(1, chance + 1); // inclusive upper bound
            if (roll == 1)
            {
                StartCoroutine(FreezeForSeconds(5f));
            }
        }
    }

    IEnumerator FreezeForSeconds(float seconds)
    {
        freezed = true;
        yield return new WaitForSeconds(seconds);
        freezed = false;
    }
}
