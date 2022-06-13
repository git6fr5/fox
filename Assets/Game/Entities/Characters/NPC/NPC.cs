using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class NPC : MonoBehaviour {
    
    void Start() {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    public virtual void Interact() {
    }

}
