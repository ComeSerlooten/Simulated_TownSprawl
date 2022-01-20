using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Text houseCount;
    [SerializeField] Text speedDisplay;
    [SerializeField] Slider speedSlider;
    [SerializeField] Toggle navEnabled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddBloc1()
    {
        GlobalVar.maxHouses += 1;
        GlobalVar.maxHouses = Mathf.Clamp(GlobalVar.maxHouses, 1, 300);
    }

    public void AddBloc10()
    {
        GlobalVar.maxHouses += 10;
        GlobalVar.maxHouses = Mathf.Clamp(GlobalVar.maxHouses, 1, 300);
    }

    public void AddBloc100()
    {
        GlobalVar.maxHouses += 100;
        GlobalVar.maxHouses = Mathf.Clamp(GlobalVar.maxHouses, 1, 300);
    }

    public void RemBloc1()
    {
        GlobalVar.maxHouses -= 1;
        GlobalVar.maxHouses = Mathf.Clamp(GlobalVar.maxHouses, 1, 300);
    }

    public void RemBloc10()
    {
        GlobalVar.maxHouses -= 10;
        GlobalVar.maxHouses = Mathf.Clamp(GlobalVar.maxHouses, 1, 300);
    }

    public void RemBloc100()
    {
        GlobalVar.maxHouses -= 100;
        GlobalVar.maxHouses = Mathf.Clamp(GlobalVar.maxHouses, 1, 300);
    }

    public void GetSliderValue()
    {
        float speed = speedSlider.value == 0? 0.1f : (speedSlider.value/10) * 4;
        
        GlobalVar.speedRatio = speed;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Game_Final");
    }

    public void ExitProg()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        houseCount.text = GlobalVar.maxHouses.ToString();
        GetSliderValue();
        speedDisplay.text = "Speed : " + GlobalVar.speedRatio.ToString();
        GlobalVar.navEnabled = navEnabled.isOn;
    }
}
