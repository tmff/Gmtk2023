using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Spawner : MonoBehaviour
{
    public GameObject skierPrefab;

    public List<Wave> waves;

    private int currentIndex = -1;

    private int currentMap = 0;

    public List<GameObject> maps = new List<GameObject>();

    public List<GameObject> redCheckpointParents = new List<GameObject>();
    public List<GameObject> blueCheckpointParents = new List<GameObject>();

    public static Spawner instance;
    public Gradient skyColour;
    [Range(0,1)]
    public float skyColourValue = 0f;

    public Light2D sun;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        SpawnWave();
    }



    public void SpawnWave()
    {
        float step = 1f / waves.Count;
        Debug.Log(step +" Tweening");
        LeanTween.value(gameObject, (float value) => {
            skyColourValue = value;
            sun.color = skyColour.Evaluate(skyColourValue);
        }, skyColourValue, skyColourValue + step, 3f);


        currentIndex++;
        if(currentIndex >= waves.Count)
        {
            //Gameover!
            currentIndex = 0;
            IncrementMap();
            if(currentMap >= maps.Count)return;
        }
        StartCoroutine(SpawnDelay());
    }



    IEnumerator SpawnDelay()
    {
        Wave wave = waves[currentIndex];
        bool spawnsWave = false;
        foreach(Vector3 spawnPoint in wave.spawnPoints)
        {
            GameObject skierObject = Instantiate(skierPrefab, spawnPoint, Quaternion.identity);
            skierObject.transform.SetParent(maps[currentMap].transform);
            Skier skier = skierObject.GetComponent<Skier>();
            skier.blueCheckPointParent = blueCheckpointParents[currentMap];
            skier.redCheckPointParent = redCheckpointParents[currentMap];

            if(spawnsWave == false)
            {
                skier.spawnsWave = true;
                spawnsWave = true;
            }
            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(10f);
        SpawnWave();
    }

    public void IncrementMap()
    {

        foreach(Light2D light2D in FindObjectsOfType<Light2D>())
        {
            if(light2D == sun)continue;

            LeanTween.value(gameObject, (float value) => {
                light2D.intensity = value;
            },1,0,0.5f);
        }

        LeanTween.value(gameObject, (float value) => {
            sun.intensity = value;
        }, sun.intensity, 0, 2f).setOnComplete(() => {
            maps[currentMap].SetActive(false);
            currentMap++;
            sun.color = skyColour.Evaluate(0);
            if(currentMap >= maps.Count)
            {
                //Gameover!
                Scorer.instance.gameFinishedPanel.SetActive(true);
                return;
            }
            maps[currentMap].SetActive(true);
            FindObjectOfType<PlayerController>().transform.position = new Vector3(0, 0, 0);
            
            
            LeanTween.value(gameObject, (float value) => {
                sun.intensity = value;
                sun.color = Color.white;
            }, 0, 1, 2f);
        });
    }

    public GameObject GetMap()
    {
        return maps[currentMap];
    }


}

[System.Serializable]
public struct Wave
{
    public List<Vector3> spawnPoints;
}