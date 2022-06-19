/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///<summary>
public class Weapon : MonoBehaviour {

    /* --- Variables --- */
    #region Variables
    
    // Projectile.
    [SerializeField] private Projectile m_Projectile;

    // UI Pieces.
    [SerializeField, ReadOnly] private Text m_WeaponName;
    [SerializeField, ReadOnly] private Text m_Description;
    [SerializeField, ReadOnly] private Text m_Cost;
    [SerializeField, ReadOnly] private Text m_Damage;
    [SerializeField, ReadOnly] private Text m_Cooldown;
    [SerializeField, ReadOnly] private Text m_Speed;
    
    #endregion

    /* --- Initialization --- */
    #region Initialization
    
    private void Init(Projectile projectile) {
        m_Projectile = projectile;
        m_WeaponName.text = m_Projectile.Name;
        m_Description.text = m_Projectile.Description;
        m_Cost.text = m_Projectile.Cost.ToString();
        m_Damage.text = m_Projectile.Damage.ToString();
        m_Cooldown.text = m_Projectile.Cooldown.ToString();
        m_Speed.text = m_Projectile.Speed.ToString();
    }
    
    #endregion

}