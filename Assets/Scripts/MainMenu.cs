using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    public List<Sprite> letterSprites = new List<Sprite>();
    public List<Sprite> numberSprites = new List<Sprite>();


    [Header("Names")]
    public List<Image> nameImages0 = new List<Image>();
    public List<Image> nameImages1 = new List<Image>();
    public List<Image> nameImages2 = new List<Image>();
    public List<Image> nameImages3 = new List<Image>();
    public List<Image> nameImages4 = new List<Image>();
    [Header("Scores")]
    public List<Image> scoreImages0 = new List<Image>();
    public List<Image> scoreImages1 = new List<Image>();
    public List<Image> scoreImages2 = new List<Image>();
    public List<Image> scoreImages3 = new List<Image>();
    public List<Image> scoreImages4 = new List<Image>();

    


    void Start()
    {
        SetupLeaderboard();
    }

    void SetupLeaderboard()
    {
        List<int> highscores = new List<int>(){PlayerPrefs.GetInt("Highscore1Num",560),PlayerPrefs.GetInt("Highscore2Num",340),PlayerPrefs.GetInt("Highscore3Num",205),PlayerPrefs.GetInt("Highscore4Num",90),PlayerPrefs.GetInt("Highscore5Num",25)};
        List<string> highscoreNames = new List<string>(){PlayerPrefs.GetString("Highscore1Name","BOB"),PlayerPrefs.GetString("Highscore2Name","CAT"),PlayerPrefs.GetString("Highscore3Name","SAM"),PlayerPrefs.GetString("Highscore4Name","OOF"),PlayerPrefs.GetString("Highscore5Name","JON"),"AAA"};
        
        Debug.Log(highscores.Count);
        foreach(int score in highscores)
        {
            Debug.Log(score);
        }

        foreach(string name in highscoreNames)
        {
            Debug.Log(name);
        }

        for(int i = 0; i < highscores.Count; i++)
        {
            int score = highscores[i];
            int thirdDigit = score % 10;
            int secondDigit = (score / 10) % 10;
            int firstDigit = (score / 100) % 10;
            switch(i)
            {
                case 0:
                    scoreImages0[0].sprite = numberSprites[firstDigit];
                    break;
                case 1:
                    scoreImages1[0].sprite = numberSprites[firstDigit];
                    break;
                case 2:
                    scoreImages2[0].sprite = numberSprites[firstDigit];
                    break;
                case 3:
                    scoreImages3[0].sprite = numberSprites[firstDigit];
                    break;
                case 4:
                    scoreImages4[0].sprite = numberSprites[firstDigit];
                    break;
            }
            switch(i)
            {
                case 0:
                    scoreImages0[1].sprite = numberSprites[secondDigit];
                    break;
                case 1:
                    scoreImages1[1].sprite = numberSprites[secondDigit];
                    break;
                case 2:
                    scoreImages2[1].sprite = numberSprites[secondDigit];
                    break;
                case 3:
                    scoreImages3[1].sprite = numberSprites[secondDigit];
                    break;
                case 4:
                    scoreImages4[1].sprite = numberSprites[secondDigit];
                    break;
            }
            switch(i)
            {
                case 0:
                    scoreImages0[2].sprite = numberSprites[thirdDigit];
                    break;
                case 1:
                    scoreImages1[2].sprite = numberSprites[thirdDigit];
                    break;
                case 2:
                    scoreImages2[2].sprite = numberSprites[thirdDigit];
                    break;
                case 3:
                    scoreImages3[2].sprite = numberSprites[thirdDigit];
                    break;
                case 4:
                    scoreImages4[2].sprite = numberSprites[thirdDigit];
                    break;
            }
        }

        for(int i = 0; i < highscoreNames.Count; i++)
        {
            string name = highscoreNames[i];
            for(int j = 0; j < 3; j++)
            {
                int digit = GetNumberFromChar(name[j].ToString());
                switch(i)
                {
                    case 0:
                        nameImages0[j].sprite = letterSprites[digit];
                        break;
                    case 1:
                        nameImages1[j].sprite = letterSprites[digit];
                        break;
                    case 2:
                        nameImages2[j].sprite = letterSprites[digit];
                        break;
                    case 3:
                        nameImages3[j].sprite = letterSprites[digit];
                        break;
                    case 4:
                        nameImages4[j].sprite = letterSprites[digit];
                        break;
                }
            }
        }
    }




    public void ResetLeaderboard()
    {
        PlayerPrefs.DeleteAll();
        SetupLeaderboard();
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }



    public void CameraShakeToggle(bool toggle)
    {
        PlayerPrefs.SetInt("CameraShake",toggle ? 1 : 0);
    }

    public void MusicToggle(bool toggle)
    {
        PlayerPrefs.SetInt("Music",toggle ? 1 : 0);
    }










    //This is shit but ugh
    private int GetNumberFromChar(string c)
    {
        if(c.Length != 1)
        {
            return 0;
        }
        c = c.ToLower();
        //switch statement for each letter with a being 0 and z being 25
        switch(c)
        {
            case "a":
                return 0;
            case "b":
                return 1;
            case "c":
                return 2;
            case "d":
                return 3;
            case "e":
                return 4;
            case "f":
                return 5;
            case "g":
                return 6;
            case "h":
                return 7;
            case "i":
                return 8;
            case "j":
                return 9;
            case "k":
                return 10;
            case "l":
                return 11;
            case "m":
                return 12;
            case "n":
                return 13;
            case "o":
                return 14;
            case "p":
                return 15;
            case "q":
                return 16;
            case "r":
                return 17;
            case "s":
                return 18;
            case "t":
                return 19;
            case "u":
                return 20;
            case "v":
                return 21;
            case "w":
                return 22;
            case "x":
                return 23;
            case "y":
                return 24;
            case "z":
                return 25;
            default:
                return 0;
        }

    }
}
