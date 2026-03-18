using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int damageAmount = 1;

    public int DamageAmount => damageAmount;
}