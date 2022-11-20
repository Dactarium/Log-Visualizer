using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    [SerializeField]
    private float timer = 1f;
    IEnumerator Start()
    {
        for(int i = 0; i < 100; i++){
            yield return new WaitForSeconds(timer);
            Debug.LogError("Something went wrong at Line 42");
        }
    }
}

























// Dactarium was here.