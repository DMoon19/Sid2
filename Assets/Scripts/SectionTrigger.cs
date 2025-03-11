using System;
using Unity.VisualScripting;
using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject road;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            road = GameObject.FindWithTag("Area");
            Debug.Log(other.gameObject.name);
            Instantiate(road, new Vector3(5,0,0),Quaternion.identity);
        }
    }
}