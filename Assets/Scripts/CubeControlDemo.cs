using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControlDemo : MonoBehaviour
{
    public float cubeSpeed;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * cubeSpeed * Time.deltaTime;
        float moveY = Input.GetAxis("Vertical") * cubeSpeed * Time.deltaTime;
        transform.Translate(moveX, moveY, 0);
    }
}
