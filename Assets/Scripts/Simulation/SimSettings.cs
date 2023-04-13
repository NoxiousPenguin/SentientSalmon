using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class SimSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider popCountSlider;
    public Slider salmonMaxSpeedSlider;
    public Slider bearSpeedSlider;
    public Slider bearAggressionSlider;
    public Slider currentResistanceSlider;
    public TMP_Dropdown mapSelection;
    public TextMeshProUGUI popCountValueText;
    public TextMeshProUGUI salmonMaxSpeedValueText;
    public TextMeshProUGUI bearSpeedValueText;
    public TextMeshProUGUI bearAggressionValueText;
    public TextMeshProUGUI currentResistanceValueText;
    public Button startButton;
    public static string[] mapNames = {"Autumn Falls", "Lazy Summer", "Icy Islands"};

    void Start()
    {
       // add listeners to sliders; invoke methods when value of slider changes 
       popCountSlider.onValueChanged.AddListener(delegate {ChangeSliderValue(popCountSlider, popCountValueText);});
       salmonMaxSpeedSlider.onValueChanged.AddListener(delegate {ChangeSliderValue(salmonMaxSpeedSlider, salmonMaxSpeedValueText);});
       bearSpeedSlider.onValueChanged.AddListener(delegate {ChangeSliderValue(bearSpeedSlider, bearSpeedValueText);});
       bearAggressionSlider.onValueChanged.AddListener(delegate {ChangeSliderValue(bearAggressionSlider, bearAggressionValueText);});
       currentResistanceSlider.onValueChanged.AddListener(delegate {ChangeSliderValue(currentResistanceSlider, currentResistanceValueText);});
       mapSelection.onValueChanged.AddListener(delegate {ChangeDropdownValue(mapSelection);});
       startButton.onClick.AddListener(delegate { StartTrainingButton(); });

       // display default slider values...can be modified in Unity Editor
       ChangeSliderValue(popCountSlider, popCountValueText);
       ChangeSliderValue(salmonMaxSpeedSlider, salmonMaxSpeedValueText);
       ChangeSliderValue(bearSpeedSlider, bearSpeedValueText);
       ChangeSliderValue(bearAggressionSlider, bearAggressionValueText);
       ChangeSliderValue(currentResistanceSlider, currentResistanceValueText);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSliderValue(Slider slider, TextMeshProUGUI valueText)
    {
        Debug.Log((int)slider.value);
        valueText.text = ((int)slider.value).ToString();
    }

    public void ChangeDropdownValue(TMP_Dropdown dropdown)
    {
        Debug.Log(mapNames[dropdown.value]);
    }

    void OnDisable()
    {
        Debug.Log("Value of popCount: " + (int)popCountSlider.value);
        Debug.Log("Value of salmonMaxSpeed: " + (int)salmonMaxSpeedSlider.value);
        Debug.Log("Value of currentResistance " + (int)currentResistanceSlider.value);
        Debug.Log("Value of bearSpeed: " + (int)bearSpeedSlider.value);
        Debug.Log("Value of spottingRange: " + (int)bearAggressionSlider.value);
        PlayerPrefs.SetInt("popCount", (int)popCountSlider.value);
        PlayerPrefs.SetFloat("salmonMaxSpeed", (int)salmonMaxSpeedSlider.value);
        PlayerPrefs.SetFloat("currentResistance", (int)currentResistanceSlider.value);
        PlayerPrefs.SetFloat("bearSpeed", (int)bearSpeedSlider.value);
        PlayerPrefs.SetFloat("spottingRange", (int)bearAggressionSlider.value);
    }

    public void StartTrainingButton()
    {

        PlayerPrefs.SetInt("mapSelection", mapSelection.value); // for the gamestatemanager

        if (mapNames[mapSelection.value] == "Autumn Falls")
        {
            SceneManager.LoadScene("AutumnFallsGameScene");
        }
        else if (mapNames[mapSelection.value] == "Lazy Summer")
        {
            SceneManager.LoadScene("Map - The Lazy Summer");
        }
        else if (mapNames[mapSelection.value] == "Icy Islands")
        {
            SceneManager.LoadScene("Map - Icy Islands");
        }
    }
}
