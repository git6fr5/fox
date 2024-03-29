/* --- Libraries --- */
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(CompositeCollider2D))]
public class ShadowCaster2DTileMap : MonoBehaviour {

    /* --- Switches --- */
    public bool selfShadows = true;
    public bool generate = false;

    /* --- Components --- */
    private CompositeCollider2D tilemapCollider;
    
    /* --- Static --- */
    static readonly FieldInfo meshField = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly FieldInfo shapePathField = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    static readonly MethodInfo generateShadowMeshMethod = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D)
                                    .Assembly
                                    .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
                                    .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    private static MethodInfo onEnableMethod = typeof(UnityEngine.Rendering.Universal.ShadowCaster2D).GetMethod("OnEnable", BindingFlags.NonPublic | BindingFlags.Instance);

    /* --- Properties --- */
    public Vector2 center;

    void Update() {
        if (generate) {
            Generate();
            generate = false;
        }
    }

    /* --- Methods --- */
    public void Generate(float delay) {

        StartCoroutine(IEGenerate(delay));

        IEnumerator IEGenerate(float delay) {
            yield return new WaitForSeconds(delay);
            Generate();
            yield return null;
        }

    }

    private void Generate() {
        DestroyAllChildren();

        tilemapCollider = GetComponent<CompositeCollider2D>();

        for (int i = 0; i < tilemapCollider.pathCount; i++) {
            Vector2[] pathVertices = new Vector2[tilemapCollider.GetPathPointCount(i)];
            tilemapCollider.GetPath(i, pathVertices);

            //center = Vector2.zero;
            //for (int j = 0; j < pathVertices.Length; j++) {
            //    center += pathVertices[j] / pathVertices.Length;
            //}

            //for (int j = 0; j < pathVertices.Length; j++) {
            //    pathVertices[j] = (pathVertices[j] - center) * (7f / 8f) + center;
            //}

            for (int j = 0; j < pathVertices.Length; j++) {

                // Find the center on the grid for the vertex.
                

                pathVertices[j] = pathVertices[j] + Vector2.down * (2f / 8f);
            }

            GameObject shadowCaster = new GameObject("shadow_caster_" + i);
            shadowCaster.transform.parent = gameObject.transform;
            UnityEngine.Rendering.Universal.ShadowCaster2D shadowCasterComponent = shadowCaster.AddComponent<UnityEngine.Rendering.Universal.ShadowCaster2D>();
            shadowCasterComponent.selfShadows = this.selfShadows;

            Vector3[] testPath = new Vector3[pathVertices.Length];
            for (int j = 0; j < pathVertices.Length; j++) {
                testPath[j] = pathVertices[j];
            }

            shapePathField.SetValue(shadowCasterComponent, testPath);
            // meshField.SetValue(shadowCasterComponent, new UnityEngine.Mesh());
            // generateShadowMeshMethod.Invoke(shadowCasterComponent, new object[] { meshField.GetValue(shadowCasterComponent), shapePathField.GetValue(shadowCasterComponent) });

            // shapePathField.SetValue(shadowCaster, positions);
            meshField.SetValue(shadowCasterComponent, null);
            onEnableMethod.Invoke(shadowCasterComponent, new object[0]);

        }

        // Debug.Log("Generate");

    }
    public void DestroyAllChildren() {

        var tempList = transform.Cast<Transform>().ToList();
        foreach (var child in tempList) {
            DestroyImmediate(child.gameObject);
        }

    }

    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(center, 1f);
    }

}
