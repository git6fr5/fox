using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Utilities;

namespace Platformer.Utilities {

    public class Timer {

        #region Fields

        // The default reset value of the timer.
        [SerializeField] 
        private float m_MaxValue = 0f;
        public float MaxValue => m_MaxValue;

        // The current value of the timer.
        [SerializeField] 
        private float m_Value = 0f;
        public float Value => m_Value;

        // Whether this timer is currently not zero.
        public bool Active => m_Value > 0f;

        #endregion

        #region Methods

        public Timer(float initialValue, float maxValue) {
            m_MaxValue = maxValue;
            m_Value = initialValue;
        }

        // Starts the timer to its max value.
        public void Start(float startValue = -1f) {
            m_Value = startValue == -1f ? m_MaxValue : startValue;  
        }

        // Starts the timer to its max value if the predicate is fulfilled.
        public void StartIf(bool predicate, float startValue = -1f) {
            if (predicate) {  
                m_Value = startValue == -1f ? m_MaxValue : startValue;  
            }
        }

        // Stops the timer (resets it to 0).
        public void Stop() {
            m_Value = 0;
        }

        // Ticks the timer down by the given interval.
        public bool TickDown(float dt) {
            bool wasnotzero = m_Value > 0f;
            bool isnowzero = false;
            m_Value -= dt;
            if (m_Value <= 0f) {
                m_Value = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Ticks the timer down by the given interval if the predicate is fulfilled.
        public bool TickDownIf(float dt, bool p) {
            bool wasnotzero = m_Value > 0f;
            bool isnowzero = false;
            if (p) { m_Value -= dt; }
            if (m_Value <= 0f) {
                m_Value = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Ticks the timer down by the given interval if the predicate is fulfilled.
        public bool TickDownIfElseReset(float dt, bool p) {
            bool wasnotzero = m_Value > 0f;
            bool isnowzero = false;
            if (p) { m_Value -= dt; }
            else { m_Value = m_MaxValue; }
            if (m_Value <= 0f) {
                m_Value = 0f;
                isnowzero = true;
            }
            return wasnotzero && isnowzero;
        }

        // Rename this to triangle ticks
        public bool TriangleTickDownIf(float dt, bool p) {
            bool wasnotzero = m_Value != 0f;
            if (p) { m_Value += dt; }
            else { m_Value -= dt; }
            if (m_Value >= m_MaxValue) { m_Value = m_MaxValue; }
            if (m_Value < 0f) { m_Value = 0f; }
            bool isnowzero = m_Value == 0f;
            return wasnotzero && isnowzero;
        }

        // Ticks the timer up to a specified maximum by the given interval.
        public bool TickUp(float dt) {
            bool wasnotmax = m_Value < m_MaxValue;
            bool isnowmax = false;
            m_Value += dt;
            if (m_Value >= m_MaxValue) {
                m_Value = m_MaxValue;
                isnowmax = true;
            }
            return wasnotmax && isnowmax;
        }

        // Ticks the timer up by the given interval  a specified maximum if the predicate is fulfilled.
        public bool TickUpIf(float dt, bool p) {
            bool wasnotmax = m_Value < m_MaxValue;
            bool isnowmax = false;
            if (p) { m_Value += dt; }
            if (m_Value >= m_MaxValue) {
                m_Value = m_MaxValue;
                isnowmax = true;
            }
            return wasnotmax && isnowmax;
        }

        public bool Cycle(float dt) {
            m_Value += dt;
            if (m_Value > 2f * m_MaxValue) {
                m_Value -= 2f * m_MaxValue;
            }
            return m_Value > m_MaxValue;
        }

        #endregion

    }

}



