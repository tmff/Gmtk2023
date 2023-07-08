using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scorer : MonoBehaviour
{
    public static Scorer instance;
    [SerializeField]
    private int m_score = 0;
    public int score
    {
        get
        {
            return m_score;
        }
        set
        {
            m_score = value;
            UpdateScore();
        }
    }

    public List<Sprite> numberSprites = new List<Sprite>();
    [Header("Score Images, 0 is the first digit, 1 is the second digit, etc.")]
    public List<Image> scoreImages = new List<Image>();
    public Color redText;

    [SerializeField]
    private int strength = 10;
    [SerializeField]
    private float duration = 0.1f;


    [Header("Scores")]
    public int knockoutScore = 10;
    public int finishCourseScore = -5;

    public int lives = 4;

    [SerializeField]
    private GameObject lifeParent;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip loseHealthClip;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }




    void UpdateScore()
    {
        int score = m_score;
        if(m_score < 0)
        {
            m_score = 0;
            //Take damage
            DecreaseLife();
            //Shake with leantween
            GameObject obj = gameObject;
                
            if(!LeanTween.isTweening(obj))
            {
                LeanTween.moveLocalX(obj, obj.transform.localPosition.x + strength, duration / 4)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setLoopPingPong(2)
                    .setOnComplete(() =>
                    {
                        // Shake the object back to its original position
                        LeanTween.moveLocalX(obj, obj.transform.localPosition.x - strength, duration / 2)
                            .setEase(LeanTweenType.easeInOutQuad)
                            .setLoopPingPong(2);
                    });
            }
        }

        int tempScore = m_score;
        for(int i = 0; i < scoreImages.Count; i++)
        {
            if(m_score <= 0)
            {
                scoreImages[i].color = redText;
            }
            else
            {
                scoreImages[i].color = Color.white;
            }
            int digit = tempScore % 10;
            scoreImages[i].sprite = numberSprites[digit];
            tempScore /= 10;
        }
    }

    public void AddKnockoutScore()
    {
        score += knockoutScore;
    }

    public void AddFinishCourseScore()
    {
        score += finishCourseScore;
    }

    public void DecreaseLife()
    {
        if(lives < 0)return;
        lives--;
        if(lives == 0)
        {
            //Game over
            Debug.Log("Game over");
        }
        GameObject heart = lifeParent.transform.GetChild(lives).gameObject;
        LeanTween.scale(heart, Vector3.zero, 0.15f).setEase(LeanTweenType.easeInOutQuad);
        audioSource.PlayOneShot(loseHealthClip);
    }

    public void IncreaseLife()
    {
        lives++;
        lifeParent.transform.GetChild(lives - 1).gameObject.SetActive(true);
    }
}
