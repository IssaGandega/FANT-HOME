using System;
using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Room curRoom;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _enemiesMask;
    [SerializeField] private GameObject _proximityGhostObject;
    [SerializeField] public PlayerShooter playerShoot;
    [SerializeField] public FlashLight flashLight;
    public GameObject CanvasPlayer;
    public TextMeshProUGUI NumPlayer;
    public Image Arrow;
    public PlayerUI playerUI;
    
    private void Start()
    {
        curRoom?.PlayerEnter(this);
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius, _enemiesMask);
        if (colliders.Length < 1)
        {
            _proximityGhostObject.SetActive(false);
            return;
        }
        _proximityGhostObject.SetActive(true);
        
        CanvasPlayer.transform.LookAt(Camera.main.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
