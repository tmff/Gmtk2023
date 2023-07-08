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


    [SerializeField]
    private GameObject snowHead;

    [Header("AI")]
    public float obstacleCheckInterval = 5;

    private enum SteerDirection
    {
        Left,
        Right,
        Straight,
    }
    [SerializeField]
    private SteerDirection currentSteerDirection = SteerDirection.Straight;


    [SerializeField]
    private bool isOnBlue = true;

    [SerializeField]
    GameObject blueCheckPointParent,redCheckPointParent;

    [SerializeField]
    private int flagIndex = 0;

    [SerializeField]
    private float rightLeftForceMod = 0.2f;

    [SerializeField]
    private float gravityMod = 0.1f;


    [SerializeField]
    private int rayCastRange = 5;
    [SerializeField]
    private int rayAngles = 10;

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
            StopAllCoroutines();
            LeanTween.scale(gameObject, new Vector3(scaleSize,scaleSize,scaleSize), 1f).setLoopPingPong(1).setEaseOutQuad().setOnComplete(() => {
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

        float distanceToCurrentFlag;
        float distanceToNextFlag;
        Transform currentFlag = isOnBlue ? blueCheckPointParent.transform.GetChild(flagIndex) : redCheckPointParent.transform.GetChild(flagIndex);
        Transform nextFlag = isOnBlue ? blueCheckPointParent.transform.GetChild(flagIndex + 1) : redCheckPointParent.transform.GetChild(flagIndex + 1);




        RaycastHit2D forwardHit = Physics2D.Raycast(transform.position, rb.velocity.normalized * rayCastRange, rayCastRange,LayerMask.GetMask("Obstacle"));
        Debug.DrawRay(transform.position, rb.velocity.normalized * rayCastRange, Color.red, 1f);
        bool forwardHitCheck = forwardHit.collider != null;

        RaycastHit2D leftForwardHit = Physics2D.Raycast(transform.position, rotate(rb.velocity.normalized,Mathf.Deg2Rad * -rayAngles), rayCastRange,LayerMask.GetMask("Obstacle"));
        Debug.DrawRay(transform.position, rotate(rb.velocity.normalized,Mathf.Deg2Rad * -rayAngles) * rayCastRange, Color.green, 1f);
        bool leftForwardHitCheck = leftForwardHit.collider != null;

        RaycastHit2D rightForwardHit = Physics2D.Raycast(transform.position, rotate(rb.velocity.normalized,Mathf.Deg2Rad * rayAngles), rayCastRange,LayerMask.GetMask("Obstacle"));
        Debug.DrawRay(transform.position, rotate(rb.velocity.normalized,Mathf.Deg2Rad * rayAngles) * rayCastRange, Color.blue, 1f);
        bool rightforwardHitCheck = rightForwardHit.collider != null;

        

        if(forwardHitCheck)
        {
            if(isOnBlue && !rightforwardHitCheck)
            {
                currentSteerDirection = SteerDirection.Right;
            }
            else if(!isOnBlue && !leftForwardHitCheck)
            {
                currentSteerDirection = SteerDirection.Left;
            }
        }
        else if(rightforwardHitCheck)
        {
            currentSteerDirection = SteerDirection.Left;
        }
        else if(leftForwardHitCheck)
        {
            currentSteerDirection = SteerDirection.Right;
        }
        else
        {
            currentSteerDirection = SteerDirection.Straight;
        }



        /*
        if(isOnBlue)
        {

            distanceToCurrentFlag = Vector2.Distance(transform.position, blueCheckPointParent.transform.GetChild(flagIndex).position);
            distanceToNextFlag = Vector2.Distance(transform.position, blueCheckPointParent.transform.GetChild(flagIndex + 1).position);
        }
        else
        {
            distanceToCurrentFlag = Vector2.Distance(transform.position, redCheckPointParent.transform.GetChild(flagIndex).position);
            distanceToNextFlag = Vector2.Distance(transform.position, redCheckPointParent.transform.GetChild(flagIndex + 1).position);
        }
        */
        distanceToCurrentFlag = Vector2.Distance(transform.position, currentFlag.position);
        distanceToNextFlag = Vector2.Distance(transform.position, nextFlag.position);
        if(distanceToNextFlag < distanceToCurrentFlag || transform.position.y < currentFlag.position.y)
        {
            flagIndex++;
            if(flagIndex >= (isOnBlue ? blueCheckPointParent.transform.childCount : redCheckPointParent.transform.childCount))
            {
                //Finish skier
                Debug.Log("Finish skier");
            }
        }
        StartCoroutine(CheckForObstacles());
    }


    void Propel()
    {
        rb.AddForce(transform.up * initialSpeed * -1f * gravityMod, ForceMode2D.Impulse);

        Debug.Log("Propel: " + currentSteerDirection);
        switch(currentSteerDirection)
        {
            case SteerDirection.Left:
                rb.AddForce(transform.right * -1f * initialSpeed * rightLeftForceMod, ForceMode2D.Impulse);
                break;
            case SteerDirection.Right:
                rb.AddForce(transform.right * initialSpeed * rightLeftForceMod, ForceMode2D.Impulse);
                break;
            case SteerDirection.Straight:
                //Add force towards next flag
                if(isOnBlue)
                {
                    rb.AddForce((blueCheckPointParent.transform.GetChild(flagIndex).position - transform.position).normalized * initialSpeed / 2f, ForceMode2D.Impulse);
                }
                else
                {
                    rb.AddForce((redCheckPointParent.transform.GetChild(flagIndex).position - transform.position).normalized * initialSpeed / 2f, ForceMode2D.Impulse);
                }
                break;
        }
    }

    IEnumerator PropelRoutine()
    {
        yield return new WaitForSeconds(propelInterval);
        Propel();
        StartCoroutine(PropelRoutine());
    }

    public Vector2 rotate(Vector2 v, float delta) {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }



}
