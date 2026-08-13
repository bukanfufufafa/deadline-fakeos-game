using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class moodbar : MonoBehaviour
{
    public Slider energy;
    public Slider stress;
    public Slider focus;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("energyDecreased", 1f, 1f);
        InvokeRepeating("focusDecreased", 1f, 1f);
        InvokeRepeating("stressIncreased", 1f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void energyDecreased()
    {
        energy.value -= 0.12f;
    }

    public void stressIncreased()
    {
        stress.value += 0.5f;
    }

    public void focusDecreased()
    {
        focus.value -= 0.5f;
    }
}
