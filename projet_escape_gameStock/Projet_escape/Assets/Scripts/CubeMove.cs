using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{

    public CharacterController characterController;
    public Transform Cam;
    public float speed;
    private int run = 1;
    public Vector3 moveDirection;
    Vector3 CamRot;
    Vector3 prevCamRot;
    Vector3 HCamRot;
    
    [SerializeField] GameObject PrefabBall;

    [SerializeField] private Transform Head;
    private Vector3 Hoffset;

    [Range(20, 1)]
    public int gravity = 10;
    [Range(200, 2000)]
    public int sensitivity = 500;


    // Start is called before the first frame update

    void Start()
    {
        Move();
        Hoffset = transform.position - Head.position;
    }

    /*IEnumerator Start()
    {
        Hoffset = transform.position - Head.position;

        for (int i = 0; i < 10000; i++)
        {
            GameObject go = GameObject.Instantiate
                (
                PrefabBall, 
                Vector3.up * 10 
                + (Random.value * 2 - 1) * Vector3.right 
                + (Random.value *2 -1) * Vector3.forward, 
                Quaternion.identity
                );
            //print(i);
            yield return new WaitForSeconds(0.0001f);
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        HeadMove();
        Move();
    }


    private void Move()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        { run = 2; }
        else
        { run = 1; }



        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");
        float upwardsMove = Input.GetAxis("Jump");

        if (characterController.isGrounded)
        {
            if (horizontalMove != 0 || verticalMove != 0)
            {
                CamRot = transform.position + Cam.forward;
                CamRot.y = transform.position.y;
                transform.LookAt(Vector3.Slerp(prevCamRot, CamRot, 0.2f));
                prevCamRot = CamRot;
            }

            moveDirection = new Vector3(horizontalMove * run, upwardsMove * 3, verticalMove * run);
            moveDirection = transform.TransformDirection(moveDirection);
        }

        //CamRot.y = Cam.rotation.y;
        //transform.rotation = CamRot;
        moveDirection.y -= gravity * Time.deltaTime;
       
        characterController.Move(moveDirection * speed * Time.deltaTime);



    }

    private void HeadMove()
    {

        HCamRot = Head.position + Cam.forward;
        HCamRot.y = Head.position.y;
        Head.LookAt(HCamRot);
        Head.position = transform.position - Hoffset;
    }
}
