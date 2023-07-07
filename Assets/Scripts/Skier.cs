using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skier : MonoBehaviour
{
    Rigidbody2D rb;
    public float initialSpeed = 10f;

    public float propelInterval = 0.5f;
    public float collisionSpeedMin = 5f;

    public float scaleSize = 1.5f;

    public int obstacleCheckInterval = 5;

    [SerializeField]
    private bool isDead;

    [SerializeField]
    private GameObject snowHead;

    [Header("AI")]
    public float overlapCircleRadius = 15f;

    private enum SteerDirection
    {
        Left,
        Right,
        Straight,
        Slow
    }
    [SerializeField]
    private SteerDirection currentSteerDirection = SteerDirection.Straight;

    private int directionChoiceAdder = 1;

    [SerializeField]
    private bool isOnBlue = true;

    [SerializeField]
    GameObject blueCheckPointParent,redCheckPointParent;

    [SerializeField]
    private int flagIndex = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(Vector2.down * initialSpeed, ForceMode2D.Impulse);
        StartCoroutine(CheckForObstacles());
        StartCoroutine(PropelRoutine());
        if(Random.value < 0.5f)
        {
            isOnBlue = false;
        }

    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if(rb.velocity.magnitude < collisionSpeedMin)
            return;


        if (col.gameObject.tag == "Obstacle" || col.gameObject.tag == "Rock")
        {
            Debug.Log("Hit obstacle");
            //Add points or something
            rb.AddForce(new Vector2(Random.Range(0.5f,1f),Random.Range(0.5f,1f)) * initialSpeed, ForceMode2D.Impulse);
            rb.AddTorque(100f, ForceMode2D.Impulse);
            GetComponentInChildren<Collider2D>().enabled = false;
            LeanTween.scale(gameObject, new Vector3(scaleSize,scaleSize,scaleSize), 1f).setLoopPingPong(1).setEaseOutQuad().setOnComplete(() => {
                isDead = true;
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
                LeanTween.rotateZ(gameObject, Random.Range(170f,190f), 1f).setEaseOutQuad().setOnComplete(() => {
                    snowHead.SetActive(true);
                });
            });
        }
    }




    IEnumerator CheckForObstacles()
    {
        yield return new WaitForSeconds(obstacleCheckInterval);


        //right circle overlap
        Collider2D[] collidersRight = Physics2D.OverlapCircleAll(transform.position + (transform.right * overlapCircleRadius) + transform.up * -5f, overlapCircleRadius);
        Debug.Log("Collider right: " + collidersRight.Length);

        Collider2D[] collidersLeft = Physics2D.OverlapCircleAll(transform.position - (transform.right * overlapCircleRadius) + transform.up * -5f, overlapCircleRadius);
        Debug.Log("Collider left: " + collidersLeft.Length);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * -1f, 10f,LayerMask.GetMask("Obstacle"));
        bool hitCheck = hit.collider != null;
        if(hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);
        }

        
        if(collidersRight.Length > collidersLeft.Length + directionChoiceAdder)
        {
            currentSteerDirection = SteerDirection.Right;
        }
        else if(collidersLeft.Length > collidersRight.Length + directionChoiceAdder)
        {
            currentSteerDirection = SteerDirection.Left;
        }
        else if(hitCheck)
        {
            currentSteerDirection = SteerDirection.Slow;
        }
        else
        {
            currentSteerDirection = SteerDirection.Straight;
        }

        //Change target flag
        //float distanceToCurrentFlag = Vector2.Distance(transform.position, isOnBlue ? blueCheckPointParent.transform.GetChild(flagIndex).position : redCheckPointParent.transform.GetChild(flagIndex).position);

        StartCoroutine(CheckForObstacles());
    }


    void Propel()
    {
        Debug.Log("Propel: " + currentSteerDirection);
        switch(currentSteerDirection)
        {
            case SteerDirection.Left:
                rb.AddForce(transform.right * -1f * initialSpeed / 5f, ForceMode2D.Impulse);
                break;
            case SteerDirection.Right:
                rb.AddForce(transform.right * initialSpeed / 5f, ForceMode2D.Impulse);
                break;
            case SteerDirection.Slow:
                rb.AddForce(transform.up * initialSpeed * 0.5f, ForceMode2D.Impulse);
                break;
            case SteerDirection.Straight:
                //rb.AddForce(transform.up * initialSpeed, ForceMode2D.Impulse);
                break;
        }
    }

    IEnumerator PropelRoutine()
    {
        yield return new WaitForSeconds(propelInterval);
        Propel();
        StartCoroutine(PropelRoutine());
    }


}
