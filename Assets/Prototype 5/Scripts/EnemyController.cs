using NUnit.Framework;
using UnityEngine;

namespace PrototypeFive
{

    public class EnemyController : MonoBehaviour
    {

        public Transform enemyPrefab;
        public BoxCollider2D boxCollider;
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

        void Awake()
        {
            Debug.Log(GameManager.Instance);
            GameManager.Instance.RegisterEnemy(this);

        }

        void OnDisable()
        {
            GameManager.Instance.UnregisterEnemy(this); 
        }



    }
}