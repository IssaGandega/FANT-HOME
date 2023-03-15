using UnityEngine;

namespace Scriptables
{
    public enum MonsterType
    {
        GHOST,
        POLTERGEIST,
        ECTOPLASMA
    }

    [CreateAssetMenu(fileName = "new GhostStatsSO", menuName = "Scriptables/GhostsSO/GhostStatsSO", order = 2)]
    public class GhostStatsSO: ScriptableObject
    {
        public int MaxHealth;
        public float MaxVeil;
        public float VeilRegenPoints;
        public float VeilRegenCD;
        public float VeilRegenOverTime;
        public float RangeVisibleToPlayer;
        public float MoveSpeed;
        public float StunDuration;
        public bool AlwaysStun;
        public int AttackDamage;
        public float AttackRange;
        public Vector3 AttackScale;
        public float AttackCD;
        public float AttackDelayBeforeAttack;
        public bool CanActivateObject;
        public int InteractionRange;
        public int InteractionCD;
        public string AttackKey;
        public string DeathKey;
        public string InteractableKey;
        public MonsterType Key;
        public string Attack_SFX, Connect_SFX, Damage_SFX, Death_SFX;
    }   
}