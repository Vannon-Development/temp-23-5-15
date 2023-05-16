using BCLib;
using BCLib.BT;
using BCLib.BT.Nodes2D;
using BCLib.BT.NodesInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Test
{
    public class Character : MonoBehaviour, IMotion, IAttack, IAITarget, IDamageable
    {
        [SerializeField] private float speed;
        [SerializeField] private TextAsset behavior;
        [SerializeField] private float attackTime;
        [SerializeField] private Projectile shotObject;
        [SerializeField] private float health;
        private BehaviorTree<Character> _tree;
        private Vector2 _moveStick;
        private Vector2 _fireStick;
        private Rigidbody2D _body;

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            _tree = new BehaviorTree<Character>(this);
            _tree.RegisterNode("CheckStick", typeof(StickActive<Character>));
            _tree.RegisterNode("Move", typeof(Motion<Character>));
            _tree.RegisterNode("CheckTimer", typeof(AttackTimer<Character>));
            _tree.RegisterNode("Attack", typeof(Attack<Character>));
            _tree.SetRoot(behavior.text);
        }

        private void FixedUpdate()
        {
            _body.angularVelocity = 0;
            _body.velocity = Vector2.zero;
            _tree.Tick();
        }

        private void OnMove(InputValue value)
        {
            _moveStick = value.Get<Vector2>();
        }

        private void OnFire(InputValue value)
        {
            _fireStick = value.Get<Vector2>();
        }

        public GameObject ParentObject => gameObject;
        public Vector2 GetAxis(string axisName)
        {
            if (axisName.Equals("move"))
                return _moveStick;
            if (axisName.Equals("fire"))
                return _fireStick;
            return Vector2.zero;
        }

        public bool GetButton(string buttonName)
        {
            return false;
        }

        public float AttackTime(string attackName) => attackTime;

        public void TriggerAttack(string attackName)
        {
            if (attackName == "primary")
            {
                var shot = Instantiate(shotObject);
                shot.Setup(transform.position, _fireStick.GetAngleDegrees());
            }
        }

        public float Velocity => speed;

        public Vector2 FrameVelocity
        {
            set => _body.velocity = value;
        }

        public string Group => "Player";
        public void Hit(float value)
        {
            health -= value;
            if (health <= 0)
                Destroy(gameObject);
        }
    }
}
