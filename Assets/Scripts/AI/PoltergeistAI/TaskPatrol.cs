﻿using AI.GhostAI;
using BehaviorTree;
using UnityEngine;

namespace AI.PoltergeistAI
{
    public class TaskPatrol : Node
    {
        private Transform _transform;
        private Animator _animator;
        private Transform[] _waypoints;

        private int _currentWaypointIndex = 0;

        private float _waitTime = 1f;
        private float _waitCounter = 0f;
        private bool _waiting = false;

        public TaskPatrol(Transform transform, Transform[] waypoints)
        {
            _transform = transform;
            _animator = transform.GetComponent<Animator>();
            _waypoints = waypoints;
        }

        public override NodeState Evaluate()
        {
            if (_waiting)
            {
                _waitCounter += Time.deltaTime;
                if (_waitCounter >= _waitTime)
                {
                    _waiting = false;
                    // _animator.SetBool("Walking", true);
                }
            }
            else
            {
                Transform wp = _waypoints[_currentWaypointIndex];
                if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
                {
                    _transform.position = wp.position;
                    _waitCounter = 0f;
                    _waiting = true;

                    _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                    // _animator.SetBool("Walking", false);
                }
                else
                {
                    _transform.position =
                        Vector3.MoveTowards(_transform.position, wp.position, GhostBT.Speed * Time.deltaTime);
                    _transform.LookAt(wp.position);
                }
            }

            _state = NodeState.RUNNING;
            return _state;
        }
    }
}