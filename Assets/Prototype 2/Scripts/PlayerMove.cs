using UnityEngine;

namespace PrototypeTwo
{

    public class PlayerMove : MonoBehaviour
    {
        private Vector2 moveInput;
        private Rigidbody2D rb;
        public float moveSpeed = 5f;
        private SpriteRenderer sr;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize();
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

}
