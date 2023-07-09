using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFinish : MonoBehaviour
{
    [SerializeField]
    private GameObject gameOver,won;

    [SerializeField]
    private GameObject scoreDisplay;
    [SerializeField]
    private List<Image> scoreImages = new List<Image>();

    private List<Sprite> numberSprites = new List<Sprite>();
    [SerializeField]
    private string pName;


    void Start()
    {
        numberSprites = Scorer.instance.numberSprites;
    }
    void OnEnable()
    {
        Debug.Log("Game Finish");
        bool hasWon = Scorer.instance.lives > 0;
        GameObject toShow = hasWon ? won : gameOver;
        LeanTween.moveY(gameObject, 360, 1f).setEaseInBack().setOnComplete(() =>{
            LeanTween.moveY(toShow, 510, 1f).setEaseInBack().setOnComplete(() =>{
                SetScore();
                LeanTween.moveY(scoreDisplay, 300, 1f).setEaseInBack().setOnComplete(() => {Time.timeScale = 0;});
            });
        });
    }

    void SetScore()
    {
        int score = Scorer.instance.score;
        int thirdDigit = score % 10;
        int secondDigit = (score / 10) % 10;
        int firstDigit = (score / 100) % 10;
        scoreImages[0].sprite = numberSprites[firstDigit];
        scoreImages[1].sprite = numberSprites[secondDigit];
        scoreImages[2].sprite = numberSprites[thirdDigit];

    }

    public void ChangeName(string name)
    {
        pName = name;
    }

    public void GoBackToMenu()
    {
        if(pName == "")
        {
            pName = "AAA";
        }
        List<int> highscores = new List<int>(){PlayerPrefs.GetInt("Highscore1Num",560),PlayerPrefs.GetInt("Highscore2Num",340),PlayerPrefs.GetInt("Highscore3Num",205),PlayerPrefs.GetInt("Highscore4Num",90),PlayerPrefs.GetInt("Highscore5Num",25)};
        List<string> highscoreNames = new List<string>(){PlayerPrefs.GetString("Highscore1Name","BOB"),PlayerPrefs.GetString("Highscore2Name","CAT"),PlayerPrefs.GetString("Highscore3Name","SAM"),PlayerPrefs.GetString("Highscore4Name","OOF"),PlayerPrefs.GetString("Highscore5Name","JON")};
        int score = Scorer.instance.score;

        bool hasChanged = false;
        //Find position in highscores
        for(int i = 0; i < highscores.Count; i++)
        {
            if(score > highscores[i])
            {
                hasChanged = true;
                highscores.Insert(i,score);
                highscores.RemoveAt(highscores.Count - 1);

                highscoreNames.Insert(i,pName);
                highscoreNames.RemoveAt(highscoreNames.Count - 1);
                break;
            }
        }
        //Set scores
        if(hasChanged)
        {
            for(int i = 0; i < highscores.Count; i++)
            {
                PlayerPrefs.SetInt("Highscore" + (i + 1) + "Num",highscores[i]);
                PlayerPrefs.SetString("Highscore" + (i + 1) + "Name",highscoreNames[i]);
            }
        }
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
