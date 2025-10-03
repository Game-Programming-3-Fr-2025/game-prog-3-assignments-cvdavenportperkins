using UnityEngine;

namespace PrototypeOne
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        public float smoothSpeed = 0.125f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                -10
            );

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
            else
                Debug.LogError("CameraFollow: Player not found!");
        }

        void Update()
        {
        }
    }
}