using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Made abstract so functions can be incomplete and finished later
public abstract class MovingObject : MonoBehaviour {

    public float moveTime;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    private float inverseMoveTime;

	// Use this for initialization; made protected and virtual so that various implementations of Start() may be created
	protected virtual void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}
	
    //"out" causes functions to be passed by reference, returning both the bool and the RaycastHit2D
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        //Discard the z-axis data
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2 (xDir, yDir);

        //Disable the boxCollider to make sure when we cast the ray we don't hit our own collider
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        //If the raycast does, in fact, hit something, begin the movement coroutine
        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        } else
        {
            return false;
        }

    }

    //Coroutine to allow for movement on tile-based grid
    protected IEnumerator SmoothMovement (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //Move the object toward the end position until the distance between the object and the defined end distance is very, very small
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    //T, in this case, defines what type of thing interacts with what if blocked
    //In the case of enemies, this is the player
    //In the case of players, this will be walls
    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T: Component
    {
        //A raycast is used only because it is computationally faster than an array lookup - in some instances, much faster
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        //If nothing was hit by the raycast, ignore the rest of the code
        if (hit.transform == null)
            return;

        //If something WAS hit, then get a reference to component of type T to the object hit
        T hitComponent = hit.transform.GetComponent<T>();

        //If an object has been hit and can be interacted with, it calls OnCantMove, passing in the given T component
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    //Abstract has an incomplete implentation, hence abstract functionality.  It will be overridden by functions in the inheriting classes
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
