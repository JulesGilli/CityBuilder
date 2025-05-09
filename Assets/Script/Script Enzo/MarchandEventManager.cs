using UnityEngine;

public class MarchandEventManager : MonoBehaviour
{
    private GameTime gameTime;
    private int dernierJourGlobal = 1;
    private int dernierJourMarchand = 1;
    public bool IsMarchandPresent { get; private set; } = false;

    private void Start()
    {
        gameTime = FindObjectOfType<GameTime>();
        if (gameTime != null)
            gameTime.OnDayChanged += GererChangementDeJour;
    }

    private void OnDestroy()
    {
        if (gameTime != null)
            gameTime.OnDayChanged -= GererChangementDeJour;
    }

    private void GererChangementDeJour(int year, int month, int day)
    {
        dernierJourGlobal++;

        if ((dernierJourGlobal - dernierJourMarchand) >= 3)
        {
            dernierJourMarchand = dernierJourGlobal;
            ActiverMarchand();
        }
    }

    private void ActiverMarchand()
    {
        IsMarchandPresent = true;
        Debug.Log("Le marchand est arrivé au village !");
        // Appeler ici une UI ou logique de popup
    }
}
