using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IHitable
{
    public enum PlayerState
    {
        BASE,
        DOWN,
        INVINCIBLE
    }

    [SerializeField] public int curHealth;
    [SerializeField] private PlayerState curState;

    [SerializeField] private GameObject deathInteractionGO;
    [SerializeField] private Player player;
    private void Awake()
    {
        curState = PlayerState.BASE;
        if (!player) player = GetComponent<Player>();
    }
    
    [ContextMenu("TakeHits")]
    private void TestHit()
    {
        GetHit(3);
    }

    public void GetHit(int damage)
    {
        if (curState == PlayerState.INVINCIBLE || curHealth <= 0) return;

        curHealth -= damage;
        player.playerUI.UpdateHealthUI(curHealth);
        Debug.Log("got hit");
        if (curHealth <= 0) Fall();
        else StartCoroutine(Invincible());
        AudioManager.Instance.PlaySFXRandom("Player_Damage", 0.8f, 1.2f);
    }

    private IEnumerator Invincible()
    {
        curState = PlayerState.INVINCIBLE;
        yield return new WaitForSeconds(1);
        curState = PlayerState.BASE;
    }

    private void Fall()
    {
        curState = PlayerState.DOWN;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
        deathInteractionGO.SetActive(true);
    }

    public void GetUp()
    {
        curState = PlayerState.BASE;
        GetComponent<PlayerController>().enabled = true;
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
        deathInteractionGO.SetActive(false);
        AudioManager.Instance.PlaySFXRandom("Player_Revive", 0.8f, 1.2f);
    }
}