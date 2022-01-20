using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    [SerializeField] public Material brick;
    [SerializeField] public Material window;
    [Space]
    [SerializeField] public int brickIndex;
    [SerializeField] public int windowIndex;
    [Space]
    [SerializeField] [ColorUsage(true, true)] public Color32 dayColor;
    [SerializeField] [ColorUsage(true, true)] public Color32 nightColor;
    Color32 currentColor;

    float randomWindowSpeed;

    //int materialIndex;


    private void Awake()
    {
        brick = GetComponent<MeshRenderer>().materials[brickIndex];
        window = GetComponent<MeshRenderer>().materials[windowIndex];
        window.EnableKeyword("_EMISSION");

        if (GlobalVar.Time == GlobalVar.timeOfDay.Day)
        {
            SetWindowLight(dayColor);
        }
        else
        {
            SetWindowLight(nightColor);
        }

        currentColor = dayColor; //window.GetColor("_EmissionColor");
    }

    // Start is called before the first frame update
    void Start()
    {
        float Size = GetComponentInParent<HouseBase>().Size;
        float offsetX = Random.Range(-0.05f, 0.05f) * Size;
        float offsetZ = Random.Range(-0.05f, 0.05f) * Size;
        transform.position += offsetX * GetComponentInParent<Transform>().right + offsetZ * GetComponentInParent<Transform>().forward;
        transform.Rotate(Vector3.up, 90 * Random.Range(0, 4) + Random.Range(-5,5));
        randomWindowSpeed = Random.Range(0.1f, 1.5f);
    }

    public void SetBrickColor(int materialR, int materialG, int materialB)
    {
        Color32 materialColor = new Color32((byte)materialR, (byte)materialG, (byte)materialB, 1);
        GetComponent<MeshRenderer>().materials[brickIndex].color = materialColor;
    }

    public void SetWindowLight(Color32 color)
    {
        window.SetColor("_EmissionColor", color);
    }


    // Update is called once per frame
    void Update()
    {
        if (GlobalVar.Time == GlobalVar.timeOfDay.Day)
        {
            currentColor = Color.Lerp(currentColor, dayColor, 0.01f);
        }
        else 
        {
            currentColor = Color.Lerp(currentColor, nightColor, 0.01f);
        }
        SetWindowLight(currentColor);


        //GetComponent<MeshRenderer>().materials[materialIndex].SetColor("_EmissionColor", DaylightRotation.currentColor);
    }
}
