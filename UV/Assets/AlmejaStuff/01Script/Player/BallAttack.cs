using UnityEngine;

public class BallAttack : MonoBehaviour
{
    [SerializeField] private GameObject ballDrop;
    [SerializeField] private Rigidbody2D ballRB;
    [SerializeField] private float speed;
    
    
    private void Awake()
    {
        gameObject.SetActive(false);
    }    
    

    public void TrowBall(Vector2 direction, Vector3 spawnPosition)
    {
        print("Pelota pateada");

        transform.position = spawnPosition;
        gameObject.SetActive(true);

        ballRB.velocity = direction.normalized * speed;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Enemy"))
        {
            Instantiate(ballDrop, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }

}
