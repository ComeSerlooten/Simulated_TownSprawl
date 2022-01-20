using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBase : MonoBehaviour
{
    Quaternion FreeSpaces = new Quaternion();

    public float money = 0;
    public int population = 0;
    public int level = 1;
    [SerializeField] public float free = 0;

    [Space]
    [SerializeField] public float Size = 1.5f;
    [SerializeField] public int houseCountMax = 2000;
    [SerializeField] float spacing = 2.0f;
    [Range(1f, 5000.0f)]
    [SerializeField] float acceleration = 1.0f;
    float accelerationRatio;

    [Space]
    [Space]

    [Header("Layers of the building")]
    [SerializeField] public List<GameObject> Layers = new List<GameObject>();
    [Space]
    [Space]
    [SerializeField] GameObject roofPrefab;
    [SerializeField] List<GameObject> layerPrefabs = new List<GameObject>();

    [SerializeField] GameObject NavAgent;
    [SerializeField] GameObject HouseParent;
    [SerializeField] GameObject EmptyParent;
    [SerializeField] GameObject NavParent;
    [SerializeField] int distanceNavTravel = 50;

    [Space]
    [Space]
    Collider[] hitColliders;

    [SerializeField] GameObject prefabHouse;
    [SerializeField] int houseSpawnChance = 3;
    [SerializeField] GameObject prefabSpace;
    [SerializeField] List<GameObject> prefabGarden = new List<GameObject>();
    [SerializeField] int spaceSpawnChance = 2;
    int totalSpawnChances;
    GameObject Prefab;

    [Space]
    [Space]
    [Header("Default Color : (147, 71, 48, 1")]
    [SerializeField] int materialR = 147;
    [SerializeField] int materialG = 71;
    [SerializeField] int materialB = 48;

    [HideInInspector] public bool isSelected = false;

    Vector3 forward;
    Vector3 right;
    Vector3 back;
    Vector3 left;

    public List<GameObject> neighbours;

    public int index;



    [Space]
    [SerializeField] float startMoney = 5;
    [SerializeField] int startPopulation = 2;
    [SerializeField] int maxLevel = 10;

    void SetBrickColor(int materialR, int materialG, int materialB)
    {
        Color32 materialColor = new Color32((byte)materialR, (byte)materialG, (byte)materialB, 1);
        //Debug.Log(materialColor);
        GetComponent<MeshRenderer>().materials[0].color = materialColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        accelerationRatio = acceleration * GlobalVar.speedRatio;
        Layers = new List<GameObject>();
        isSelected = false;
        houseCountMax = GlobalVar.maxHouses;
        //GetComponent<Outline>().enabled = isSelected;

        transform.SetParent(HouseParent.transform);
        foreach (Layer layer in transform.GetComponentsInChildren<Layer>())
        {
            Destroy(layer.gameObject);
        }
        isSelected = false;

        transform.rotation = Quaternion.Euler(0, Random.Range(0,4) * 90, 0);


        //Value Initialisation
        neighbours = new List<GameObject>();
        forward = (Size + spacing) * transform.forward;
        right = (Size + spacing) * transform.right;
        back = -(Size + spacing) * transform.forward;
        left = -(Size + spacing) * transform.right;
        //materialColor = new Color32((byte)materialR, (byte)materialG, (byte)materialB, (byte)materialA);

        // Scale initialisation
        transform.localScale = new Vector3(Size, 0.25f * Size, Size);

        // Index and blocCount adjustment
        GlobalVar.blocCount++;
        index = GlobalVar.blocCount;
        // maxSize = 3 * Size * randSizeFactor * level / (float)System.Math.Log10(index + 10);

        // position reajustment
        Vector3 position = transform.position;
        position.y = transform.localScale.y / 2;
        transform.position = position;

        //Vary Color from origin
        if (index != 0)
        {
            materialR += Random.Range(-15, 15);
            materialG += Random.Range(-15, 15);
            materialB += Random.Range(-15, 15);

            materialR = Mathf.Clamp(materialR, 10, 245);
            materialG = Mathf.Clamp(materialG, 10, 245);
            materialB = Mathf.Clamp(materialB, 10, 245);
        }
        SetBrickColor(materialR, materialG, materialB);

        //Add Roof
        Vector3 roofPos = transform.position + Vector3.up * (0.5f * Size * (Layers.Count + 1));
        GameObject newLayer = Instantiate(roofPrefab, position: roofPos, rotation: Quaternion.identity);
        Layers.Add(newLayer);
        Vector3 Scale = Vector3.one * Size;
        newLayer.transform.localScale = Scale;
        newLayer.GetComponent<Layer>().SetBrickColor(materialR + 5, materialG + 5, materialB + 5);
        newLayer.transform.SetParent(transform);

        // Content initialisation
        money = startMoney;
        population = startPopulation;
        level = 1;
        totalSpawnChances = houseSpawnChance + spaceSpawnChance;
        maxLevel = Random.Range(4, 11);

        // Initialize surrounding check
        SurroundCheck();
    }

    void SurroundCheck()
    {
        // Reinitialise variables
        neighbours = new List<GameObject>();
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        float spacingray = 2 * (1 + spacing);
        // Get a position near the House base
        Vector3 basePos = transform.position;

        //Check Front
        // Get a list of objects in front direction (theoretically, only one will be there)
        RaycastHit[] RayHits = (Physics.SphereCastAll(basePos + 1f * Size * transform.forward, transform.localScale.y / 4, transform.forward, spacingray, layerMask, 0));
        if (index == 0) { Debug.Log(RayHits.Length); }
        // If no object is found
        if (RayHits.Length == 0)
        {
            //Draw corresponding Green Debug Ray and set the FreeSpace Vector value to 1 (free)
            Debug.DrawRay(basePos, transform.forward * spacingray, Color.green);
            FreeSpaces.w = 1;
        }
        // If no object is found
        else
        {
            // Set the FreeSpace Vector value to 0 (occupied)
            FreeSpaces.w = 0;
            // Draw the Debug Ray corresponding to the object encountered
            if (RayHits[0].collider.tag == "House")
            {
                Debug.DrawRay(basePos, transform.forward * spacingray, Color.red);
                neighbours.Add(RayHits[0].collider.gameObject);
            }
            else if (RayHits[0].collider.tag == "EmptySpace") { Debug.DrawRay(basePos, transform.forward * spacingray, Color.blue); }
        }

        //Check Right
        RayHits = Physics.SphereCastAll(basePos + 1f * Size * transform.right, transform.localScale.y / 4, transform.right, spacingray, layerMask, 0);
        if (index == 0) { Debug.Log(RayHits.Length); }
        if (RayHits.Length == 0)
        {
            Debug.DrawRay(basePos, transform.right * spacingray, Color.green);
            FreeSpaces.x = 1;
        }
        else
        {
            FreeSpaces.x = 0;

            if (RayHits[0].collider.tag == "House")
            {
                Debug.DrawRay(basePos, transform.right * spacingray, Color.red);
                neighbours.Add(RayHits[0].collider.gameObject);
            }
            else if (RayHits[0].collider.tag == "EmptySpace") { Debug.DrawRay(basePos, transform.right * spacingray, Color.blue); }
        }

        //Check Back
        RayHits = Physics.SphereCastAll(basePos - 1f * Size * transform.forward, transform.localScale.y / 4, -transform.forward, spacingray, layerMask, 0);
        if (index == 0) { Debug.Log(RayHits.Length); }
        if (RayHits.Length == 0)
        {
            Debug.DrawRay(basePos, -transform.forward * spacingray, Color.green);
            FreeSpaces.y = 1;
        }
        else
        {
            FreeSpaces.y = 0;

            if (RayHits[0].collider.tag == "House")
            {
                Debug.DrawRay(basePos, -transform.forward * spacingray, Color.red);
                neighbours.Add(RayHits[0].collider.gameObject);
            }
            else if (RayHits[0].collider.tag == "EmptySpace") { Debug.DrawRay(basePos, -transform.forward * spacingray, Color.blue); }
        }

        //Check Left
        RayHits = Physics.SphereCastAll(basePos - 1f * Size * transform.right, transform.localScale.y / 4, -transform.right, spacingray, layerMask, 0);
        if (index == 0) { Debug.Log(RayHits.Length); }
        if (RayHits.Length == 0)
        {
            Debug.DrawRay(basePos, -transform.right * spacingray, Color.green);
            FreeSpaces.z = 1;
        }
        else
        {
            FreeSpaces.z = 0;

            if (RayHits[0].collider.tag == "House")
            {
                Debug.DrawRay(basePos, -transform.right * spacingray, Color.red);
                neighbours.Add(RayHits[0].collider.gameObject);
            }
            else if (RayHits[0].collider.tag == "EmptySpace") { Debug.DrawRay(basePos, -transform.right * spacingray, Color.blue); }
        }
    }

    void MoneyUp()
    {
        // Random chance to enter the loop
        if (Random.Range(0f, 50 / (accelerationRatio * population)) < 2)
        {
            // If the money isn't maxed out
            if (money < 8 * population) { money += 0.5f * level * (1 + (population / (level * level * 5))); }
            // Distribution of extra money to neighbours
            /*foreach (GameObject i in neighbours)
            {
                if (i.GetComponent<HouseBase>().money <= 20 * i.GetComponent<HouseBase>().population)
                {
                    i.GetComponent<HouseBase>().money += 0.1f * (float)System.Math.Log10(population);
                    //Debug.DrawRay(transform.position, i.transform.position - transform.position, Color.yellow, 0.1f, true);
                }
            }*/
        }
    }

    void PopUp()
    {
        // Random chance to enter the loop
        if (Random.Range(0, (6000 / (accelerationRatio * (population/2)))) < 2)
        {

            // If the money isn't maxed out
            if (population < 5 * level * level) { population += 1; }
        }
    }

    void PopDown()
    {
        if (population > 2 && Random.Range(0, 10000 / accelerationRatio) < 2) { population--; }
    }

    public void Spawner()
    {
        // If there are less houses In-Game than the specified maximum,
        //and money and population are over half their max amount

        if ((GlobalVar.blocCount <= houseCountMax) && money > 5 * population && population > level * level * 2.5 /*&& FreeSpaces == one*/)
        {
            // Decrease population and money to "pay"
            money = money / 4;
            population /= 2;
            // Empty the directions vector
            List<Vector3> directions = new List<Vector3>();
            // update the directions vector to contain the vectors of the free spaces
            if (FreeSpaces.w == 1) { directions.Add(forward); }
            if (FreeSpaces.x == 1) { directions.Add(right); }
            if (FreeSpaces.y == 1) { directions.Add(back); }
            if (FreeSpaces.z == 1) { directions.Add(left); }

            // If there are free spaces
            if (directions.Count > 0)
            {
                // Set an offset vector as a random element of directions
                Vector3 offset = directions[Random.Range(0, directions.Count)];

                // Decide either to spawn a house or an empty space
                if (neighbours.Count == 0) { Prefab = prefabHouse; }
                else
                {
                    if (Random.Range(0, totalSpawnChances) > houseSpawnChance) { Prefab = (Random.Range(0,10)) == 1? prefabGarden[Random.Range(0,prefabGarden.Count)] : prefabSpace ; }
                    else { Prefab = prefabHouse; }
                }

                // Instantiate the new building
                GameObject newHouse = Instantiate(Prefab, transform.position + offset, transform.rotation);
                if (Prefab == prefabHouse)
                {
                    newHouse.transform.SetParent(HouseParent.transform);
                }
                else if (Prefab == prefabSpace)
                {
                    newHouse.transform.SetParent(EmptyParent.transform);
                }
                else
                {
                    newHouse.transform.SetParent(EmptyParent.transform);
                    newHouse.transform.localScale *= Size;
                    Vector3 position = newHouse.transform.position;
                    position.y = newHouse.transform.localScale.y / 2 - transform.localScale.y;
                    newHouse.transform.position = position;
                    newHouse.transform.Rotate(Vector3.up, Random.Range(0, 4) * 90);
                }

            }
        }
    }

    public void LvlUp()
    {
        if (money >= 8 * population && population >= (level * level * 5) - 1 && Random.Range(0, 5000 / accelerationRatio) >= 1 && level < maxLevel && free <= 1)
        {
            // "pay" for the increase
            money = money / 3;
            population = population / 2;
            // Increase level
            level++;

            // Add Layer
            GameObject newLayer = Instantiate(layerPrefabs[Random.Range(0, layerPrefabs.Count)], position: (transform.position + Vector3.up * (0.5f * Size * (Layers.Count) - transform.localScale.y / 2)), rotation: Quaternion.identity);
            Layers.Add(newLayer);
            //newLayer.GetComponent<Layer>().SetBrickColor(materialR + (5 * (Layers.Count-1)), materialG + (5 * (Layers.Count - 1)), materialB + (5 * (Layers.Count - 1)), materialA);
            //Layers[0].GetComponent<Layer>().SetBrickColor(materialR + (5 * (Layers.Count)), materialG + (5 * (Layers.Count)), materialB + (5 * (Layers.Count)), materialA);
            Vector3 Scale = newLayer.transform.localScale * Size;
            //Scale.y *= 0.5f;
            newLayer.transform.localScale = Scale;
            newLayer.transform.SetParent(transform);

            Layers[0].transform.position = transform.position
                                            + Vector3.up * (0.5f * Size * (Layers.Count - 1)
                                            + Layers[0].transform.localScale.y / 4 - transform.localScale.y / 2)
                                            + Layers[0].transform.forward * 0.125f * Size;

            SetBrickColor(materialR + 5, materialG + 5, materialB + 5);
            for (int i = 0; i < Layers.Count; i++)
            {
                GameObject layer = Layers[i];
                layer.GetComponent<Layer>().SetBrickColor(materialR + (i + 1) * 5, materialG + (i+1) * 5, materialB + (i+1) * 5);
            }
            

            /*
            //Add Color Increment
            materialR += 5;
            materialG += 5;
            materialA += 5;*/

        }
    }

    // Update is called once per frame
    void Update()
    {
        // free = FreeSpaces.w + FreeSpaces.x + FreeSpaces.y + FreeSpaces.z;
        // SurroundCheck();
        /* if (free > 0)
         {
             SurroundCheck();
         }*/
        free = FreeSpaces.w + FreeSpaces.x + FreeSpaces.y + FreeSpaces.z;

        //GetComponent<Outline>().enabled = isSelected;

        MoneyUp();
        PopUp();
        PopDown();


        float randomaction = Random.Range(0f, 4);

        if (Random.Range(0, (2000 / accelerationRatio)) <= 1)
        {

            if (randomaction < free)
            {
                SurroundCheck();
                if (randomaction < free)
                    Spawner();
            }
        }

        LvlUp();
        if(population < 2) { population = 2; }
        
        //if (index == 1) { Debug.Log(neighbours.Count); }
        /// ((float)System.Math.Log(GlobalVar.blocCount)+1)
        if (Random.Range(0, 2000 / (acceleration/2)) <= 5 && population >= 5 && GlobalVar.navEnabled)
        {
            GameObject Nav = Instantiate(NavAgent, transform.position + transform.forward * 1.1f, transform.rotation);
            Nav.GetComponent<MoveTo>().origin = this.gameObject;
            Nav.transform.SetParent(NavParent.transform);
            population -= 2;
        }
    }
}
