using UnityEngine;

namespace PrototypeTwo
{

    public class PlayerMove : MonoBehaviour
    {
        private float moveInputX;
        private Rigidbody2D rb;
        public float moveSpeed = 6f;
        public float fallSpeed = 5f;
        //public float wallLimit = 6f;
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
            moveInputX = Input.GetAxisRaw("Horizontal");

            if (sr != null && moveInputX != 0)
                sr.flipX = moveInputX < 0;
        }

        private void FixedUpdate()
        {
            rb.linearVelocity = new Vector2(moveInputX * moveSpeed, -fallSpeed);

            //float clampedX = Mathf.Clamp(transform.position.x, -wallLimit, wallLimit);
            //transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

}
