using UnityEngine;

public class BlocClickable : MonoBehaviour
{
    public GameObject popupUI; // Ce sera ta popup à afficher

    void OnMouseDown()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(true);
        }
    }
}
