using System.Collections;
using Scriptables;
using Unity.Collections;
using UnityEngine;

public class Ghost : MonoBehaviour, IEnemy
{
    [ReadOnly] public Room CurRoom;

    [Header("Stats")] 
    public GhostStatsSO _ghostSO;
    [SerializeField] private string _name = "Ghost";
    [SerializeField] private GameObject _stunObject;

    [HideInInspector] public bool IsStun;
    [HideInInspector] public bool IsFleeing;

    [Header("Stats in Runtime")] 
    private float _health;
    [HideInInspector] public float Veil;
    private float _regenVeilPoints;
    private float _regenVeilCD;
    private float _regenVeilOverTime;
    private float _stunTime;

    private MeshRenderer _meshRenderer;

    private bool _isVulnerable;
    private bool _canBeStun;
    private float _stunCounter;
    private float _veilCounter;

    private Color _colorVeil;
    private Color _colorHealth;

    private Coroutine _regenCO;

    private void Start()
    {
        gameObject.name = _name;
        _health = _ghostSO.MaxHealth;
        Veil = _ghostSO.MaxVeil;
        _regenVeilPoints = _ghostSO.VeilRegenPoints;
        _regenVeilCD = _ghostSO.VeilRegenCD;
        _regenVeilOverTime = _ghostSO.VeilRegenOverTime;
        _stunTime = _ghostSO.StunDuration;
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        _colorVeil = _meshRenderer.material.color;
    }

    private void Update()
    {
        if (IsStun)
        {
            _stunCounter += Time.deltaTime;
            if (_stunCounter >= _stunTime)
            {
                _stunObject.SetActive(false);
                IsStun = false;
            }
        }

        if (!_isVulnerable) return;
        _veilCounter += Time.deltaTime;
        if (!(_veilCounter >= _regenVeilCD)) return;
        _regenCO = StartCoroutine(RegenVeil());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Bullet") || !_isVulnerable) return;
        if (_regenCO != null) StopCoroutine(_regenCO);
        _veilCounter = 0;
        if (other.gameObject.GetComponent<Bullet>().isPhysic)
        {
            other.gameObject.GetComponent<Bullet>().Contact();
        }
        TakeDamage(other.gameObject.GetComponent<Bullet>().damage);
    }

    public void TakeVeil(float damageVeil)
    {
        Veil -= damageVeil;
        if (Veil < _ghostSO.MaxHealth) _meshRenderer.enabled = true;
        if (Veil <= 0)
        {
            Veil = 0;
            _isVulnerable = true;
        }
        var alpha = 1 - (Veil / _ghostSO.MaxVeil);
        _colorVeil.a = alpha;
        _meshRenderer.material.color = _colorVeil;
        if (!_isVulnerable) return;
        if (_ghostSO.AlwaysStun)
        {
            _stunCounter = 0;
            IsStun = true;
            _stunObject.SetActive(true);
        }
        else if (!_canBeStun)
        {
            _canBeStun = true;
            _stunCounter = 0;
            IsStun = true;
            _stunObject.SetActive(true);
        }
        if (_regenCO != null) StopCoroutine(_regenCO);
        IsFleeing = true;
        Veil = 0;
        _veilCounter = 0;
        AudioManager.Instance.PlaySFXRandom("Ghost_Revealed", 0.8f, 1.2f);
    }

    public void TakeDamage(float damage)
    {
        if (_health <= 0 || !_isVulnerable) return;
        _health -= damage;
        _colorHealth = Color.Lerp(Color.red, Color.green, _health / _ghostSO.MaxHealth);
        _colorHealth.a = 0.8f;
        _meshRenderer.material.color = _colorHealth;
        if (_health <= 0)
        {
            Pooler.instance.Depop(_ghostSO.Key.ToString(), gameObject);
            AudioManager.Instance.PlaySFXRandom(_ghostSO.Death_SFX, 0.8f, 1.2f);
            return;
        }

        AudioManager.Instance.PlaySFXRandom(_ghostSO.Damage_SFX, 0.8f, 1.2f);
    }

    private IEnumerator RegenVeil()
    {
        float alpha;
        while (Veil <= _ghostSO.MaxVeil)
        {
            Veil += _regenVeilPoints;
            _isVulnerable = false;
            IsFleeing = false;
            _canBeStun = false;
            alpha = 1 - (Veil / _ghostSO.MaxVeil);
            _colorVeil.a = alpha;
            _meshRenderer.material.color = _colorVeil;
            yield return new WaitForSeconds(_regenVeilOverTime);
        }

        alpha = 1 - (Veil / _ghostSO.MaxVeil);
        _colorVeil.a = alpha;
        _meshRenderer.material.color = _colorVeil;
        if (Veil >= _ghostSO.MaxHealth) _meshRenderer.enabled = false;
    }
}