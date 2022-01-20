using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCanvas : MonoBehaviour
{
    [Space]
    public GameObject selectedBuilding;
    public bool isSelected;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject cameraPlayer;

    [Space] [Space]
    [SerializeField] Text indexSelectObj;
    [SerializeField] Text popSelectObj;
    [SerializeField] Text moneySelectObj;
    [SerializeField] Text levelSelectObj;
    [Space]
    [SerializeField] Text popTotObj;
    [SerializeField] Text houseTotObj;
    [Space]
    [Space]
    [SerializeField] PointerArrow arrow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void moveToSelected()
    {
        if(selectedBuilding)
        {
            transform.position = selectedBuilding.transform.position;
            transform.position += Vector3.up * (selectedBuilding.transform.localScale.y / 2 + 4f);
        }
    }

    public void contentUpdate()
    {
        HouseBase buildingComp = selectedBuilding.GetComponent<HouseBase>();

        indexSelectObj.text = "House Number : " + buildingComp.index.ToString();

        popSelectObj.text = "Population : " + buildingComp.population.ToString();

        moneySelectObj.text = "Money : " + buildingComp.money.ToString();

        levelSelectObj.text = "Level : " + buildingComp.level.ToString();

        popTotObj.text = "Total World Population  : " + GlobalVar.totalPop.ToString();

        houseTotObj.text = "Total Houses : " + GlobalVar.blocCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(isSelected)
        {
            arrow.pointedHouse = selectedBuilding;
            arrow.gameObject.SetActive(true);

            canvas.SetActive(true);
            //moveToSelected();
            transform.LookAt(cameraPlayer.transform);
            contentUpdate();
        }
        else
        {
            canvas.SetActive(false);

            arrow.gameObject.SetActive(false);

        }
    }
}
