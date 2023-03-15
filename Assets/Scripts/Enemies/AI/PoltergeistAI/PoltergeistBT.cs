using System.Collections.Generic;
using AI.GhostAI;
using BehaviorTree;
using Scriptables;
using UnityEngine;
using Tree = BehaviorTree.Tree;

namespace AI.PoltergeistAI
{
    public class PoltergeistBT : Tree
    {
        [SerializeField] private LayerMask _enemiesMask;
        [SerializeField] private LayerMask _playerMask;

        private PoltergeistStatsSO _poltergeistStatsSO;
        private Room[] _roomWaypoints;

        protected override Node SetupTree()
        {
            _poltergeistStatsSO = (PoltergeistStatsSO)GetComponent<Ghost>()._ghostSO;
            _roomWaypoints = RoomsManager.Instance.rooms;

            Node root = new Selector(new List<Node>
            {
                new CheckStun(transform),
                new Sequence(new List<Node>
                {
                    new CheckFleeing(transform),
                    new TaskFleeing(transform, _roomWaypoints, _poltergeistStatsSO.MoveSpeed)
                }),
                new Sequence(new List<Node>
                {
                    new CheckCooldownSpawn(_poltergeistStatsSO.UnitMakingCD),
                    new TaskSpawnGhosts(transform, _poltergeistStatsSO.NbrUnitsToSpawn,
                        _poltergeistStatsSO.KeyGhostToSpawn)
                }),
                new Sequence(new List<Node>
                {
                    new CheckPlayerInAttackRange(transform, _poltergeistStatsSO.AttackRange,
                        _poltergeistStatsSO.AttackCD),
                    new TaskAttack(transform, _poltergeistStatsSO.AttackDamage, _poltergeistStatsSO.AttackScale,
                        _poltergeistStatsSO.AttackKey, _poltergeistStatsSO.AttackDelayBeforeAttack,
                        _poltergeistStatsSO.AttackRange, _playerMask),
                }),
                new Sequence(new List<Node>
                {
                    new CheckInteractable(transform, _poltergeistStatsSO.InteractionCD,
                        _poltergeistStatsSO.InteractionRange),
                    new TaskInteractable(transform, _poltergeistStatsSO.MoveSpeed,
                        _poltergeistStatsSO.CanActivateObject, _poltergeistStatsSO.InteractableKey)
                }),
                new Sequence(new List<Node>
                {
                    new CheckPlayerInFOVRange(transform, _poltergeistStatsSO.DetectionRange),
                    new TaskGoToTarget(transform, GetComponent<MeshRenderer>(), _enemiesMask,
                        _poltergeistStatsSO.MoveSpeed,
                        _poltergeistStatsSO.AttackRange, _poltergeistStatsSO.RangeVisibleToPlayer,
                        _poltergeistStatsSO.MaxHealth, _poltergeistStatsSO.MaxVeil),
                }),
                new TaskPatrol(transform, _roomWaypoints, _poltergeistStatsSO.MoveSpeed),
            });

            return root;
        }
    }
}