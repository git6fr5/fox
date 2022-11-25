/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monet;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    public class UIComponent : MonoBehaviour {

        // Mouse Position.
        public static Vector2 MousePosition => Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        
        // Components.
        protected SpriteRenderer UIRenderer => GetComponent<SpriteRenderer>();
        protected BoxCollider2D UICollider => GetComponent<BoxCollider2D>();
        protected LineRenderer UILineRenderer => GetComponent<LineRenderer>();

        void Awake() {
            gameObject.layer = LayerMask.NameToLayer("UI");
        }

        public static bool IsAnInputFieldFocused() {
            InputField[] inputFields = (InputField[])GameObject.FindObjectsOfType(typeof(InputField));
            for (int i = 0; i < inputFields.Length; i++) {
                if (inputFields[i].isFocused) {
                    return true;
                }
            }
            return false;
        }

        // Fades this renderer.
        protected void Fade(float ratio) {
            UIRenderer.material.SetColor("_AddColor", (1f - ratio) * new Color(0f, 0f, 0f, 1f));
        }

        // Scales this up.
        protected void Scale(float ratio, bool reset) {
            Vector3 id = new Vector3(1f, 1f, 1f) ;
            transform.localScale = reset ? id : ratio * id;
        }

        // Highlights the button.
        protected void Highlight(bool highlight) {
            float outlineWidth = highlight ? 0.05f : 0f;
            UIRenderer.material.SetFloat("_OutlineWidth", outlineWidth);
        }

        // Rotates the button.
        protected void Rotate(float deltaTime, float speed, bool reset) {
            Vector3 deltaRotation = Vector3.forward * speed * deltaTime;
            transform.eulerAngles = reset ? Vector3.zero : transform.eulerAngles + deltaRotation;
        }

        protected static void Tick(ref float ticks, float duration, float deltaTime, bool condition, bool zero) {
            if (condition) {
                ticks = ticks >= duration ? duration : ticks + deltaTime;
            }
            else {
                ticks = zero || ticks <= 0 ? 0 : ticks - deltaTime;
            }
        }

    }
}