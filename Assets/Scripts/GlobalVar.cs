using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVar : MonoBehaviour
{
    public enum timeOfDay
    {
        Day,
        Night
    }
    public static timeOfDay Time;

    [SerializeField] [Range(0.1f, 10f)] public float speed;
    public static float speedRatio;
    [SerializeField] public GameObject HouseParent;
    [SerializeField] public static List<GameObject> Houses;
    public static int blocCount = 0;
    public static int totalPop = 0;

    public static bool navEnabled = true;

    public static int maxHouses = 10;

    public static float dayLight = 0;

    private void Awake()
    {
        speedRatio = speed;
    }

    void Start()
    {
        
    }

    void Update()
    {
        totalPop = 0;
        foreach(HouseBase house in HouseParent.GetComponentsInChildren<HouseBase>())
        {
            totalPop += house.population;
        }
        //print(Houses.Count);
    }


}
