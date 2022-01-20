using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerArrow : MonoBehaviour
{
    public Vector3 targetPosition;
    public GameObject pointedHouse;
    Vector3 offset;
    Vector3 rotating = Vector3.up;

    [SerializeField] float distanceToRoof = 1;

    [SerializeField] float amplitude = 1;
    [SerializeField] [Range(0.1f, 4f)] float verticalSpeed = 1;
    [SerializeField] [Range(0.1f, 2f)] float rotatingSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = new Vector3(pointedHouse.GetComponent<HouseBase>().transform.position.x, pointedHouse.GetComponent<HouseBase>().Layers[0].transform.position.y, pointedHouse.GetComponent<HouseBase>().transform.position.z) + distanceToRoof * Vector3.up;
        rotating = Quaternion.AngleAxis(1 * verticalSpeed, Vector3.right) * rotating;
        offset = new Vector3(0, rotating.y * amplitude, 0);

        transform.position = targetPosition + offset;
        transform.RotateAround(transform.position, Vector3.up, rotatingSpeed);

        GetComponentInChildren<Light>().gameObject.transform.position = pointedHouse.GetComponent<HouseBase>().transform.position;

    }
}
