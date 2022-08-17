/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class GroundEffectManager : MonoBehaviour {

        [HideInInspector] private List<Rigidbody2D> m_Bodies = new List<Rigidbody2D>();

        void OnCollisionEnter2D(Collision2D collision) {
            Rigidbody2D body = collision.collider.GetComponent<Rigidbody2D>();
            if (body != null && !m_Bodies.Contains(body)) {
                SoundManager.PlaySound(Game.SoundManager.GroundImpactSound, 0.15f);
                m_Bodies.Add(body);
            }
        }

        void OnCollisionExit2D(Collision2D collision) {
            Rigidbody2D body = collision.collider.GetComponent<Rigidbody2D>();
            if (body != null && m_Bodies.Contains(body)) {
                m_Bodies.Remove(body);
            }
        }

        void Update() {
            m_Bodies = m_Bodies.FindAll(body => body != null);
            for (int i = 0; i < m_Bodies.Count; i++) {
                Flipbook flipbook = m_Bodies[i]?.GetComponent<Character>()?.CharacterFlipbook;
                if (flipbook != null) {
                    if (flipbook.ExtStepA) {
                        SoundManager.PlaySound(Game.SoundManager.GroundStepSoundA, flipbook.ExtStepVol);
                        flipbook.ExtStepA = false;
                        print("Step A");
                    }
                    if (flipbook.ExtStepB) {
                        // if (m_StepEffectB != null) { m_StepEffectB.Play(); }
                        SoundManager.PlaySound(Game.SoundManager.GroundStepSoundB, flipbook.ExtStepVol);
                        flipbook.ExtStepB = false;
                    }
                }
            }

        }

        // void OnCollisionStay2D(Collision2D collision) {
        //     Rigidbody2D body = collision.collider.GetComponent<Rigidbody2D>();
            // Flipbook flipbook = collision.collider.GetComponent<Rigidbody2D>();
            // if (flipbook != null) {
            //     if (flipbook.Step || flipbook.StepA) {
            //         float vA = Random.Range(0.025f, 0.05f);
            //         SoundManager.PlaySound(Game.SoundManager.GroundStepSoundA, vA);
            //     }
            //     if (flipbook.StepB) {
            //         float vB = Random.Range(0.02f, 0.03f);
            //         // if (m_StepEffectB != null) { m_StepEffectB.Play(); }
            //         SoundManager.PlaySound(Game.SoundManager.GroundStepSoundB, vB);
            //     }
            // }
        // }
        
    }
}