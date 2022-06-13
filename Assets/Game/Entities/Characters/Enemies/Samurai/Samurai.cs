using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Timer = Enemy.Timer;

public class Samurai : Enemy {

    public Timer m_WaitingAtPosition = new Timer(1f);
    public Timer m_DashTimer = new Timer(0.25f);
    public Timer m_AggroTimer = new Timer(1f);
    public Timer m_JumpTimer = new Timer(5f);

    public override void GetMove() {

        if (Active) {
            float playerDirection = Mathf.Sign((player.transform.position - transform.position).x);
            float distance = (player.transform.position - transform.position).x;

            if (Mathf.Abs(distance) > m_AttackRange) {
                m_Direction = playerDirection * Vector2.right;
            }
            else if (m_Controller.State.Direction != playerDirection) {
                m_Direction = playerDirection * Vector2.right;
            }
            else {
                m_Direction = Vector2.zero;
            }
            return;
        }

        m_WaitingAtPosition.CheckFinished();
        if (m_WaitingAtPosition.Active) {
            m_Direction = new Vector2(0f, 0f);
            return;
        }

        Vector2 displacement = (Vector2)(m_Path[m_PathIndex] - transform.position);
        if (Mathf.Abs(displacement.x) < GameRules.MovementPrecision) {
            print("reached");
            m_Direction = new Vector2(0f, 0f);
            m_PathIndex = (m_PathIndex + 1) % m_Path.Length;
            m_WaitingAtPosition.Start();
        }
        else {
            m_Direction = new Vector2(Mathf.Sign(displacement.x), 0f);
        }

    }

    public override void  GetAttack() {

        if (Active) {
            m_AggroTimer.Continue();
            m_Attack = m_AggroTimer.CheckFinished(false);

            Vector2 playerPosition = player.transform.position;
            Vector2 playerDisplacement = (Vector2)playerPosition - (Vector2)transform.position;
            m_AttackDirection = playerDisplacement.normalized;
            
        }
        else {
            m_Attack = false;
            m_AggroTimer.Stop();
        }

    }

    public override void  GetJump() {
        m_Jump = false;
        m_Float = true;
        if (Active && m_AggroTimer.CheckFinished(false) && m_JumpTimer.CheckFinished()) {
            float y = (player.transform.position - transform.position).y;
            if (y > 1f) {
                m_Jump = true;
                m_JumpTimer.Start();
            }
        }

    }

    public override void  GetDash() {
        m_DashTimer.CheckFinished();
        if (!Active || m_DashTimer.Active) {
            m_Dash = false;
            return;
        }

        if (m_Direction != Vector2.zero) {
            m_Dash = true;
            m_DashTimer.Start();
        }

    }

    public override void Timers(float deltaTime) {
        m_WaitingAtPosition.Update(deltaTime);
        m_DashTimer.Update(deltaTime);
        m_AggroTimer.Update(deltaTime);
        m_JumpTimer.Update(deltaTime);
    }

}
