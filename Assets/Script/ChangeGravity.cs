using UnityEngine;
using UnityEngine.UI;

public class GravityWheelSimple : MonoBehaviour
{
    [Header("UI flèches")]
    public GameObject arrowUp;
    public GameObject arrowDown;
    public GameObject arrowLeft;
    public GameObject arrowRight;

    [Header("Clé pour ouvrir la roue")]
    public KeyCode openWheelKey = KeyCode.E;

    private bool wheelOpen = false;

    void Update()
    {
        // Ouvrir / fermer la roue
        if (Input.GetKeyDown(openWheelKey))
        {
            wheelOpen = !wheelOpen;
            SetArrowVisibility(wheelOpen);

            Cursor.lockState = wheelOpen ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = wheelOpen;
        }

        if (!wheelOpen) return;

        // Cliquer sur les flèches pour sélectionner la direction
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            if (IsMouseOver(arrowUp, mousePos))
            {
                Debug.Log("Haut sélectionné !");
                CloseWheel();
            }
            else if (IsMouseOver(arrowDown, mousePos))
            {
                Debug.Log("Bas sélectionné !");
                CloseWheel();
            }
            else if (IsMouseOver(arrowLeft, mousePos))
            {
                Debug.Log("Gauche sélectionné !");
                CloseWheel();
            }
            else if (IsMouseOver(arrowRight, mousePos))
            {
                Debug.Log("Droite sélectionné !");
                CloseWheel();
            }
        }
    }

    private bool IsMouseOver(GameObject uiElement, Vector2 mousePos)
    {
        if (uiElement == null) return false;

        RectTransform rt = uiElement.GetComponent<RectTransform>();
        Vector2 pos = rt.position;
        Vector2 size = rt.sizeDelta;

        Rect rect = new Rect(pos.x - size.x / 2, pos.y - size.y / 2, size.x, size.y);
        return rect.Contains(mousePos);
    }

    private void SetArrowVisibility(bool visible)
    {
        arrowUp.SetActive(visible);
        arrowDown.SetActive(visible);
        arrowLeft.SetActive(visible);
        arrowRight.SetActive(visible);
    }

    private void CloseWheel()
    {
        wheelOpen = false;
        SetArrowVisibility(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
