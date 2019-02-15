using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Butterfly butterfly;
    
    void Start()
    {
        Instantiate(butterfly, transform.position, Quaternion.identity, transform);
    }
}
