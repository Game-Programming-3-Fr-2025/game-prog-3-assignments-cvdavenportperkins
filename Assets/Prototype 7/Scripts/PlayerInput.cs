using UnityEngine;

namespace PrototypeSeven
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;

        private Rigidbody2D rb;
        private Vector2 moveInput;
                
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
           
        }

        void Update()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
        }

        void FixedUpdate()
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
                
        private void LateUpdate()
        {
            if (GameManager.Instance != null)
                transform.position = GameManager.Instance.ClampToWorldBounds(transform.position, 0.2f);

        }
    }
}

