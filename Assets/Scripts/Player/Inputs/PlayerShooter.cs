using UnityEngine;
using UnityEngine.InputSystem;

namespace Entities
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private WeaponsSO weapon;
        public WeaponsSO GetCurWeapon => weapon;
        [SerializeField] private bool _triggerShoot;
        [SerializeField] private FlashLight flashLight;

        private delegate void ShootAction();
        private ShootAction _shootAction;
        
        private GameObject _bullet;
        private GameObject _particle;
        private Bullet _bulletScript;
        private Vector3 _eulerAngles;
        private float _bulletSpeed;
        private int _bulletDamage;
        private float _reloadTime;
        private string _bulletKey;
        private bool _shootOrder;
        private float _lastShootTime;
        private float _bulletSpread;
        private float AOE_Range;

        private void Start()
        {
            ChangeWeapon(weapon);
        }

        public void ChangeWeapon(WeaponsSO newWeapon)
        {
            weapon = newWeapon;
            _bulletSpeed = weapon.bulletSpeed;
            _reloadTime = weapon.reloadTime;
            _bulletSpread = weapon.bulletSpread;
            _bulletDamage = weapon.damage;
            AOE_Range = weapon.AOE_Range;
            _bulletKey = weapon.key.ToString();
            _particle = weapon.particles;
            if (weapon.flashLight)
            {
                flashLight.SetEquip(true, weapon.flashLight);
            }
            else
            {
                flashLight.SetEquip(false, null);
            }

            _shootAction += ShootParticles;
            _shootAction += weapon.type switch
            {
                BulletTypes.Multiple => ShootMultiple,
                BulletTypes.Explosive => ShootExplosive,
                _ => Shoot
            };
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ShootParticles()
        {
            GameObject particles = Pooler.instance.Pop("VFX_"+_bulletKey+"Launch");
            particles.transform.position = transform.position + transform.forward;
            Pooler.instance.DelayedDepop(0.5f, "VFX_"+_bulletKey+"Launch", particles);
        }

        private void OnRotate(InputValue value)
        {
            if (!_triggerShoot && _lastShootTime + _reloadTime < Time.fixedTime)
            {
                CheckFire(value.Get<Vector2>());
            }
        }

        private void CheckFire(Vector2 value)
        {
            if (value == Vector2.zero) return;
            _lastShootTime = Time.fixedTime;
            _shootAction.Invoke();
        }

        public void Fire()
        {
            if (!_triggerShoot) return;
            _shootOrder = !_shootOrder;
            // if (_shootOrder)
            // {
            //     Debug.Log(_particle.name);
            //     _currentParticle = Pooler.instance.Pop(_particle.name);
            //     _currentParticle.transform.parent = transform;
            //     _currentParticle.transform.localPosition = Vector3.zero;
            //     _currentParticle.transform.localPosition += transform.forward;
            //     
            //     _currentParticle.transform.rotation = transform.rotation;
            // }
            // else
            // {
            //     Pooler.instance.Depop(_particle.name, _currentParticle);
            // }
        }

        private void Update()
        {
            if (_shootOrder && _lastShootTime + _reloadTime < Time.fixedTime)
            {
                _lastShootTime = Time.fixedTime;

                Shoot();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Shoot()
        {
            _eulerAngles = transform.eulerAngles;
            _eulerAngles.y += Random.Range(0.0f, _bulletSpread);
            AudioManager.Instance.PlaySFXRandom(_bulletKey + "_Shoot", 0.8f, 1.2f);
            SetBullet();
            _bulletScript.SetBase();
        }
        
        private void ShootExplosive()
        {
            _eulerAngles = transform.eulerAngles;
            AudioManager.Instance.PlaySFXRandom("GunShot", 0.8f, 1.2f);
            SetBullet();
            _bulletScript.SetExplosive();
        }

        private void ShootMultiple()
        {
            float totalAngle = weapon.bulletNumber %2 == 0 ? 30 : 45;
            AudioManager.Instance.PlaySFXRandom("GunShot", 0.8f, 1.2f);
            
            for (int i = 0; i < weapon.bulletNumber; i++)
            {
                _eulerAngles = transform.eulerAngles;
                _eulerAngles.y += -totalAngle / 2 + i*totalAngle/(weapon.bulletNumber-1);
                SetBullet();
                _bulletScript.SetMultiple();
            }
        }

        private void SetBullet()
        {
            _bullet = Pooler.instance.Pop(_bulletKey);
            _bulletScript = _bullet.GetComponent<Bullet>();
            _bulletScript.speed = _bulletSpeed;
            _bulletScript.key = _bulletKey;
            _bulletScript.AOE_Range = AOE_Range;
            _bulletScript.damage = _bulletDamage;
            _bullet.transform.eulerAngles = _eulerAngles;
            _bullet.transform.position = transform.position + transform.forward;
        }
    }
}