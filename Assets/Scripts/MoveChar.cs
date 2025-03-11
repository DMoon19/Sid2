using System;
using UnityEngine;

public class MoveChar : MonoBehaviour
{
    private Rigidbody rb;
    private bool isGrounded=true;
    [SerializeField] private float jumpForce = 5f; // Ajustable desde el Inspector

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.Log(isGrounded);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("oli");
            rb.AddForce(new Vector3(0,jumpForce, 0), ForceMode.Impulse); // Salto con física
            if (gameObject.transform.position.y > 0)
            { isGrounded = false;}
        }
    }

    private void OnCollisionStay(Collision other) // Detecta colisión con el suelo en 3D
    {
        Debug.Log("Collision Enter");
        Debug.Log(other.gameObject.tag + " "+other.gameObject.name);
        if (other.gameObject.CompareTag("Area"))
        {
            Debug.Log("TRIGGER");
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Dead"))
        {
            Debug.Log("DIE");
            GameObject.Find("Game").SetActive(false);
        }
    }
}