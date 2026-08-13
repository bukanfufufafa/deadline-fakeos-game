using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class knowledgeBar : MonoBehaviour
{
    public Slider math;
    public Slider science;
    public Slider language;
    public Slider computerScience;

    public float mathValue = 0;
    public float scienceValue = 0;
    public float languageValue = 0;
    public float computerValue = 0;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        math.value = mathValue;
        science.value = scienceValue;
        language.value = languageValue;
        computerScience.value = computerValue;
    }

    void mathIncreased(float value)
    {
        mathValue += value;
    }

    void scienceIncreased(float value)
    {
        scienceValue += value;
    }

    void languageIncresead(float value)
    {
        languageValue += value;
    }

    void computerIncreased(float value)
    {
        computerValue += value;
    }
}
