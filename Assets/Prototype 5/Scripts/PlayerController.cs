using UnityEngine;

namespace PrototypeFive
{

    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        private Rigidbody2D rb;
        private Vector2 movement;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
        }

        void FixedUpdate()
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                GameManager.Instance.OnPlayerHit();
            }
        }
    }
}