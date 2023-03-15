using BehaviorTree;
using UnityEngine;

namespace AI.GhostAI
{
    public class CheckStun : Node
    {
        private Transform _transform;
        
        public CheckStun(Transform transform)
        {
            _transform = transform;
        }
        
        public override NodeState Evaluate()
        {
            if (_transform.GetComponent<Ghost>().IsStun || _transform.GetComponent<Ghost>().IsAttacking)
            {
                _state = NodeState.RUNNING;
                return _state;
            }

            _state = NodeState.FAILURE;
            return _state;
        }
    }
}