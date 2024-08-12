using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalAnimationHashes
{
#region EnemyHashes

    public static readonly int EnemyAnim_AlarmAllEnemies = Animator.StringToHash("AlarmAllEnemies");
    public static readonly int EnemyAnim_Shoot = Animator.StringToHash("Shoot");
    public static readonly int EnemyAnim_AlarmRecieved = Animator.StringToHash("AlarmRecieved");
    public static readonly int EnemyAnim_OpenFieldAttack = Animator.StringToHash("OpenFieldAttack");
    public static readonly int EnemyAnim_Backstab = Animator.StringToHash("Backstab");
    public static readonly int EnemyAnim_Combat = Animator.StringToHash("Combat");
    public static readonly int EnemyAnim_Headshot = Animator.StringToHash("Headshot");
    public static readonly int EnemyAnim_Frontstab = Animator.StringToHash("Frontstab");
    public static readonly int EnemyAnim_Delay = Animator.StringToHash("Delay");
    public static readonly int EnemyAnim_Speed = Animator.StringToHash("Speed");
    public static readonly int EnemyAnim_Patrol = Animator.StringToHash("Patrol");
    public static readonly int EnemyAnim_PatrolSpeed = Animator.StringToHash("PatrolSpeed");
    public static readonly int EnemyAnim_Turn = Animator.StringToHash("Turn");
    public static readonly int EnemyAnim_TurnSpeed = Animator.StringToHash("TurnSpeed");
    public static readonly int EnemyAnim_Reload = Animator.StringToHash("Reload");

#endregion

#region PlayerHashes

    public static readonly int PlayerAnim_Aim = Animator.StringToHash("Aim");
    public static readonly int PlayerAnim_Reload = Animator.StringToHash("Reload");
    public static readonly int PlayerAnim_Backstab = Animator.StringToHash("Backstab");
    public static readonly int PlayerAnim_Frontstab = Animator.StringToHash("Frontstab");
    public static readonly int PlayerAnim_EquipMeele = Animator.StringToHash("EquipMeele");
    public static readonly int PlayerAnim_UnequipMeele = Animator.StringToHash("UnequipMeele");
    public static readonly int PlayerAnim_Speed = Animator.StringToHash("Speed"); 
    public static readonly int PlayerAnim_Death = Animator.StringToHash("Death");


#endregion

#region Objective

    public static readonly int Objective_TurnOff = Animator.StringToHash("turnOff");

#endregion

#region UI

    public static readonly int UI_HideHintAnim = Animator.StringToHash("HideHint");
    public static readonly int UI_ShowHintAnim = Animator.StringToHash("ShowHint");
           
    public static readonly int UI_MainWeaponEquip = Animator.StringToHash("MainWeaponEquip");
    public static readonly int UI_SwordEquip = Animator.StringToHash("SwordEquip");
    
    public static readonly int UI_PickupAmmo = Animator.StringToHash("AddAmmo");

    #endregion

}
