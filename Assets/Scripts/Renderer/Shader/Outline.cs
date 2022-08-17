/* --- Libraries --- */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Tilemaps;
using Monet;

namespace Monet {

    ///<summary>
    ///
    ///<summary>
    public class Outline : MonoBehaviour {

        public static void Add(SpriteRenderer renderer, float width, float ppu) {
            // if (width == 0f) { return; }

            List<Material> outlines = new List<Material>();
            Material right = Add(renderer, 1f, 0, width / ppu);
            Material left = Add(renderer, -1f, 0, width / ppu);
            Material up = Add(renderer, 0, 1f, width / ppu);
            Material down = Add(renderer, 0, -1f, width / ppu);

            outlines.Add(right);
            outlines.Add(left);
            outlines.Add(up);
            outlines.Add(down);

            for (int i = 0; i < renderer.materials.Length; i++) {
                outlines.Add(renderer.materials[i]);
            }
            renderer.materials = outlines.ToArray();
        }

        public static Material Add(SpriteRenderer renderer, float x, float y, float width) {
            Material outline = new Material(Shader.Find("Custom/OutlineShader"));
            outline.SetFloat("_OutlineX", x);
            outline.SetFloat("_OutlineY", y);
            outline.SetFloat("_OutlineWidth", width);
            return outline;
        }

        public static void Set(SpriteRenderer renderer, Color color) {
            for (int i = 0; i < renderer.materials.Length; i++) {
                bool isOutline = renderer.materials[i].shader == Shader.Find("Custom/OutlineShader");
                if (isOutline) {
                    renderer.materials[i].SetColor("_OutlineColor", color);
                }
            }
        }

        public static void Add(SpriteShapeRenderer renderer, float width, float ppu) {
            List<Material> outlines = new List<Material>();
            Material right = Add(renderer, 1f, 0, width / ppu);
            Material left = Add(renderer, -1f, 0, width / ppu);
            Material up = Add(renderer, 0, 1f, width / ppu);
            Material down = Add(renderer, 0, -1f, width / ppu);

            outlines.Add(right);
            outlines.Add(left);
            outlines.Add(up);
            outlines.Add(down);

            for (int i = 0; i < renderer.materials.Length; i++) {
                outlines.Add(renderer.materials[i]);
            }
            renderer.materials = outlines.ToArray();
        }

        public static Material Add(SpriteShapeRenderer renderer, float x, float y, float width) {
            Material outline = new Material(Shader.Find("Custom/OutlineShader"));
            outline.SetFloat("_OutlineX", x);
            outline.SetFloat("_OutlineY", y);
            outline.SetFloat("_OutlineWidth", width);
            return outline;
        }

        public static void Set(SpriteShapeRenderer renderer, Color color) {
            for (int i = 0; i < renderer.materials.Length; i++) {
                bool isOutline = renderer.materials[i].shader == Shader.Find("Custom/OutlineShader");
                if (isOutline) {
                    renderer.materials[i].SetColor("_OutlineColor", color);
                }
            }
        }

        public static void Add(TilemapRenderer renderer, float width, float ppu) {
            List<Material> outlines = new List<Material>();
            Material right = Add(renderer, 1f, 0, width / ppu);
            Material left = Add(renderer, -1f, 0, width / ppu);
            Material up = Add(renderer, 0, 1f, width / ppu);
            Material down = Add(renderer, 0, -1f, width / ppu);

            for (int i = 0; i < renderer.materials.Length; i++) {
                outlines.Add(renderer.materials[i]);
            }
            outlines.Add(right);
            outlines.Add(left);
            outlines.Add(up);
            outlines.Add(down);

            renderer.materials = outlines.ToArray();
        }

        public static Material Add(TilemapRenderer renderer, float x, float y, float width) {
            Material outline = new Material(Shader.Find("Custom/OutlineShader"));
            outline.SetFloat("_OutlineX", x);
            outline.SetFloat("_OutlineY", y);
            outline.SetFloat("_OutlineWidth", width);
            return outline;
        }

        public static void Set(TilemapRenderer renderer, Color color) {
            for (int i = 0; i < renderer.materials.Length; i++) {
                bool isOutline = renderer.materials[i].shader == Shader.Find("Custom/OutlineShader");
                if (isOutline) {
                    renderer.materials[i].SetColor("_OutlineColor", color);
                }
            }
        }

    }
}