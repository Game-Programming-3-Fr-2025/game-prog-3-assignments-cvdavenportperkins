using UnityEngine;

namespace PrototypeFour
{
    public class PlayerMovement : MonoBehaviour
    {

        public PlayerMovement player;
        public Rigidbody2D rb;
        public float moveSpeed = 5f;
        private Vector2 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>(); 
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

        void FixedUpdate()
        {
            
            rb.linearVelocity = moveInput * moveSpeed;
            
        }
    }


}
