using System.Collections;
using UnityEngine;

public class GhostAttack : MonoBehaviour
{
    [SerializeField] private GameObject _indicator;

    private Ghost _sender;
    private Vector3 _center;
    private Vector3 _scale;
    private Vector3 _direction;
    private LayerMask _playerMask;
    private float _attackRange;
    private int _damage;
    private float _attackDelayBeforeAttack;
    private string _attackKey;

    public void Explode(Vector3 center, Vector3 scale, Vector3 direction, int damage,
        float attackRange, float attackDelayBeforeAttack,
        Ghost sender, LayerMask playerMask, string attackKey)
    {
        _center = center;
        _scale = scale;
        _direction = direction;
        _damage = damage;
        _playerMask = playerMask;
        _attackRange = attackRange;
        _attackDelayBeforeAttack = attackDelayBeforeAttack;
        _attackKey = attackKey;
        _sender = sender;

        StartCoroutine(PrepareSpell());
    }

    private void OnParticleSystemStopped()
    {
        _sender.IsAttacking = false;
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
        Pooler.instance.Depop(_attackKey, gameObject);
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
        //Draw a Ray forward from GameObject toward the maximum distance
        Gizmos.DrawRay(transform.position, _direction.normalized * _attackRange);
        //Draw a cube at the maximum distance
        Gizmos.DrawWireCube(transform.position + _direction.normalized * _attackRange, _scale);
    }
}