using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Butterfly butterfly;
    public int maxNumberOfButterflies;
    
    void Start()
    {
        int numberOfButterflies = Random.Range(0, maxNumberOfButterflies + 1);
        while (numberOfButterflies > 0) {
            Instantiate(butterfly, transform.position, Quaternion.identity, transform);
            numberOfButterflies--;
        }
    }
}
