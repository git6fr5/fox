/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    [System.Serializable]
    public class ActionPattern {

        [System.Serializable]
        public class Action {

            [SerializeField] private string m_Name;

            [SerializeField] private float m_Duration;
            [SerializeField, ReadOnly] private float m_Ticks;
            public float Ticks => m_Ticks;

            [SerializeField] protected Vector2 m_Direction;
            public Vector2 Direction => m_Direction;
            [SerializeField] protected bool m_MoveTowardsPlayer;
            public bool MoveTowardsPlayer => m_MoveTowardsPlayer;
            [SerializeField] protected float m_Range;
            public float Range => m_Range;
            [SerializeField] protected bool m_MaintainDirection;
            public bool MaintainDirection => m_MaintainDirection;

            [SerializeField] protected bool m_Jump;
            public bool Jump => m_Jump;
            [SerializeField] protected bool m_HoldJump;
            public bool HoldJump => m_HoldJump;

            [SerializeField] protected bool m_Attack;
            public bool Attack => m_Attack;
            [SerializeField] protected bool m_AttackTowardsPlayer;
            public bool AttackTowardsPlayer => m_AttackTowardsPlayer;
            [SerializeField] protected bool m_AttackOnlyHorizontal;
            public bool AttackOnlyHorizontal => m_AttackOnlyHorizontal;
            [SerializeField] protected Vector2 m_AttackDirection;
            public Vector2 AttackDirection => m_AttackDirection;
            [SerializeField] protected bool m_Dash;
            public bool Dash => m_Dash;
            [SerializeField] protected bool m_DashTowardsPlayer;
            public bool DashTowardsPlayer => m_DashTowardsPlayer;
            [SerializeField] protected bool m_DashOnlyHorizontal;
            public bool DashOnlyHorizontal => m_DashOnlyHorizontal;
            [SerializeField] protected Vector2 m_DashDirection;
            public Vector2 DashDirection => m_DashDirection;

            public void OnStart() {
                Timer.CountdownTicks(ref m_Ticks, true, m_Duration, 0f);
            }

            public void OnUpdate(float deltaTime) {
                Timer.CountdownTicks(ref m_Ticks, false, m_Duration, deltaTime);
            }

        }

        [SerializeField] private Action[] m_Pattern;
        [SerializeField, ReadOnly] private int m_Index;

        public void OnStart() {
            if (m_Pattern == null || m_Pattern.Length == 0) {
                return;
            }

            m_Index = 0;
            m_Pattern[0].OnStart();
        }

        public Action OnUpdate(float deltaTime) {
            if (m_Pattern == null || m_Pattern.Length == 0) {
                return null;
            }

            m_Pattern[m_Index].OnUpdate(deltaTime);
            if (m_Pattern[m_Index].Ticks == 0f) {
                m_Index = (m_Index + 1) % m_Pattern.Length;
                m_Pattern[m_Index].OnStart();
            }
            return m_Pattern[m_Index];
        }


    }

}