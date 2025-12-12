using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    [SerializeField]
    private Animator playerAnimator;

    [SerializeField]
    private Equipment equipmentManager;

    [SerializeField]
    private Transform playerTransform;

    [SerializeField]
    private GameObject boat;

    [SerializeField]
    private UIManager uiManager;

    [SerializeField]
    private InteractBehavior interactBehavior;

    private bool isAttacking = false;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Vector3 attackOffset;

    public ItemData equippedWeapon;

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(transform.position + attackOffset, transform.forward * attackRange, Color.red);
        if (Input.GetMouseButtonDown(0) && CanAttack() && !equippedWeapon.isPortalGun)
        {
            isAttacking = true;
            DealDamage();
            //Lancer l'animation d'attaque
            playerAnimator.SetTrigger("Attack");
        }
        else if (Input.GetMouseButtonDown(0) && CanAttack() && equippedWeapon.isPortalGun)
        {
            isAttacking = true;
            //se téléporter au bateau de victoire
            playerTransform.position = boat.transform.position + new Vector3(0, 30, 0);
            Boat.instance.ShowWinScreen();
        }
    }

    public void AttackIsOver()
    {
        isAttacking = false;
    }

    public void DealDamage()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + attackOffset, transform.forward, out hit, attackRange, layerMask))
        {
            if (hit.transform.CompareTag("AI"))
            {
                EnemyAI enemyAI = hit.transform.GetComponent<EnemyAI>();
                if (enemyAI != null)    
                {
                    enemyAI.TakeDamage(equipmentManager.equippedWeaponItem.weaponDamage); // Example damage value    
                }
            }
        }
    }

    public bool CanAttack()
    {
        return equipmentManager.equippedWeaponItem != null && !isAttacking && !uiManager.isUIActive && !interactBehavior.isBusy;
    }
}
