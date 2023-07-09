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

    [SerializeField]
    private float targetMaxY;

    [Header("Audio")]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] footstepClips;

    [SerializeField]
    private float footstepInterval;

    [SerializeField]
    private AudioClip rockPickupClip,rockThrowClip;


    [SerializeField]
    private AudioClip rockLandClip;

    [SerializeField]
    private AudioClip pieClip;

    [SerializeField]
    private GameObject tutorialKey;





    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        OnHoldingRockChanged += ChangeSpeedWhenHoldingRock;
        OnHoldingRockChanged += ActivateCrosshair;
        OnHoldingRockChanged += ChangeSpriteToHoldRock;
        mainCamera = Camera.main;
        StartCoroutine(PlayFootStepAudio());

        GameObject musicObject = GameObject.FindGameObjectWithTag("Music");
        if(musicObject != null)
        {
            if(PlayerPrefs.GetInt("Music",1) == 1)
            {
                musicObject.GetComponent<AudioSource>().Play();
            }
            else
            {
                musicObject.GetComponent<AudioSource>().Stop();
            }
        }
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

        if(Input.anyKeyDown && tutorialKey.activeSelf)
        {
            StartCoroutine(ShortDelay());
        }
    }

    IEnumerator ShortDelay()
    {
        yield return new WaitForSeconds(2f);
        LeanTween.moveLocalY(tutorialKey,1440f,4f).setOnComplete(()=>{tutorialKey.SetActive(false);});
    }

    IEnumerator PlayFootStepAudio()
    {
        yield return new WaitForSeconds(footstepInterval);
        if(animator.GetBool("isWalking"))
        {
            audioSource.PlayOneShot(footstepClips[Random.Range(0,footstepClips.Length)]);
        }
        StartCoroutine(PlayFootStepAudio());
    }


    void FixedUpdate()
    {
        rb.velocity = movementVector * speed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Rock" && !holdingRock && !crosshairObject.activeSelf)
        {
            Debug.Log("Collided with rock");
            rock = collision.gameObject;
            holdingRock = true;
            rock.transform.SetParent(transform);
            rock.transform.localPosition = new Vector3(0,0.8f,0);
            rock.GetComponent<Collider2D>().enabled = false;
            audioSource.PlayOneShot(rockPickupClip,1f);
        }

        if(collision.gameObject.tag == "Skier")
        {
            Scorer.instance.DecreaseLife();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Pie")
        {
            speed *= 1.3f;
            collider.gameObject.SetActive(false);
            audioSource.PlayOneShot(pieClip,0.5f);
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
        holdingRock = false;
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        SpriteRenderer rockSpriteRenderer = rock.GetComponent<SpriteRenderer>();
        rockSpriteRenderer.sortingOrder = 100;
        crosshairFollowsMouse = false;
        GameObject currentMap = Spawner.instance.GetMap();
        rock.transform.SetParent(currentMap.transform);

        audioSource.PlayOneShot(rockThrowClip,1f);

        Vector2 target = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if(target.y > targetMaxY)
        {
            target.x = Mathf.Min(target.x,Random.Range(-15f,-20f));
        }

        float distance = Vector2.Distance(transform.position,target);

        float timeToTarget = distance / rockSpeed;

        //Scale rock up and down over time
        LeanTween.scale(rock, new Vector3(rockScaleSize,rockScaleSize,rockScaleSize), timeToTarget / 2).setEase(LeanTweenType.easeOutQuad).setLoopPingPong(1);

        //Move rock to cursor over time
        LeanTween.move(rock, target, timeToTarget).setEase(LeanTweenType.easeOutQuad).setOnComplete(() => {
            rock.GetComponent<Collider2D>().enabled = true;
            rock.GetComponent<AudioSource>().PlayOneShot(rockLandClip);
            rockSpriteRenderer.sortingOrder = 0;
            DisableCrosshair();
            if(cameraFollow != null && PlayerPrefs.GetInt("CameraShake",1) == 1)
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
