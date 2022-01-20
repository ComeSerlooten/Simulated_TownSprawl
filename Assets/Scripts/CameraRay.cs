using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using cakeslice;

public class CameraRay : MonoBehaviour
{

    public int pointedIndex = 0;
    int rayDist = 0;
    GameObject previous;
    [SerializeField] public InfoCanvas display;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void GetHouse()
    {
        previous = display.selectedBuilding;
        rayDist = 1;
        RaycastHit[] pointedHouse = (Physics.RaycastAll(transform.position, transform.forward, rayDist));
        //Debug.DrawRay(transform.position, transform.forward * rayDist, Color.white, 0.1f);
        while (pointedHouse.Length == 0 && rayDist < 10000)
        {
            //if (rayDist >= 300) { print ("No House found in range"); }
            rayDist++;
            pointedHouse = (Physics.RaycastAll(transform.position, transform.forward, rayDist));
            Debug.DrawRay(transform.position, transform.forward * rayDist, Color.white, 0.1f);
        }

        bool found;
        GameObject go;
        if (pointedHouse[0].collider.transform.gameObject.GetComponent<HouseBase>())
        {
            found = true;
            go = pointedHouse[0].collider.transform.gameObject;

        }
        else if (pointedHouse[0].collider.transform.GetComponent<Layer>())
        {
            found = true;
            go = pointedHouse[0].collider.transform.parent.gameObject;
        }
        else { go = null; found = false; }



        if (found && previous != pointedHouse[0].collider.transform.gameObject)
        {
            if (display.selectedBuilding)
            {
                //display.selectedBuilding.GetComponent<HouseBase>().material.color = new Color(90, 58, 51);
                if (display.selectedBuilding.GetComponent<HouseBase>())
                { display.selectedBuilding.GetComponent<HouseBase>().isSelected = false; }
                display.selectedBuilding = null;
            }
            display.isSelected = false;

            pointedIndex = go.GetComponent<HouseBase>().index;
            display.selectedBuilding = go;
            //Selection.activeObject = go;
            display.isSelected = true;
            display.selectedBuilding.GetComponent<HouseBase>().isSelected = true;

        }
        else if (!found /*&& previous == pointedHouse[0].collider.transform.gameObject*/)
        {
            //display.selectedBuilding.GetComponent<CubeSpreading>().material.color = new Color(90, 58, 51);
            if ( display.selectedBuilding && display.selectedBuilding.GetComponent<HouseBase>())
            { display.selectedBuilding.GetComponent<HouseBase>().isSelected = false; }

            display.selectedBuilding = null;
            display.isSelected = false;

            //pointedHouse[0].collider.transform.gameObject.GetComponent<Outline>().enabled = false;
        }



    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            GetHouse();
        }
    }
}
