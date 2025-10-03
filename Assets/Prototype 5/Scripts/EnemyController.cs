using NUnit.Framework;
using UnityEngine;

namespace PrototypeFive
{

    public class EnemyController : MonoBehaviour
    {
        public float moveSpeed = 2f;
        private Transform player;

        void Start()
        {
            player = GameManager.Instance.player;
        }

        void Update()
        {
            if (player == null) return;

            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }

        void OnEnable()
        {
            GameManager.Instance.RegisterEnemy(this);
        }

        void OnDisable()
        {
            GameManager.Instance.UnregisterEnemy(this); 
        }



    }
}