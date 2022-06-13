/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///
///<summary>
public class Armory : MonoBehaviour {

    public bool m_Active;

    [SerializeField] private Weapon m_WeaponBase;
    [SerializeField] private Projectile[] m_Projectiles;
    [SerializeField] private List<Weapon> m_Weapons = new List<Weapon>();

    /* --- Unity --- */
    #region Unity
    
    void Start() {
        CreateArmory();
        Hide();
    }
    
    void Update() {
    }
    
    void FixedUpdate() {
        float deltaTime = Time.fixedDeltaTime;
    }
    
    #endregion

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    private void CreateArmory() {
        for (int i = 0; i < m_Weapons.Count; i++) {
            CreateWeaponSlot(null, i);
        }
    }

    private void CreateWeaponSlot(Weapon m_Weapons, int index) {

        int n = 3;     
        int row = (int)Mathf.Floor(index / n);
        int col = index - (row * n);

        RectTransform prt = m_WeaponBase.transform.parent.GetComponent<RectTransform>();
        RectTransform rt = m_WeaponBase.GetComponent<RectTransform>();
        float buffer = prt.rect.width / n - rt.rect.width;
        // buffer *= 0.5f;

        Weapon newWeapon = Instantiate(m_WeaponBase.gameObject, Vector3.zero, Quaternion.identity, m_WeaponBase.transform.parent).GetComponent<Weapon>();
        RectTransform _rt = newWeapon.GetComponent<RectTransform>();
        _rt.anchoredPosition  = new Vector3(buffer / 2f + rt.rect.width / 2f + col * (rt.rect.width + buffer), - rt.rect.height / 2f - row * (rt.rect.height + 20f), 0f);
        newWeapon.gameObject.SetActive(true);
    }

}