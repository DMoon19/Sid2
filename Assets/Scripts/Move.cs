using UnityEngine;

public class Move : MonoBehaviour
{
   
    void Update()
    {
        transform.position += new Vector3(-2,0,0) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);
        }
    }
}