using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Entities
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private WeaponsSO weapon;
        
        public WeaponsSO GetCurWeapon => weapon;
        [SerializeField] private bool _triggerShoot;
        [SerializeField] private FlashLight flashLight;
        [SerializeField] private Player player;

        [SerializeField] private GameObject baseCanon;
        [SerializeField] private GameObject explosiveCanon;
        [SerializeField] private GameObject lightCanon;
        [SerializeField] private GameObject zapCanon;
        [SerializeField] private GameObject lightHandLight;
        [SerializeField] private GameObject lightPhotos;
        [SerializeField] private GameObject lightTorchLamp;

        private delegate void ShootAction();
        private ShootAction _shootAction;
        
        private GameObject _bullet;
        private Mesh _tooth;
        private Bullet _bulletScript;
        private Vector3 _eulerAngles;
        private float _bulletSpeed;
        private int _bulletDamage;
        private float _reloadTime;
        private string _bulletKey;
        public bool ShootOrder;
        private float _lastShootTime;
        private float _bulletSpread;
        private float AOE_Range;

        

        private void Start()
        {
            if (!player) player = GetComponent<Player>();
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
            if (weapon.flashLight)
            {
                flashLight.SetEquip(true, weapon.flashLight);

                switch (weapon.flashLight.type)
                {
                    case FlashLightSO.LightType.PHOTO_LIGHT:
                        flashLight.curLight = lightPhotos.transform.GetChild(0).GetComponent<Light>();
                        break;
                    case FlashLightSO.LightType.HAND_LIGHT:
                        flashLight.curLight = lightTorchLamp.transform.GetChild(0).GetComponent<Light>();
                        break;
                    case FlashLightSO.LightType.PHARE_LIGHT:
                        flashLight.curLight = lightHandLight.transform.GetChild(0).GetComponent<Light>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                lightHandLight.SetActive(weapon.flashLight.type == FlashLightSO.LightType.PHARE_LIGHT);
                lightTorchLamp.SetActive(weapon.flashLight.type == FlashLightSO.LightType.HAND_LIGHT);
                lightPhotos.SetActive(weapon.flashLight.type == FlashLightSO.LightType.PHOTO_LIGHT);
                
                Debug.Log(weapon.flashLight.type);
            }
            else
            {
                flashLight.SetEquip(false, null);
                lightHandLight.SetActive(false);
                lightCanon.SetActive(false);
                lightPhotos.SetActive(false);
                Debug.Log("No light");
            }
            
            baseCanon.SetActive(weapon.type == BulletTypes.Classic);
            explosiveCanon.SetActive(weapon.type == BulletTypes.Explosive);
            zapCanon.SetActive(weapon.type == BulletTypes.Multiple);
            //lightCanon.SetActive(weapon.type == BulletTypes.Classic);
            
            _shootAction = weapon.type switch
            {
                BulletTypes.Multiple => ShootMultiple,
                BulletTypes.Explosive => ShootExplosive,
                _ => Shoot
            };
            _shootAction += ShootParticles;
            
            player.playerUI.UpdateWeaponUI(weapon);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ShootParticles()
        {
            if (flashLight.isActive) return;
            GameObject particles = Pooler.instance.Pop("VFX_"+_bulletKey+"Launch");
            particles.transform.position = transform.position + transform.forward;
            particles.transform.rotation = transform.rotation;
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
            ShootOrder = !ShootOrder;
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
            if (ShootOrder && _lastShootTime + _reloadTime < Time.fixedTime)
            {
                _lastShootTime = Time.fixedTime;

                _shootAction.Invoke();
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void Shoot()
        {
            if (flashLight.isActive) return;
            _eulerAngles = transform.eulerAngles;
            _eulerAngles.y += Random.Range(0.0f, _bulletSpread);
            AudioManager.Instance.PlaySFXRandom(_bulletKey + "_Shoot", 0.8f, 1.2f);
            SetBullet();
            _bulletScript.SetBase();
        }
        
        private void ShootExplosive()
        {
            _eulerAngles = transform.eulerAngles;
            AudioManager.Instance.PlaySFXRandom("BigBertha_Shoot", 0.8f, 1.2f);
            SetBullet();
            _bulletScript.SetExplosive();
        }

        private void ShootMultiple()
        {
            float totalAngle = weapon.bulletNumber %2 == 0 ? 30 : 45;
            
            AudioManager.Instance.PlaySFXRandom("TV_WhiteNoise", 0.8f, 1.2f);
            
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