using BehaviorTree;
using UnityEngine;

namespace AI.PoltergeistAI
{
    public class CheckCooldownSpawn : Node
    {
        private float _spawnCounter;
        private float _spawnTime;

        public CheckCooldownSpawn(float spawnTime)
        {
            _spawnTime = spawnTime;
        }

        public override NodeState Evaluate()
        {
            _state = NodeState.FAILURE;
            return _state;
            
            _spawnCounter += Time.deltaTime;
            if (_spawnCounter >= _spawnTime)
            {
                _spawnCounter = 0;
                _state = NodeState.SUCCESS;
                return _state;
            }

            _state = NodeState.FAILURE;
            return _state;
        }
    }
}