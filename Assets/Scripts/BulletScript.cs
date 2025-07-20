using UnityEngine;

public class BulletScript : MonoBehaviour
{
    
    
    private Rigidbody2D rb;
    public float speed = 10f;
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
