using System.Collections.Generic;
using AI.PoltergeistAI;
using BehaviorTree;
using Scriptables;
using UnityEngine;
using Tree = BehaviorTree.Tree;

namespace AI.GhostAI
{
    public class GhostBT : Tree
    {
        [SerializeField] private LayerMask _enemiesMask;
        [SerializeField] private LayerMask _playerMask;

        private GhostStatsSO _ghostStatsSO;

        protected override Node SetupTree()
        {
            _ghostStatsSO = GetComponent<Ghost>()._ghostSO;
            Node root = new Selector(new List<Node>
            {
                new CheckStun(transform),
                new Sequence(new List<Node>
                {
                    new CheckPlayerInAttackRange(transform, _ghostStatsSO.AttackRange, _ghostStatsSO.AttackCD),
                    new TaskAttack(transform, _ghostStatsSO.AttackDamage, _ghostStatsSO.AttackScale,
                        _ghostStatsSO.AttackKey, _ghostStatsSO.AttackDelayBeforeAttack, _ghostStatsSO.AttackRange,
                        _playerMask),
                }),
                new Sequence(new List<Node>
                {
                    new CheckInteractable(transform, _ghostStatsSO.InteractionCD, _ghostStatsSO.InteractionRange),
                    new TaskInteractable(transform, _ghostStatsSO.MoveSpeed, _ghostStatsSO.CanActivateObject, _ghostStatsSO.InteractableKey)
                }),
                new Sequence(new List<Node>
                {
                    new CheckPlayer(transform),
                    new TaskGoToTarget(transform, GetComponent<MeshRenderer>(), _enemiesMask, _ghostStatsSO.MoveSpeed,
                        _ghostStatsSO.AttackRange, _ghostStatsSO.RangeVisibleToPlayer, _ghostStatsSO.MaxHealth,
                        _ghostStatsSO.MaxVeil),
                })
            });

            return root;
        }
    }
}