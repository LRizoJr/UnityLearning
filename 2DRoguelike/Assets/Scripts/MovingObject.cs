using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour 
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;     // pertains to the custom BlockingLayer in the Layers

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    // initialize relevant GameComponent objects
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();        
        // this lets us ese inverseMoveTime using multiplication, which is much faster computationally during run time
        // (equivalent of a velocity when used in conjunction with Time.deltaTime)
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position; // vector3 -> vector2 auto-cast
        Vector2 end = start + new Vector2(xDir, yDir);

        this.boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        this.boxCollider.enabled = true;

        if(hit.transform == null)   // no hit
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;  // wait for the end of the frame before continuing and evaluating the while loop condition
        }
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if(hit.transform == null)
        {
            return;
        }
        T hitComponent = hit.transform.GetComponent<T>();        
        if(!canMove && hitComponent != null)
        {
            // execute the colliding component's "OnCantMove" implementation
            OnCantMove(hitComponent);
        }

    }
    protected abstract void OnCantMove<T>(T component) 
        where T : Component;
}