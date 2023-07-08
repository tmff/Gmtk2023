using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Spawner : MonoBehaviour
{
    public GameObject skierPrefab;

    public List<Wave> waves;

    private int currentIndex = -1;

    public GameObject redCheckPointParent,blueCheckPointParent;

    public Gradient skyColour;
    [Range(0,1)]
    public float skyColourValue = 0f;

    public Light2D sun;


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
            return;
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
            Skier skier = skierObject.GetComponent<Skier>();
            skier.blueCheckPointParent = blueCheckPointParent;
            skier.redCheckPointParent = redCheckPointParent;

            if(spawnsWave == false)
            {
                skier.spawnsWave = true;
                spawnsWave = true;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }


}

[System.Serializable]
public struct Wave
{
    public List<Vector3> spawnPoints;
}