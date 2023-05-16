using System.Collections.Generic;
using BCLib;
using BCLib.BT;
using BCLib.BT.Nodes2D;
using UnityEngine;

namespace Test
{
    public interface IFiringAI: ITargetingAI
    {
        public Vector2 PredictPosition { set; }
        public float ShotSpeed { get; }
    }

    public interface IMotionAI : ITargetingAI
    {
        public Vector2 MotionDirection { set; }
        public float Velocity { get; }
    }

    public class PredictPosition<T> : LeafNode<T> where T : class, IFiringAI
    {
        private Vector2 _prevPosition;
        private readonly float _variance;

        public PredictPosition(T context, float variance) : base(context)
        {
            _variance = variance;
        }
        public PredictPosition(T context, string[] param) : this(context, float.Parse(param[0])){}

        public override Status Tick()
        {
            var targetPos = (Vector2)Context.CurrentTarget.transform.position;
            var myPos = (Vector2)Context.ParentObject.transform.position;
            var dist = (targetPos - myPos).magnitude;
            var predictTime = dist / Context.ShotSpeed + Random.Range(-_variance, _variance);

            var motion = targetPos - _prevPosition;
            var predictDist = motion.magnitude * predictTime / Time.fixedDeltaTime;
            Context.PredictPosition = (targetPos + motion.normalized * predictDist - myPos).normalized;

            _prevPosition = targetPos;
            return Status.Success;
        }
    }

    public class IdealMotion<T> : LeafNode<T> where T : class, IMotionAI
    {
        private float _targetDist;

        public IdealMotion(T context, float dist, float variance) : base(context)
        {
            _targetDist = dist + Random.Range(-variance, variance);
        }
        public IdealMotion(T context, IReadOnlyList<string> param) : this(context, float.Parse(param[0]), float.Parse(param[1])) { }

        public override Status Tick()
        {
            var myPos = (Vector2)Context.ParentObject.transform.position;
            var targetPos = (Vector2)Context.CurrentTarget.transform.position;
            var dir = targetPos - myPos;
            var dist = dir.magnitude - _targetDist;
            var maxPerFrame = Context.Velocity * Time.fixedDeltaTime;
            var mod = Mathf.Min(1, Mathf.Abs(dist).Lerp(0, maxPerFrame, 0, 1));
            Context.MotionDirection = dir.normalized * (Mathf.Sign(dist) * mod);
            return Status.Success;
        }
    }
}