using UnityEngine;
using TMPro;

public class clock : MonoBehaviour
{
    public int hour = 8;
    public int minute = 0;

    [SerializeField] private TMP_Text clockText;

    void Start()
    {
        UpdateClockText();
        InvokeRepeating(nameof(minuteIncreased), 10f, 10f);
    }

    public void minuteIncreased()
    {
        minute += 10;

        if (minute >= 60)
        {
            minute -= 60;
            hour++;
        }

        if (hour >= 24)
        {
            hour = 0;
        }

        UpdateClockText();
    }

    void UpdateClockText()
    {
        clockText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }
}