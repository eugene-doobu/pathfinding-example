using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

namespace FTG.Character
{
    public enum AnimationState
    {
        IDLE,
        MOVE,
    }
    
    public class Character : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _moveThreshold = 0.1f;
        
        // TODO: Remove
        [SerializeField] private Transform _targetTest;
        
        private Transform _target;
        
        private AnimationState _animState;
        
        public Transform Target
        {
            get => _target;
            set
            {
                _target = value;
                LookAtTarget();
            }
        }
        
        public AnimationState AnimState
        {
            get => _animState;
            set
            {
                if (_animState == value) return;
                
                _animState = value;
                switch (_animState)
                {
                    case AnimationState.IDLE:
                        SkeletonAnim.AnimationName = "Idle";
                        break;
                    case AnimationState.MOVE:
                        SkeletonAnim.AnimationName = "Run";
                        break;
                }
                SkeletonAnim.loop = true;
                
                Debug.Log($"Animation: {_animState}");
            }
        }
        
        public SkeletonAnimation SkeletonAnim { get; private set; }

#region Mono
        private void Awake()
        {
            SkeletonAnim = GetComponent<SkeletonAnimation>();
            Target       = _targetTest;
            _animState   = AnimationState.IDLE;
        }
        
        private void Update()
        {
            SetAnimation();
            Move();
        }
#endregion

        private void SetAnimation()
        {
            if (_target == null) return;
            
            if (Vector3.Distance(transform.position, _target.position) > _moveThreshold)
            {
                AnimState = AnimationState.MOVE;
            }
            else
            {
                AnimState = AnimationState.IDLE;
            }
        }
        
        private void Move()
        {
            if (_target == null) return;

            if (Vector3.Distance(transform.position, _target.position) <= _moveThreshold)
            {
                transform.position = _target.position;
                return;
            }
            
            var dir = _target.position - transform.position;
            transform.position += dir.normalized * (_speed * Time.deltaTime);
        }
        
        private void LookAtTarget()
        {
            if (_target == null) return;
            
            Vector2 dir = _target.transform.position - transform.position;
            SkeletonAnim.Skeleton.ScaleX = dir.x > 0 ? -1 : 1;
        }
    }
}
