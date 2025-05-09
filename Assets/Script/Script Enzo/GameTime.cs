using UnityEngine;
using TMPro;

public class GameTime : MonoBehaviour
{
    // Temps de jeu
    public int year = 1000;
    public int month = 1;
    public int day = 1;
    public int hour = 6;
    public int minute = 0;
    public TextMeshProUGUI timeDisplay;

    // Timer
    private float timer = 0f;

    // Saisons
    public enum Season { Printemps, Été, Automne, Hiver }
    public Season currentSeason;

    // Jours de la semaine
    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private int dayOfWeekIndex = 0;

    // Jours par mois
    private int[] daysInMonth = {
        31, 28, 31, 30, 31, 30,
        31, 31, 30, 31, 30, 31
    };

    // Vitesses du temps en secondes réelles par minute de jeu
    private float[] timeSpeeds = { 0.05f, 0.01389f, 0.001f };
    private int timeSpeedIndex = 0; // 0 = lent, 1 = normal, 2 = rapide

    void Start()
    {
        UpdateSeason();
    }

    void Update()
    {
        // Passage à la vitesse supérieure (T)
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (timeSpeedIndex < timeSpeeds.Length - 1)
            {
                timeSpeedIndex++;
                Debug.Log("Vitesse augmentée : " + timeSpeeds[timeSpeedIndex]);
            }
        }

        // Passage à la vitesse inférieure (R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (timeSpeedIndex > 0)
            {
                timeSpeedIndex--;
                Debug.Log("Vitesse réduite : " + timeSpeeds[timeSpeedIndex]);
            }
        }

        // Avancer le temps
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

        string formattedTime = GetFormattedTime();
        Debug.Log(formattedTime);

        // ✅ MAJ de l'UI
        if (timeDisplay != null)
        {
            timeDisplay.text = formattedTime;
        }

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

        Debug.Log("Nouvelle saison : " + currentSeason);
    }

    string GetFormattedTime()
    {
        string jour = daysOfWeek[dayOfWeekIndex];
        return $"{jour} {day:D2}/{month:D2}/{year} - {hour:D2}:{minute:D2}";
    }

    // Pour usage futur dans ton UI
    public void IncreaseSpeed() => timeSpeedIndex = Mathf.Min(timeSpeedIndex + 1, timeSpeeds.Length - 1);
    public void DecreaseSpeed() => timeSpeedIndex = Mathf.Max(timeSpeedIndex - 1, 0);
}
