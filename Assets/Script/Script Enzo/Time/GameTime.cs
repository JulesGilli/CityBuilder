using UnityEngine;
using TMPro;
using System;

public class GameTime : MonoBehaviour
{
    public int year = 1000;
    public int month = 1;
    public int day = 1;
    public int hour = 6;
    public int minute = 0;
    public TextMeshProUGUI timeDisplay;

    private float timer = 0f;
    public static GameTime Instance { get; private set; }

    public enum Season { Printemps, Été, Automne, Hiver }
    public Season currentSeason;

    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private int dayOfWeekIndex = 0;

    private int[] daysInMonth = {
        31, 28, 31, 30, 31, 30,
        31, 31, 30, 31, 30, 31
    };

    private float[] timeSpeeds = { 0.05f, 0.01389f, 0.001f };
    private int timeSpeedIndex = 0;

    public event Action<int, int, int> OnDayChanged; // year, month, day

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UpdateSeason();
        if (timeDisplay != null)
            timeDisplay.text = GetFormattedTime();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (timeSpeedIndex < timeSpeeds.Length - 1)
                timeSpeedIndex++;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (timeSpeedIndex > 0)
                timeSpeedIndex--;
        }

        timer += Time.deltaTime;

        if (timer >= timeSpeeds[timeSpeedIndex])
        {
            timer = 0f;
            AdvanceMinute();
        }
    }

    void AdvanceMinute()
    {
        minute++;

        if (minute >= 60)
        {
            minute = 0;
            hour++;

            if (hour >= 24)
            {
                hour = 0;
                day++;
                dayOfWeekIndex = (dayOfWeekIndex + 1) % 7;

                OnDayChanged?.Invoke(year, month, day);

                if (day > daysInMonth[month - 1])
                {
                    day = 1;
                    month++;

                    if (month > 12)
                    {
                        month = 1;
                        year++;
                    }
                    UpdateSeason();
                }
            }
        }

        if (timeDisplay != null)
            timeDisplay.text = GetFormattedTime();
    }

    void UpdateSeason()
    {
        if (month >= 3 && month <= 5)
            currentSeason = Season.Printemps;
        else if (month >= 6 && month <= 8)
            currentSeason = Season.Été;
        else if (month >= 9 && month <= 11)
            currentSeason = Season.Automne;
        else
            currentSeason = Season.Hiver;
    }

    string GetFormattedTime()
    {
        string jour = daysOfWeek[dayOfWeekIndex];
        return $"{jour} {day:D2}/{month:D2}/{year} - {hour:D2}:{minute:D2}";
    }

    public void IncreaseSpeed() => timeSpeedIndex = Mathf.Min(timeSpeedIndex + 1, timeSpeeds.Length - 1);
    public void DecreaseSpeed() => timeSpeedIndex = Mathf.Max(timeSpeedIndex - 1, 0);
}