using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightRotation : MonoBehaviour
{
    [Range(0.01f, 2.0f)]
    [SerializeField] float dayLength = 1.0f;

    [SerializeField] GameObject dayLight;
    [SerializeField] GameObject nightLight;
    [Space]
    [SerializeField] Color32 dayFog;
    [SerializeField] Color32 nightFog;
    Color32 currentFog;
    [Space]
    [SerializeField] Transform background;
    [SerializeField] Color32 backgroundColor;
    [SerializeField] Color32 backgroundNightColor;
    List<Transform> backgroundItems = new List<Transform>();


    Light dayLightIntensity;
    Light nightLightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        dayLightIntensity = dayLight.GetComponent<Light>();
        nightLightIntensity = nightLight.GetComponent<Light>();
        //if (dayLight.transform.position.y < -2.0f) currentColor = nightWindow;
        currentFog = RenderSettings.fogColor;

        for (int i = 0; i < background.childCount; i++)
        {
            Transform group = background.GetChild(i);
            foreach (Transform item in group.GetComponentsInChildren<Transform>())
            {
                if (item.GetComponent<SpriteRenderer>()) { backgroundItems.Add(item); }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, transform.right, 0.01f * GlobalVar.speedRatio / dayLength );

        if (dayLight.transform.position.y < -1.0f)
        {
            GlobalVar.Time = GlobalVar.timeOfDay.Night;

            dayLightIntensity.intensity -= 0.001f;
            dayLightIntensity.intensity = Mathf.Clamp(dayLightIntensity.intensity, -0.1f, 1);
            GlobalVar.dayLight -= 0.01f;
        }
        else if (dayLightIntensity.intensity < 1/* && dayLight.transform.position.y < 0.0f*/)
        {
            dayLightIntensity.intensity += 0.001f;

            foreach (Transform item in backgroundItems)
            {
                item.GetComponent<SpriteRenderer>().color = Color.Lerp(item.GetComponent<SpriteRenderer>().color, backgroundColor, 0.01f);
            }

            //currentFog = Color.Lerp(currentFog, dayFog, 0.01f);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 1, 0.01f);

            //RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.02f, 0.0001f);

            GlobalVar.dayLight += 0.01f;
        }

        if (nightLight.transform.position.y < -1.0f)
        {
            GlobalVar.Time = GlobalVar.timeOfDay.Day;

            nightLightIntensity.intensity -= 0.001f;



            nightLightIntensity.intensity = Mathf.Clamp(nightLightIntensity.intensity, -1, 0.5f);
        }
        else if (nightLightIntensity.intensity < 0.5f/* && nightLight.transform.position.y < 0.0f*/)
        {
            nightLightIntensity.intensity += 0.001f;

            foreach (Transform item in backgroundItems)
            {
                item.GetComponent<SpriteRenderer>().color = Color.Lerp(item.GetComponent<SpriteRenderer>().color, backgroundNightColor, 0.01f);
            }

            //currentFog = Color.Lerp(currentFog, nightFog, 0.01f);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.25f, 0.001f);
            //RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, 0.05f, 0.0001f);
        }
        //print(GlobalVar.Time);

        DynamicGI.UpdateEnvironment();
    }
}
