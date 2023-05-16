using UnityEngine;

namespace Test
{
    public interface IDamageable
    {
        public void Hit(float value);
    }
    
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float damage;
        [SerializeField] private float range;
        private Rigidbody2D _body;
        private Vector2 _direction;

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            Invoke(nameof(Remove), range / speed);
        }

        public void Setup(Vector2 position, float rotation)
        {
            transform.SetPositionAndRotation(position, Quaternion.Euler(0, 0, rotation));
            _direction = (transform.localToWorldMatrix * Vector2.right).normalized;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            var dmg = other.gameObject.GetComponent<IDamageable>();
            dmg?.Hit(damage);
            Remove();
        }

        private void Remove()
        {
            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            _body.velocity = _direction * speed;
        }

        public float Speed => speed;
    }
}
