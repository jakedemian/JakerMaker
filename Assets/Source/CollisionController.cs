using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * CollisionController
 * 
 * Custom collision handling, controls what happens when a player,
 * collides with a collidable object
 */
public class CollisionController : MonoBehaviour {
    public bool drawSpriteBounds = false;
    public bool drawCollisionRaycasts = false;
    public Vector2 boundsScale = new Vector2(1f, 1f);
    public Vector2 boundsOffset = new Vector2(0f, 0f);
    public float raycastLength = 1f;


    CollisionBounds bounds;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    GravityController gravity;

    // START
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        gravity = GetComponent<GravityController>();

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        bounds = new CollisionBounds(spriteBounds, gameObject, boundsScale, boundsOffset);
	}

    // FIXED UPDATE
    private void FixedUpdate() {
        if (rb.velocity.y < 0f) {
            RaycastHit2D blHit = Physics2D.Raycast(bounds.getBottomLeftWorldPoint(), -Vector2.up, raycastLength);
            RaycastHit2D brHit = Physics2D.Raycast(bounds.getBottomRightWorldPoint(), -Vector2.up, raycastLength);

            if (blHit.collider != null || brHit.collider != null) {
                // collision below has occurred
                gravity.doDisableGravity();
                transform.position = new Vector2(0f, blHit.point.y + bounds.getDefaultBoundsObject().extents.y - boundsOffset.y);
            }
        }
    }

    // ON DRAW GIZMOS
    private void OnDrawGizmos() {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.green;

        if (drawSpriteBounds) {
            doDrawSpriteBounds(bounds.getTopLeftWorldPoint(), bounds.getTopRightWorldPoint(), bounds.getBottomLeftWorldPoint(), bounds.getBottomRightWorldPoint());
        }

        if (drawCollisionRaycasts) {
            doDrawCollisionRaycasts(bounds.getTopLeftWorldPoint(), bounds.getTopRightWorldPoint(), bounds.getBottomLeftWorldPoint(), bounds.getBottomRightWorldPoint());
        }
    }

    // draw this object's sprite bounds after our custom modifications
    private void doDrawSpriteBounds(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        Gizmos.DrawLine(bounds.getTopLeftWorldPoint(), bounds.getTopRightWorldPoint());
        Gizmos.DrawLine(bounds.getTopLeftWorldPoint(), bounds.getBottomLeftWorldPoint());
        Gizmos.DrawLine(bounds.getBottomRightWorldPoint(), bounds.getTopRightWorldPoint());
        Gizmos.DrawLine(bounds.getBottomRightWorldPoint(), bounds.getBottomLeftWorldPoint());
    }

    // draw this object's collision raycasts
    private void doDrawCollisionRaycasts(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br) {
        Gizmos.DrawLine(bounds.getBottomLeftWorldPoint(), bounds.getBottomLeftWorldPoint() + (-Vector2.up * raycastLength));
        Gizmos.DrawLine(bounds.getBottomRightWorldPoint(), bounds.getBottomRightWorldPoint() + (-Vector2.up * raycastLength));
    }
}

/**
 * CollisionBounds
 * 
 * A customized version of the Bounds class with additional functionality.
 * Cannot inherit from Bounds so we just keep a Bounds object accessable.
 */
public class CollisionBounds {
    Bounds bounds;
    GameObject target;

    Vector2 topLeftRelative;
    Vector2 topRightRelative;
    Vector2 botLeftRelative;
    Vector2 botRightRelative;

    Vector2 boundsOffsetFromRealPoint;

    public CollisionBounds(Bounds b, GameObject go, Vector2 scale, Vector2 offset) {
        bounds = b;
        bounds.extents = new Vector3(scale.x * bounds.extents.x, scale.y * bounds.extents.y);
        bounds.center += new Vector3(offset.x, offset.y);

        calculateRelativePoints();

        target = go;
        boundsOffsetFromRealPoint = offset;
    }

    private void calculateRelativePoints() {
        topLeftRelative = bounds.min + new Vector3(0f, bounds.size.y);
        topRightRelative = bounds.max;
        botLeftRelative = bounds.min;
        botRightRelative = bounds.min + new Vector3(bounds.size.x, 0f);
    }

    public Vector2 getTargetWorldPoint() {
        return target.transform.position;
    }

    public Vector2 getTopLeftWorldPoint() {
        return getTargetWorldPoint() + topLeftRelative;
    }

    public Vector2 getTopRightWorldPoint() {
        return getTargetWorldPoint() + topRightRelative;
    }

    public Vector2 getBottomLeftWorldPoint() {
        return getTargetWorldPoint() + botLeftRelative;
    }

    public Vector2 getBottomRightWorldPoint() {
        return getTargetWorldPoint() + botRightRelative;
    }

    public Vector2 getWorldPoint() {
        return new Vector2(target.transform.position.x, target.transform.position.y) + ((getTopLeftWorldPoint() - getBottomRightWorldPoint()) / 2f);
    }

    public Bounds getDefaultBoundsObject() {
        return bounds;
    }

    // hide default constructor
    private CollisionBounds() { }
}
