﻿using BehaviorTree;
using UnityEngine;

namespace AI.GhostAI
{
    public class TaskAttack : Node
    {
        private Animator _animator;
        private Transform _transform;
        private PlayerHealth _playerHealth;
        private int _damage;
        private Vector3 _attackScale;
        private string _attackKey;
        private float _attackDelayBeforeAttack;
        private float _attackRange;
        private LayerMask _playerMask;

        public TaskAttack(Transform transform, int damage, Vector3 attackScale, string attackKey,
            float attackDelayBeforeAttack, float attackRange, LayerMask playerMask)
        {
            _transform = transform;
            _animator = transform.GetComponent<Animator>();
            _damage = damage;
            _attackScale = attackScale;
            _attackKey = attackKey;
            _attackDelayBeforeAttack = attackDelayBeforeAttack;
            _attackRange = attackRange;
            _playerMask = playerMask;
        }

        public override NodeState Evaluate()
        {
            Transform target = (Transform)GetData("target");

            if (target.GetComponent<PlayerHealth>().curHealth <= 0)
            {
                ClearData("target");
                // _animator.SetBool("Attacking", false);
                // _animator.SetBool("Walking", true);
            }
            else
            {
                _transform.LookAt(target.position);
                GameObject attackGhost = Pooler.instance.Pop(_attackKey);
                attackGhost.transform.position = _transform.position;
                _transform.GetComponent<Ghost>().IsAttacking = true;
                attackGhost.GetComponent<GhostAttack>().Explode(
                    (_transform.position + (target.position - _transform.position) / 2), _attackScale,
                    target.position - _transform.position,
                    Quaternion.LookRotation(target.position - _transform.position), _damage, _attackRange, _attackDelayBeforeAttack,
                    _transform.GetComponent<Ghost>(), _playerMask);
                _transform.GetComponent<Ghost>().IsFleeing = true;
            }

            _state = NodeState.SUCCESS;
            return _state;
        }
    }
}