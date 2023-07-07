using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField]
    public float speed {get; private set;} = 10.0f;

    [field: SerializeField,Range(0f,1f)]
    public float inputThreshold {get; private set;}

    private Rigidbody2D rb;

    private Animator animator;
    [SerializeField]
    private Vector2 movementVector;
    private Camera mainCamera;

    [SerializeField]
    public bool holdingRock
    {
        get
        {
            return m_holdingRock;
        }
        private set
        {
            if(m_holdingRock == value)return;
            m_holdingRock = value;
            if(OnHoldingRockChanged != null)
            {
                OnHoldingRockChanged(m_holdingRock);
            }
            
        }
    }

    public delegate void HoldingRockChanged(bool holdingRock);
    public event HoldingRockChanged OnHoldingRockChanged;

    private bool m_holdingRock;
    private GameObject rock;

    [SerializeField]
    private float rockSpeed= 0.5f,rockScaleSize = 1.5f;

    [SerializeField,Header("Crosshair")]
    private GameObject crosshairObject;

    [SerializeField]
    private float crosshairSpeed;

    private bool crosshairFollowsMouse = true;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        OnHoldingRockChanged += ChangeSpeedWhenHoldingRock;
        OnHoldingRockChanged += ActivateCrosshair;
        OnHoldingRockChanged += ChangeSpriteToHoldRock;
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(movementVector.magnitude > inputThreshold)
        {
            animator.SetBool("isWalking",true);
        }
        else
        {
            animator.SetBool("isWalking",false);
        }

        if(crosshairFollowsMouse)
        {
            Vector3 desiredPosition = new Vector3(mainCamera.ScreenToWorldPoint(Input.mousePosition).x,mainCamera.ScreenToWorldPoint(Input.mousePosition).y,0);
            Vector3 smoothedPosition = Vector3.Lerp(crosshairObject.transform.position,desiredPosition,crosshairSpeed * Time.deltaTime);
            crosshairObject.transform.position = smoothedPosition;
        }

        if(Input.GetMouseButtonDown(0) && holdingRock)
        {
            ThrowRock();
        }
    }


    void FixedUpdate()
    {
        rb.velocity = movementVector * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Rock" && !holdingRock)
        {
            Debug.Log("Collided with rock");
            rock = collision.gameObject;
            holdingRock = true;
            rock.transform.SetParent(transform);
            rock.transform.localPosition = new Vector3(0,0.8f,0);
            rock.GetComponent<Collider2D>().enabled = false;
        }
    }

    private void ChangeSpeedWhenHoldingRock(bool holdingRock)
    {
        if(holdingRock)
        {
            speed *= 0.25f;
        }
        else
        {
            speed *= 4f;
        }
    }

    private void ActivateCrosshair(bool holdingRock)
    {
        if(holdingRock)
        {
            crosshairObject.SetActive(true);
            crosshairFollowsMouse = true;
        }
    }

    private void ThrowRock()
    {
        Debug.Log("Rock thrown");
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        SpriteRenderer rockSpriteRenderer = rock.GetComponent<SpriteRenderer>();
        rockSpriteRenderer.sortingOrder = 100;
        holdingRock = false;
        crosshairFollowsMouse = false;
        rock.transform.SetParent(null);

        Vector2 target = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        float distance = Vector2.Distance(transform.position,target);

        float timeToTarget = distance / rockSpeed;

        //Scale rock up and down over time
        LeanTween.scale(rock, new Vector3(rockScaleSize,rockScaleSize,rockScaleSize), timeToTarget / 2).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);

        //Move rock to cursor over time
        LeanTween.move(rock, target, timeToTarget).setEase(LeanTweenType.easeOutQuad).setOnComplete(() => {
            rock.GetComponent<Collider2D>().enabled = true;
            rockSpriteRenderer.sortingOrder = 0;
            DisableCrosshair();
            if(cameraFollow != null)
            {
                cameraFollow.shakeDuration = 0.2f;
            }
        });
    }

    private void DisableCrosshair()
    {
        crosshairFollowsMouse = true;
        crosshairObject.SetActive(false);
    }

    private void ChangeSpriteToHoldRock(bool isHoldingRock)
    {
        animator.SetBool("isHoldingRock",isHoldingRock);
    }




}
