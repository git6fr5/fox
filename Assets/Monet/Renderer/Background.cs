/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Background : MonoBehaviour {
        
        public float x;

        [SerializeField] private float m_ParrallaxScale;
        [SerializeField] private Color m_Hue;
        [SerializeField] private Color m_ShadowHue;

        public BackgroundLayer[] m_Layers;

        void Start() {

            List<BackgroundLayer> bkglayers = new List<BackgroundLayer>();
            foreach (Transform child in transform) {

                if (child.GetComponent<BackgroundLayer>()) {
                    bkglayers.Add(child.GetComponent<BackgroundLayer>());
                }
                
            }

            m_Layers = bkglayers.ToArray();

            for (int i = 0; i < m_Layers.Length; i++) {
                // float H; float S; float V;
                float x = 0.75f + 0.25f * m_Layers[i].Depth / 100f;
                Color c = Color.white * x; // m_Hue * x;
                c.a = 1f;
                //Color.HSVToRGB(m_Hue,, 0.5f + m_Layers[i].Depth / 100f);
                m_Layers[i].GetComponent<SpriteRenderer>().color = c;
            }

            // float x = Screen.ScreenSize.x / 2f;
            // List<BackgroundLayer> bkglayers = new List<BackgroundLayer>();

            for (int i = 0; i < m_Layers.Length; i++) {

                float x = m_Layers[i].Width;
                float y = m_Layers[i].Height; // Screen.ScreenSize.y / 2f;

                float theta = 60f; //  * ((-m_Layers[i].Depth / 100f) / 2f + 0.5f);
                if (!m_Layers[i].Perspective) {
                    theta = 0f;
                }
                float phi = 0f * (-m_Layers[i].Depth / 100f);
                Vector3 dv = (Quaternion.Euler(0f, 0f, theta) * new Vector3(x, y, 0f)) - new Vector3(x, y, 0f);
                float dx = 0f; // -x/2f * (1 - Mathf.Cos(phi / 180f * Mathf.PI));

                Transform leftArm = Instantiate(m_Layers[i].gameObject).transform;
                Transform rightArm = Instantiate(m_Layers[i].gameObject).transform;
                leftArm.SetParent(m_Layers[i].transform);
                rightArm.SetParent(m_Layers[i].transform);

                leftArm.localPosition = new Vector3(-x*2f - dv.x - dx, -dv.y, 0f);
                leftArm.eulerAngles = new Vector3(0f, -phi, theta);

                rightArm.localPosition = new Vector3(x*2f + dv.x + dx, -dv.y, 0f);
                rightArm.eulerAngles = new Vector3(0f, phi, -theta);

                SpriteRenderer layerRenderer = m_Layers[i].transform.GetComponent<SpriteRenderer>();
                SpriteRenderer leftArmRenderer = leftArm.GetComponent<SpriteRenderer>();
                SpriteRenderer rightArmRenderer = rightArm.GetComponent<SpriteRenderer>();
                SetArmSprite(layerRenderer, leftArmRenderer, m_Layers[i].Perspective);
                SetArmSprite(layerRenderer, rightArmRenderer, m_Layers[i].Perspective);
                // bkglayers.Add(leftArm.GetComponent<BackgroundLayer>());
                // bkglayers.Add(rightArm.GetComponent<BackgroundLayer>());

                if (m_Layers[i].Shadow) {
                    GameObject shadow = Instantiate(m_Layers[i].transform.gameObject, m_Layers[i].transform.position + Vector3.up * 0.25f, Quaternion.identity, transform);
                    shadow.GetComponent<SpriteRenderer>().color = m_ShadowHue;
                    shadow.GetComponent<SpriteRenderer>().sortingOrder -= 2;
                    foreach (Transform child in shadow.transform) {
                        SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                        if (renderer != null) {
                            renderer.color = new Color(0.25f, 0.25f, 0.25f, 0.25f);
                            renderer.sortingOrder -= 2;
                            // bkglayers.Add(child.GetComponent<BackgroundLayer>());
                        }
                    }
                    bkglayers.Add(shadow.GetComponent<BackgroundLayer>());
                }

            }

            m_Layers = bkglayers.ToArray();

        }

        private void SetArmSprite(SpriteRenderer layer, SpriteRenderer arm, bool perspective) {
            arm.sprite = layer.sprite;
            if (perspective) {
                arm.color = layer.color * 0.75f; // new Color(0.75f, 0.75f, 0.75f, 1f);
                arm.color = new Color(arm.color.r, arm.color.g, arm.color.b, layer.color.a);
            }
            arm.sortingLayerName = layer.sortingLayerName;
            arm.sortingOrder = layer.sortingOrder - 1;
        }

        void Update() {

            // x = -1 if on left side of screen.
            // x = 0 if in the middle of the screen.
            // x = 1 if on the right side of the screen.
            x = Game.MainPlayer.transform.position.x - Screen.MainCamera.transform.position.x;
            x /= Screen.ScreenSize.x;
            x *= 2f;

            for (int i = 0; i < m_Layers.Length; i++) {
                m_Layers[i].transform.localPosition = m_Layers[i].Origin + new Vector3(m_ParrallaxScale * x * (m_Layers[i].Depth / 100f), 0f, 50f);
            }

        }

    }
}