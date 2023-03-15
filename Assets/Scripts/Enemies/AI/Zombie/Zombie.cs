using System;
using System.Collections;
using System.Collections.Generic;
using Scriptables;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombie : MonoBehaviour
{
    public enum ZombieState
    {
        PATROL,
        POURSUE,
        ATTACKING
    }

    [SerializeField] private Vector3 target;
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField] private float detectionLength;
    [SerializeField] private LayerMask playerMask, obstaclesMask;
    [SerializeField] private ZombieState curState = ZombieState.PATROL;

    private bool _reachedTargetPos;
    private Ghost _ghost;
    private GhostStatsSO _stats;

    private int _attackDamage;
    private float _attackRange, _attackCD, _delayBeforeAttack;
    private Vector3 _attackScale;
    private bool _attackIsInCD;
    private string _attackKey;

    private void Start()
    {
        if (!navAgent) GetComponent<NavMeshAgent>();
        _ghost = GetComponent<Ghost>();
        _stats = _ghost._ghostSO;
        _attackDamage = _stats.AttackDamage;
        _attackRange = _stats.AttackRange;
        _attackScale = _stats.AttackScale;
        _attackCD = _stats.AttackCD;
        _delayBeforeAttack = _stats.AttackDelayBeforeAttack;
        _attackKey = _stats.AttackKey;

        SetRandomTarget();
    }

    void Update()
    {
        CheckNearestPlayer();

        float targetDist = Vector3.Distance(transform.position, target);

        switch (curState)
        {
            case ZombieState.PATROL:
                _reachedTargetPos = targetDist <= 0.7f;
                if (navAgent.path.status == NavMeshPathStatus.PathInvalid ||
                    navAgent.path.status == NavMeshPathStatus.PathPartial || _reachedTargetPos) SetRandomTarget();
                break;
            case ZombieState.POURSUE:
                if (targetDist < _attackRange && !_attackIsInCD) StartCoroutine(Attack());
                break;
            case ZombieState.ATTACKING:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }


        if (target != navAgent.destination) navAgent.SetDestination(target);
    }

    private void CheckNearestPlayer()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, detectionLength, Vector3.down, detectionLength,
            playerMask);

        if (hits.Length > 0)
        {
            bool newTarget = false;
            foreach (var hit in hits)
            {
                if (EnemyIsVisible(hit.transform))
                {
                    target = hit.transform.position;
                    newTarget = true;
                }
            }

            if (curState != ZombieState.ATTACKING && newTarget) curState = ZombieState.POURSUE;
            else if (curState != ZombieState.ATTACKING) curState = ZombieState.PATROL;
        }
    }


    private void SetRandomTarget()
    {
        _reachedTargetPos = false;


        Vector3 newPos = transform.position +
                         new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        NavMeshHit hit;
        NavMesh.SamplePosition(newPos, out hit, 5, 1);
        target = hit.position;
    }

    IEnumerator Attack()
    {
        curState = ZombieState.ATTACKING;
        navAgent.isStopped = true;

        GameObject ghost = Pooler.instance.Pop(_attackKey);
        ghost.transform.position = target;
        ghost.GetComponent<GhostAttack>().Explode(target, _attackScale, (target - transform.position),
            Quaternion.LookRotation(target - transform.position), _attackDamage, _attackRange, _delayBeforeAttack,
            transform.GetComponent<Ghost>(), playerMask);

        yield return new WaitForSeconds(_delayBeforeAttack);
        navAgent.isStopped = false;
        curState = ZombieState.PATROL;

        _attackIsInCD = true;
        yield return new WaitForSeconds(_attackCD);
        _attackIsInCD = false;
    }

    private bool EnemyIsVisible(Transform trans)
    {
        RaycastHit hit = new RaycastHit();
        if (Physics.Linecast(transform.position, trans.position, out hit, obstaclesMask))
        {
            return false;
        }

        return true;
    }
}