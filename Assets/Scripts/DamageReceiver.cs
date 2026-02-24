//using UnityEngine;

//public class DamageReceiver : MonoBehaviour
//{
//    private HealthSystem health;
//    private PoiseSystem poise;
//    private PlayerController player;

//    private void Awake()
//    {
//        health = GetComponent<HealthSystem>();
//        poise = GetComponent<PoiseSystem>();
//        player = GetComponent<PlayerController>();
//    }

//    public void ReceiveDamage(float damage, float poiseDamage)
//    {
//        if (player != null && player.IsInvincible)
//            return;

//        health.TakeDamage(damage);

//        if (poise.ApplyPoiseDamage(poiseDamage))
//        {
//            if (player != null)
//                player.StateMachine.ChangeState(new HitState(player));
//        }
//    }
//}