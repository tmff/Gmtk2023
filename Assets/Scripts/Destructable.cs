using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{


    void OnTriggerEnter2D(Collider2D other)
    {
        GetComponent<Collider2D>().enabled = false;
        LeanTween.scaleY(gameObject, 0.1f, 0.25f).setEaseOutCirc();
    }
}
