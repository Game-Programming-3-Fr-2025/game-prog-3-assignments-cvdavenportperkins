using UnityEngine;


namespace PrototypeOne
{

    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        public float smoothSpeed = 1f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, offset.z);

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
