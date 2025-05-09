using UnityEngine;

public class FermerPopup : MonoBehaviour
{
    public GameObject popupUI;

    public void Fermer()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }
    }
}
