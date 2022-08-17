/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Monet.UI;

namespace Monet.UI {

    ///<summary>
    ///
    ///<summary>
    public class UI : MonoBehaviour {

        /* --- Variables --- */
        #region Variables
        
        // Singleton.
        public static UI MainUI;

        // Button Settings.
        [SerializeField] protected Sprite m_ButtonBackgroundSprite;

        // Text Settings.
        [SerializeField] protected Font m_Font;
        [SerializeField] protected int m_FontSize;
        [SerializeField] protected Color m_FontColor;

        // Sounds.
        [SerializeField] public AudioClip ClickSound;
        
        #endregion

        // Runs once on instantiation.
        public virtual void Awake() {
            MainUI = this;
        }

        // Position an object in a list on the screen.
        public static Vector3 PositionOnScreen(int index, int count, bool isVertical) {
            float ratio = 0;
            if (count-1 > 0) {
                // Calculate the ratio.
                 ratio = (float)index / (count-1) - 0.5f;
            }

            
            // Calculate the position.
            float vertical = isVertical ? 1f : 0f;
            float horizontal = isVertical ? 0f : 1f;
            float y = vertical * ratio * -Screen.ScreenSize.y / 2f;
            float x = horizontal * ratio * Screen.ScreenSize.x / 4f;

            // Return the position.
            return new Vector3(x, y, 0f);

        }

        public static GameObject CreateClickable(string name, Vector3 position, Vector2 size) {
            // Create the gameObject.
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(MainUI.transform);
            obj.transform.localPosition = position;
            obj.transform.localScale = new Vector3(0.01f, 0.01f, 1f);

            // Add the box collider.
            obj.AddComponent<BoxCollider2D>();
            obj.GetComponent<BoxCollider2D>().size = size;
            obj.AddComponent<SpriteRenderer>();

            // Return the object.
            return obj;
        }

        public static void AddImage(GameObject obj, Sprite sprite, Vector2 size) {
            GameObject childObj = new GameObject("Background Image", typeof(Image));
            childObj.transform.SetParent(obj.transform);
            childObj.transform.localScale = new Vector3(1f, 1f, 1f);
            childObj.transform.localPosition = Vector3.zero;
            childObj.GetComponent<RectTransform>().sizeDelta = size;
            childObj.GetComponent<Image>().sprite = sprite;
            childObj.GetComponent<Image>().type = Image.Type.Sliced;
            childObj.GetComponent<Image>().pixelsPerUnitMultiplier = 3.5f;
            childObj.GetComponent<Image>().fillAmount = 1f;
        }

        public static void AddText(GameObject obj, string text, Vector2 size) {
            GameObject childObj = new GameObject(text + " Text", typeof(Text));
            childObj.transform.SetParent(obj.transform);
            childObj.transform.localScale = new Vector3(1f, 1f, 1f);
            childObj.transform.localPosition = Vector3.zero;
            childObj.GetComponent<RectTransform>().sizeDelta = size;
            childObj.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            childObj.GetComponent<Text>().text = text.ToUpper();
            childObj.GetComponent<Text>().font = MainUI.m_Font;
            childObj.GetComponent<Text>().fontSize = MainUI.m_FontSize;
            childObj.GetComponent<Text>().color = MainUI.m_FontColor;
        }

        public static void SetText(Text text) {
            text.alignment = TextAnchor.MiddleCenter;
            text.text = text.text.ToUpper();
            text.font = MainUI.m_Font;
            text.fontSize = MainUI.m_FontSize;
            text.color = MainUI.m_FontColor;
        }



    }
}