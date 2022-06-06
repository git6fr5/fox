/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class Player : MonoBehaviour {

    [SerializeField, ReadOnly] private Level m_Level;
    public Level level => m_Level;

    [SerializeField, ReadOnly] private Checkpoint m_Checkpoint;
    public Checkpoint checkpoint => m_Checkpoint;

    public void SetLevel(Level level) {
        m_Level = level;
    }

    public void SetCheck(Checkpoint checkpoint) {
        m_Checkpoint = checkpoint;
    }

}
