using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpreading : MonoBehaviour
{

    Quaternion FreeSpaces = new Quaternion();
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
    [SerializeField] int spaceSpawnChance = 2;
    int totalSpawnChances;
    GameObject Prefab;
    [HideInInspector] public Material material;
    [Space]
    [Space]
    [Header("Default Color : (90, 58, 51, 1")]
    [SerializeField] int materialR = 90;
    [SerializeField] int materialG = 58;
    [SerializeField] int materialB = 51;
    [SerializeField] int materialA = 1;
    Color32 materialColor;
    [HideInInspector] public bool isSelected = false;

    [Space]
    [SerializeField] float Size = 1.5f;
    [SerializeField] int houseCountMax = 200;
    [SerializeField] float spacing = 2.0f;
    float currentSize;

    [Range(1f, 10000.0f)]
    [SerializeField] float accelerationRatio = 1.0f;

    Vector3 forward;
    Vector3 right;
    Vector3 back;
    Vector3 left;

    List<GameObject> neighbours;

    public int index;
    float maxSize;
    float randSizeFactor;

    float free = 0;

    [Space]
    [SerializeField] float startMoney = 20;
    [SerializeField] int startPopulation = 2;
    [Space]
    [Space]
    public float money = 0;
    public int population = 0;
    public int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Add self to house list
        /*GlobalVar.Houses.Add(this.gameObject);*/

        transform.SetParent(HouseParent.transform);
        isSelected = false;
        material = GetComponent<Renderer>().material;

        // Scale initialisation
        randSizeFactor = Random.Range(1.0f, 1.5f);
        Vector3 Scale = Vector3.one * Size;
        transform.localScale = Scale;
        currentSize = transform.localScale.y;

        //Value Initialisation
        neighbours = new List<GameObject>();
        forward = (Size + spacing) * transform.forward;
        right = (Size + spacing) * transform.right;
        back = -(Size + spacing) * transform.forward;
        left = -(Size + spacing) * transform.right;
        materialColor = new Color32((byte)materialR, (byte)materialG, (byte)materialB, (byte)materialA);

        // Content initialisation
        money = startMoney;
        population = startPopulation;
        totalSpawnChances = houseSpawnChance + spaceSpawnChance;

        // Index and blocCount adjustment
        GlobalVar.blocCount++;
        index = GlobalVar.blocCount;
        maxSize = 3 * Size * randSizeFactor * level / (float)System.Math.Log10(index + 10);

        // position reajustment
        Vector3 position = transform.position;
        position.y = transform.localScale.y / 2;
        transform.position = position;


        //Vary Color from origin
        if (index != 0)
        {
            materialR += Random.Range(-10, 10);
            materialG += Random.Range(-10, 10);
            materialB += Random.Range(-10, 10);

            materialR = Mathf.Clamp(materialR, 0, 255);
            materialG = Mathf.Clamp(materialG, 0, 255);
            materialB = Mathf.Clamp(materialB, 0, 255);
        }

        // Initialize surrounding check
        SurroundCheck();

    }

    // Function to increase the instance's wallet content
    void MoneyUp()
    {
        // Random chance to enter the loop
        if (Random.Range(0f, 50 / (accelerationRatio * population)) < 2)
        {
            // If the money isn't maxed out
            if (money < 20 * population) { money += 0.5f * level * (1 + (population / (level * level * 10))); }
            // Distribution of extra money to neighbours
            foreach (GameObject i in neighbours)
            {
                if (i.GetComponent<CubeSpreading>().money <= 20 * i.GetComponent<CubeSpreading>().population)
                {
                    i.GetComponent<CubeSpreading>().money += 0.1f * (float)System.Math.Log10(population);
                    Debug.DrawRay(transform.position, i.transform.position - transform.position, Color.yellow, 0.1f, true);
                }
            }
        }
    }

    // Function to increase the instance's population content
    void PopUp()
    {
        // Random chance to enter the loop
        if (Random.Range(0, (1500 / (accelerationRatio * (population)))) < 2)
        {

            // If the money isn't maxed out
            if (population < 10 * level * level) { population += 1; }
        }
    }

    // Function to remove an inhabitant of a house (death of population)
    void PopDown()
    {
        if (population > 2 && Random.Range(0, 5000 / accelerationRatio) < 2) { population--; }
    }

    // Function to check the surroundings space of the instance
    void SurroundCheck()
    {
        // Reinitialise variables
        neighbours = new List<GameObject>();
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        float spacingray = 2 * (1 + spacing);
        // Get a position near the House base
        Vector3 basePos = transform.position - Vector3.up * transform.localScale.y / 2 + Vector3.up * Size;

        //Check Front
        // Get a list of objects in front direction (theoretically, only one will be there)
        RaycastHit[] RayHits = (Physics.SphereCastAll(basePos + 1f * Size * transform.forward, Size / 4, transform.forward, spacingray, layerMask, 0));
        // If no object is found
        if (RayHits.Length == 0)
        {
            //Draw corresponding Green Debug Ray and set the FreeSpace Vector value to 1 (free)
            Debug.DrawRay(basePos, Vector3.forward * spacingray, Color.green);
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
                Debug.DrawRay(basePos, Vector3.forward * spacingray, Color.red);
                neighbours.Add(RayHits[0].collider.gameObject);
            }
            else if (RayHits[0].collider.tag == "EmptySpace") { Debug.DrawRay(basePos, Vector3.forward * spacingray, Color.blue); }
        }

        //Check Right
        RayHits = Physics.SphereCastAll(basePos + 1f * Size * transform.right, Size / 4, transform.right, spacingray, layerMask, 0);
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
        RayHits = Physics.SphereCastAll(basePos - 1f * Size * transform.forward, Size / 4, -transform.forward, spacingray, layerMask, 0);
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
        RayHits = Physics.SphereCastAll(basePos - 1f * Size * transform.right, Size / 4, -transform.right, spacingray, layerMask, 0);
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

    // Function to spawn a new building
    public void Spawner()
    {
        // If there are less houses In-Game than the specified maximum,
        //and money and population are over half their max amount

        if (!(GlobalVar.blocCount >= houseCountMax) && money > 10 * population && population > level * level * 5 /*&& FreeSpaces == one*/)
        {
            // Decrease population and money to "pay"
            money = money / 2;
            population -= 4;
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
                    if (Random.Range(0, totalSpawnChances) > houseSpawnChance) { Prefab = prefabSpace; }
                    else { Prefab = prefabHouse; }
                }

                // Instantiate the new building
                GameObject newHouse = Instantiate(Prefab, transform.position + offset, transform.rotation);
                if(Prefab == prefabHouse)
                {
                    newHouse.transform.SetParent(HouseParent.transform);
                }
                else if (Prefab == prefabSpace)
                {
                    newHouse.transform.SetParent(EmptyParent.transform);
                }

            }
        }
    }

    // Function to increase the height of the building
    public void Sizer()
    {
        // If the instance isn't already at current max height

        if (transform.localScale.y < maxSize && money > 10 * population && population > level * level * 5 /*&& FreeSpaces == one*/)
        {
            // "pay" for the increase
            money -= 5;
            // Get the current scale and position of the instance
            Vector3 scale = transform.localScale;
            Vector3 position = transform.position;

            // Increase the scale of the building within the limits
            scale.y = Mathf.Clamp(scale.y * 1.05f, 0, maxSize);
            transform.localScale = scale;
            currentSize = transform.localScale.y;

            // Reajust the position for the house to be on the ground
            position.y = scale.y / 2;
            transform.position = position;
        }
    }

    // Function to increase the level of the building
    public void LvlUp()
    {
        if (money >= 20 * population && population >= (level * level * 10) - 1 && Random.Range(0, 2500 / accelerationRatio) >= 1 && level < 10 && currentSize >= maxSize)
        {
            // "pay" for the increase
            money = money / 2;
            population = population / 2;
            // Increase level
            level++;

            //      Adjust size
            // Get the current scale and position of the instance and adjust them to level
            Vector3 scale = transform.localScale;
            //scale.x += (spacing) / 20;
            //scale.z += (spacing) / 20;

            transform.localScale = scale;
            /*
                        //Add Color Increment
                        materialR += 5;
                        materialG += 5;
                        materialA += 5;*/

            // Increase max size
            maxSize = 3 * Size * randSizeFactor * level / (float)System.Math.Log10(index + 10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        free = FreeSpaces.w + FreeSpaces.x + FreeSpaces.y + FreeSpaces.z;

        if (free > 0)
        {
            SurroundCheck();
        }
        free = FreeSpaces.w + FreeSpaces.x + FreeSpaces.y + FreeSpaces.z;

        /*randomSpawn = Random.Range(0f, 1000 / accelerationRatio);
        if (randomSpawn < 2)
        {
            Spawner();
        }

        randomSize = Random.Range(0, 1000 / accelerationRatio);
        if (randomSize < 5)
        {
            Sizer();
        }*/

        MoneyUp();
        PopUp();
        PopDown();
        //Sizer();
        //Spawner();


        float randomaction = Random.Range(0f, 4);

        if (Random.Range(0, (1000 / accelerationRatio)) <= 1) { if (randomaction + 1 > free) { Sizer(); } }

        if (Random.Range(0, (2000 / accelerationRatio)) <= 1) { if (randomaction < free) { Spawner(); } }

        LvlUp();

        if (!isSelected)
        {
            materialColor = new Color32((byte)materialR, (byte)materialG, (byte)materialB, (byte)materialA);
            material.color = materialColor;
        }
        else
        {
            material.color = Color.white;
        }
        //if (index == 1) { Debug.Log(neighbours.Count); }
        /// ((float)System.Math.Log(GlobalVar.blocCount)+1)
        if (Random.Range(0, 2000) <= 5)
        {
            

            
            List<GameObject> housesNearby = new List<GameObject>();
            hitColliders = Physics.OverlapSphere(transform.position, distanceNavTravel);
            //print(hitColliders.Length);
            foreach (Collider hit in hitColliders)
            {
                if (hit.transform.tag == "House" || hit.GetComponent<CubeSpreading>() == this)
                {
                    housesNearby.Add(hit.gameObject);
                }
            }
            GameObject Nav = Instantiate(NavAgent, transform.position - Vector3.up * (transform.localScale.y / 2 - 2), transform.rotation);
            Nav.GetComponent<MoveTo>().goal = housesNearby[Random.Range(1, housesNearby.Count)].transform;
            Nav.transform.SetParent(NavParent.transform);
        }

    }

}
