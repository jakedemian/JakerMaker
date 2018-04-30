using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour {
    Rigidbody2D rb;

    private Vector2 gravity = new Vector2(0, -9.8f);
    public float maxFallSpeed = -10f;
    public bool enableGravity = true;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update () {
        if (enableGravity) {
            if(rb.velocity.y > maxFallSpeed) {
                rb.AddForce(gravity);
            }
        }
	}

    public void doEnableGravity() {
        enableGravity = true;
    }

    public void doDisableGravity() {
        enableGravity = false;
        rb.velocity = new Vector2(rb.velocity.x, 0f);
    }
}
