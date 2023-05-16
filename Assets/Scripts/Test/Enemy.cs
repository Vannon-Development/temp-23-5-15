using System.Collections.Generic;
using BCLib;
using BCLib.BT;
using BCLib.BT.Nodes2D;
using BCLib.BT.NodesInput;
using UnityEngine;

namespace Test
{
    public class Enemy : MonoBehaviour, IAttack, IMotion, IAITarget, IFiringAI, IMotionAI, IDamageable
    {
        [SerializeField] private float speed;
        [SerializeField] private TextAsset behavior;
        [SerializeField] private TextAsset aiBehavior;
        [SerializeField] private float attackTime;
        [SerializeField] private Projectile shotObject;
        [SerializeField] private float health;
        private BehaviorTree<Enemy> _tree;
        private BehaviorTree<Enemy> _ai;
        private Vector2 _moveStick;
        private Vector2 _fireStick;
        private Rigidbody2D _body;
        private float _adjustedAttackTime;

        private readonly List<GameObject> _targets = new ();
        private GameObject _target;
        private readonly string[] _filter = { "Enemy" };

        private void Start()
        {
            _body = GetComponent<Rigidbody2D>();
            _tree = new BehaviorTree<Enemy>(this);
            _tree.RegisterNode("CheckStick", typeof(StickActive<Enemy>));
            _tree.RegisterNode("Move", typeof(Motion<Enemy>));
            _tree.RegisterNode("CheckTimer", typeof(AttackTimer<Enemy>));
            _tree.RegisterNode("Attack", typeof(Attack<Enemy>));
            _tree.SetRoot(behavior.text);

            _ai = new BehaviorTree<Enemy>(this);
            _ai.RegisterNode("HasTarget", typeof(HasTarget<Enemy>));
            _ai.RegisterNode("FindAllTargets", typeof(FindAllTargets<Enemy>));
            _ai.RegisterNode("SelectNearestTarget", typeof(SelectNearestTarget<Enemy>));
            _ai.RegisterNode("Predict", typeof(PredictPosition<Enemy>));
            _ai.RegisterNode("Move", typeof(IdealMotion<Enemy>));
            _ai.SetRoot(aiBehavior.text);
        }

        private void FixedUpdate()
        {
            _body.velocity = Vector2.zero;
            _body.angularVelocity = 0;
            _moveStick = Vector2.zero;
            _fireStick = Vector2.zero;
            _ai.Tick();
            _tree.Tick();
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

        public float AttackTime(string attackName) => _adjustedAttackTime;

        public void TriggerAttack(string attackName)
        {
            if (attackName == "primary")
            {
                var shot = Instantiate(shotObject);
                shot.Setup(transform.position, _fireStick.GetAngleDegrees());
                _adjustedAttackTime = attackTime + Random.Range(0, 1.2f);
            }
        }

        public float Velocity => speed;

        public Vector2 FrameVelocity
        {
            set => _body.velocity = value;
        }

        public string Group => "Enemy";
        public string[] FilterGroups => _filter;
        public List<GameObject> AllTargets => _targets;

        public GameObject CurrentTarget
        {
            get => _target;
            set => _target = value;
        }

        public Vector2 PredictPosition
        {
            set => _fireStick = value;
        }
        public float ShotSpeed => shotObject.Speed;
        public Vector2 MotionDirection { set => _moveStick = value; }
        public void Hit(float value)
        {
            health -= value;
            if(health <= 0)
                Destroy(gameObject);
        }
    }
}
