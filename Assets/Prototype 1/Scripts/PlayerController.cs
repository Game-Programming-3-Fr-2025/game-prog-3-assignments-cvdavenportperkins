using UnityEngine;

namespace PrototypeOne
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;

        private Rigidbody2D rb;
        private Vector2 moveInput;

        private ShapeVisualController visualController;
        private SpriteRenderer sr;

        public FactionType faction;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            visualController = GetComponentInChildren<ShapeVisualController>();
            sr = GetComponent<SpriteRenderer>();
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

        public void UpdateVisuals()
        {
            if (visualController != null)
            {
                visualController.SetFactionVisual(faction);
            }
            else if (sr != null)
            {
                sr.color = FactionManager.GetColor(faction);
            }

        }

        private void LateUpdate()
        {
            if (GameManager.Instance != null)
                transform.position = GameManager.Instance.ClampToWorldBounds(transform.position, 0.2f);

        }
    }
}

