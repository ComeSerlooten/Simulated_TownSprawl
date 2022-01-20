// MoveTo.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public GameObject origin;
    public Transform goal;
    NavMeshAgent agent;
    Vector3 posGoalAtZero;
    [SerializeField] float waitingTimeInSec = 1;
    [SerializeField] float stoppingDistStep = 5;
    [SerializeField] int stepsToDestinationMin = 3;
    [SerializeField] int stepsToDestinationMax = 5;
    [SerializeField] int streetDistance = 3;
    int randomSteps;
    int stepCounter;
    float waitCounter;
    bool waiting = false;

    void Start()
    {
        //goal = GlobalVar.Houses[Random.Range(0, GlobalVar.Houses.Count)].transform;
        agent = this.GetComponent<NavMeshAgent>();
        /*posGoalAtZero = goal.position;
        posGoalAtZero.y = 0;
        agent.destination = posGoalAtZero;*/

        agent.stoppingDistance = stoppingDistStep;
        stepCounter = 0;
        randomSteps = Random.Range(stepsToDestinationMin, stepsToDestinationMax + 1);

        GameObject house = origin;

        for (int i = 0; i < streetDistance; i++)
        {
            if (house.GetComponent<HouseBase>().neighbours.Count != 0)
            {
                house = house.GetComponent<HouseBase>().neighbours[Random.Range(0, house.GetComponent<HouseBase>().neighbours.Count)];
            }
        }
        goal = (house == origin) ? null : house.transform;
        if (goal == null) Destroy(this.gameObject);





    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "House")
        {
            print("ouch");
        }
    }

    private void Update()
    {

        /*posGoalAtZero = goal.position;
        posGoalAtZero.y = 0;
        this.GetComponent<NavMeshAgent>().destination = posGoalAtZero;*/
        /*if(this.GetComponent<NavMeshAgent>().remainingDistance <= 0)
        {
            List<GameObject> housesNearby = new List<GameObject>();
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 25);
            //print(hitColliders.Length);
            foreach (Collider hit in hitColliders)
            {
                if (hit.transform.tag == "House" || hit.GetComponent<HouseBase>() == goal.GetComponent<HouseBase>())
                {
                    housesNearby.Add(hit.gameObject);
                }
            }
            goal = housesNearby[Random.Range(1, housesNearby.Count)].transform;

        }
        else if(this.GetComponent<NavMeshAgent>().remainingDistance <= 4)
        {
            Destroy(this.gameObject);
        }*/

        posGoalAtZero = goal.position + goal.forward * 1.1f;
        posGoalAtZero.y = 0;
        agent.destination = posGoalAtZero;

        if (waiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitingTimeInSec)
            {
                waiting = false;
                waitCounter = 0;
            }
        }
        else
        {
            waitCounter = 0;
            if (agent.remainingDistance < stoppingDistStep && stepCounter < randomSteps - 1)
            {
                GameObject house = goal.gameObject;
                for (int i = 0; i < streetDistance; i++)
                {
                    if (house.GetComponent<HouseBase>().neighbours.Count != 0)
                    {
                        house = house.GetComponent<HouseBase>().neighbours[Random.Range(0, house.GetComponent<HouseBase>().neighbours.Count)];
                    }
                }
                goal = house.transform;
                waiting = true;
                stepCounter++;
            }
            else if (agent.remainingDistance < stoppingDistStep && stepCounter == randomSteps - 1)
            {
                goal.GetComponent<HouseBase>().population += 2;
                Destroy(this.gameObject);
            }
        }


        if (goal == null || !agent.isOnNavMesh) Destroy(this.gameObject);
    }

}
