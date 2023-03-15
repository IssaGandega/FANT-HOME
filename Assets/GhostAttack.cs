using System;
using System.Collections;
using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;

    private Ghost _sender;
    private Vector3 _center;
    private Vector3 _scale;
    private Vector3 _direction;
    private Quaternion _rotation;
    private LayerMask _playerMask;
    private float _attackRange;
    private int _damage;
    private float _attackDelayBeforeAttack;

    public void Explode(Vector3 center, Vector3 scale, Vector3 direction, Quaternion rotation, int damage,
        float attackRange, float attackDelayBeforeAttack,
        Ghost sender, LayerMask playerMask)
    {
        _center = center;
        _scale = scale;
        _direction = direction;
        _rotation = rotation;
        _damage = damage;
        _playerMask = playerMask;
        _attackRange = attackRange;
        _attackDelayBeforeAttack = attackDelayBeforeAttack;
        _sender = sender;

        StartCoroutine(PrepareSpell());
    }

    private void OnParticleSystemStopped()
    {
        RaycastHit[] hits = Physics.BoxCastAll(_center, _scale / 2, _direction, Quaternion.identity, _attackRange,
            _playerMask);
        bool hitAPlayer = false;
        if (hits.Length > 0)
        {
            foreach (var hitCollider in hits)
            {
                hitCollider.transform.GetComponent<PlayerHealth>().GetHit(_damage);
                AudioManager.Instance.PlaySFXRandom("Ghost_Trash_Attack_Connect", 0.8f, 1.2f);
                hitAPlayer = true;
            }
        }

        if (_indicator) _indicator.SetActive(false);
        if (hitAPlayer) AudioManager.Instance.PlaySFXRandom("Ghost_Attack_Whiff", 0.8f, 1.2f);
        _sender.IsAttacking = false;
        Pooler.instance.Depop("TrashAttack", gameObject);
    }

    private IEnumerator PrepareSpell()
    {
        if (_indicator) _indicator.SetActive(true);
        yield return new WaitForSeconds(_attackDelayBeforeAttack);
        GetComponent<ParticleSystem>().Play();
        AudioManager.Instance.PlaySFXRandom("Ghost_Trash_Attack", 0.8f, 1.2f);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(_center, _direction, Color.red);
    }
}