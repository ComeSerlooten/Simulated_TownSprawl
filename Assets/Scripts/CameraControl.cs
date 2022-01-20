using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour
{
    public bool Rotate = true;
    Vector3 Rotation;
    Vector3 Position;

    float colliding = 1;
    float scrollSpeed = 1f;

    float forward = 0;
    float side = 0;
    float vert = 0;

    [Range(1, 10)] public int sensitivity = 2;

    [SerializeField] float speed = 1.0f;
    float speedFactor = 1f;


    // Start is called before the first frame update
    void Start()
    {
        Rotation = transform.rotation.eulerAngles;
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "EmptySpace")
        {
            colliding = 0.1f;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag != "EmptySpace")
        {
            colliding = 1;
        }
    }*/

    void ScrollSpeed()
    {
        scrollSpeed -= Input.GetAxis("Mouse ScrollWheel");
        scrollSpeed = Mathf.Clamp(scrollSpeed, 0.5f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        Rotation.x -= Input.GetAxis("Mouse Y") * sensitivity;
        Rotation.y += Input.GetAxis("Mouse X") * sensitivity;

        Rotation.x = Mathf.Clamp(Rotation.x, -89f, 89f);

        transform.rotation = Quaternion.Euler(Rotation.x, Rotation.y, 0);
        if (Input.GetKey(KeyCode.LeftControl) && ((Input.GetAxis("Vertical") != 0) || (Input.GetAxis("Horizontal") != 0))) { speedFactor = 1.5f;/* (speedFactor + 0.01f) * 1.01f; */}
        else { speedFactor = speedFactor * 0.99f; }
        speedFactor = Mathf.Clamp(speedFactor, 1, 10);
        
        if(Input.GetAxis("Vertical") != 0) { forward = scrollSpeed * speedFactor * colliding * speed * speedFactor * Input.GetAxis("Vertical") * Time.deltaTime * 2; }
        else {forward = forward * 0.9f; if (System.Math.Abs(forward) <= 0.05) { forward = 0; } }

        if (Input.GetAxis("Horizontal") != 0) { side = scrollSpeed * speedFactor * colliding * speed * speedFactor * Input.GetAxis("Horizontal") * Time.deltaTime * 2; }
        else { side = side * 0.9f; if (System.Math.Abs(side) <= 0.05) { side = 0; } }

        if (Input.GetAxis("Jump") != 0) { vert = scrollSpeed * speed * colliding * Input.GetAxis("Jump") * Time.deltaTime * 2; }
        else { vert = vert * 0.9f; if (System.Math.Abs(vert) <= 0.05) { vert = 0; } }

        //if (forward + side == 0) { speedFactor = 1; }

        Vector3 forwardHor = transform.forward;
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            forwardHor.y = 0;
            forwardHor = forwardHor.normalized;
        }


        Vector3 sideHor = transform.right;
        sideHor.y = 0;
        sideHor = sideHor.normalized;

        Position = transform.position;
        Position += forwardHor * forward + sideHor * side + Vector3.up * vert;
        Position.x = Mathf.Clamp(Position.x, -200, 200);
        Position.y = Mathf.Clamp(Position.y, 0.5f, 50);
        Position.z = Mathf.Clamp(Position.z, -200, 200);
        transform.position = Position;

        ScrollSpeed();
        /*Debug.Log(Input.GetAxis("Mouse ScrollWheel"));
        Debug.Log(scrollSpeed);*/


        if(Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene("Menu_Final");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Application.Quit();
        }
    }
}

